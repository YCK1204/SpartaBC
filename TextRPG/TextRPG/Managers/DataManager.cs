using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TextRPG.Utils;

namespace TextRPG.Managers
{
    internal class Data<TWrapper, T> where TWrapper : new()
    {
        public Dictionary<int, T> MakeDictionary(Func<T, int> keySelector, string json)
        {
            TWrapper wrapper = JsonConvert.DeserializeObject<TWrapper>(json);
            var list = wrapper.GetType()
                              .GetProperties()
                              .Where(p => p.PropertyType == typeof(List<T>))
                              .Select(p => (List<T>)p.GetValue(wrapper))
                              .FirstOrDefault();

            if (list == null)
                return new Dictionary<int, T>();

            return list.ToDictionary(keySelector);
        }
    }
    public class DataManager
    {
        public Dictionary<int, Item> Items = new Dictionary<int, Item>();
        public Dictionary<int, CharacterStatus> CharacterStatuses = new Dictionary<int, CharacterStatus>();
        public Dictionary<int, PlayerCaracter> PlayerCharacters = new Dictionary<int, PlayerCaracter>();
        public Dictionary<string, MonsterData> Monsters = new Dictionary<string, MonsterData>();
        
        // 타입별 아이템 리스트
        public List<Item> Helmets = new List<Item>();
        public List<Item> Armors = new List<Item>();
        public List<Item> Shoes = new List<Item>();
        public List<Item> Swords = new List<Item>();
        public List<Item> Potions = new List<Item>();

        string _dataPath = "./Data";
        string _itemPath = "Items.json";
        string _characterStatusPath = "CharacterStatuses.json";
        string _playerCharacterPath = "PlayerCharacters.json";
        string _monstersPath = "Monsters.json";

        public void LoadData()
        {
            string charJson = File.ReadAllText($"{_dataPath}/{_characterStatusPath}");
            var charData = new Data<CharacterStatuses, CharacterStatus>();
            CharacterStatuses = charData.MakeDictionary(c => (int)c.Id, charJson);  // key는 enum값도 OK

            string itemJson = File.ReadAllText($"{_dataPath}/{_itemPath}");
            var itemData = new Data<Items, Item>();
            Items = itemData.MakeDictionary(i => i.Id, itemJson);
            
            // 타입별로 아이템 분류
            CategorizeItemsByType();

            string playerCharInfoJson = File.ReadAllText($"{_dataPath}/{_playerCharacterPath}");
            var playerCharData = new Data<PlayerCharacters, PlayerCaracter>();
            PlayerCharacters = playerCharData.MakeDictionary(p => p.CharacterId, playerCharInfoJson);
            
            // 기존 캐릭터들의 인벤토리 초기화
            InitializeInventoryForExistingCharacters();

            // 몬스터 데이터 로드 (ASCII 아트 포함)
            LoadMonsterData();
        }
        public void SaveData()
        {
            // Dictionary를 PlayerCharacters 구조체로 변환하여 "Characters" 배열로 감싸기
            var playerCharactersWrapper = new PlayerCharacters
            {
                Characters = PlayerCharacters.Values.ToList()
            };
            
            string json = JsonConvert.SerializeObject(playerCharactersWrapper, Formatting.Indented);
            File.WriteAllText($"{_dataPath}/{_playerCharacterPath}", json, Encoding.UTF8);
        }
        // 아이템을 타입별로 분류하는 메서드
        private void CategorizeItemsByType()
        {
            // 기존 리스트 초기화
            Helmets.Clear();
            Armors.Clear();
            Shoes.Clear();
            Swords.Clear();
            Potions.Clear();

            // 모든 아이템을 타입별로 분류
            foreach (var item in Items.Values)
            {
                switch (item.Type)
                {
                    case ItemType.Helmet:
                        Helmets.Add(item);
                        break;
                    case ItemType.Armor:
                        Armors.Add(item);
                        break;
                    case ItemType.Shoes:
                        Shoes.Add(item);
                        break;
                    case ItemType.Sword:
                        Swords.Add(item);
                        break;
                    case ItemType.Potion:
                        Potions.Add(item);
                        break;
                }
            }

        }

        // 특정 타입의 아이템 리스트 가져오기
        public List<Item> GetItemsByType(ItemType itemType)
        {
            return itemType switch
            {
                ItemType.Helmet => Helmets,
                ItemType.Armor => Armors,
                ItemType.Shoes => Shoes,
                ItemType.Sword => Swords,
                ItemType.Potion => Potions,
                _ => new List<Item>()
            };
        }

        public void AddPlayerCharacterAndSave(CharacterStatus status)
        {
            PlayerCaracter character = new PlayerCaracter();
            int id = GenerateNewCharacterId();
            character.CharacterId = id;
            character.Status = status;
            // 새 캐릭터에게 빈 인벤토리 초기화
            character.Inventory = new Inventory
            {
                Helmets = new List<Item>(),
                Armors = new List<Item>(),
                Swords = new List<Item>(),
                Shoes = new List<Item>(),
                Potions = new List<Item>()
            };
            // 새 캐릭터에게 빈 장비 초기화
            character.Equipment = new Equipment
            {
                Helmet = null,
                Armor = null,
                Sword = null,
                Shoes = null
            };
            PlayerCharacters.Add(id, character);
            SaveData();
        }

        public int GenerateNewCharacterId()
        {
            if (PlayerCharacters.Count == 0)
                return 1; // 첫 번째 캐릭터 ID는 1
            return PlayerCharacters.Last().Value.CharacterId + 1;
        }
        // 기존 캐릭터들의 인벤토리 및 장비 초기화 (JSON에 정보가 없는 경우)
        public void InitializeInventoryForExistingCharacters()
        {
            bool needsSave = false;
            var charactersToUpdate = new Dictionary<int, PlayerCaracter>();

            foreach (var kvp in PlayerCharacters)
            {
                var character = kvp.Value;
                bool updated = false;

                // 인벤토리 초기화
                if (character.Inventory.Helmets == null)
                {
                    character.Inventory = new Inventory
                    {
                        Helmets = new List<Item>(),
                        Armors = new List<Item>(),
                        Swords = new List<Item>(),
                        Shoes = new List<Item>(),
                        Potions = new List<Item>()
                    };
                    updated = true;
                }

                // 장비 초기화 (Equipment가 설정되지 않은 경우)
                if (!character.Equipment.Helmet.HasValue && 
                    !character.Equipment.Armor.HasValue && 
                    !character.Equipment.Sword.HasValue && 
                    !character.Equipment.Shoes.HasValue)
                {
                    character.Equipment = new Equipment
                    {
                        Helmet = null,
                        Armor = null,
                        Sword = null,
                        Shoes = null
                    };
                    updated = true;
                }

                if (updated)
                {
                    charactersToUpdate[kvp.Key] = character;
                    needsSave = true;
                }
            }

            // 업데이트된 캐릭터들을 딕셔너리에 반영
            foreach (var kvp in charactersToUpdate)
            {
                PlayerCharacters[kvp.Key] = kvp.Value;
            }

            if (needsSave)
            {
                SaveData();
            }
        }

        // 몬스터 데이터 로드 (ASCII 아트 포함)
        private void LoadMonsterData()
        {
            try
            {
                string monsterJson = File.ReadAllText($"{_dataPath}/{_monstersPath}");
                var monstersData = JsonConvert.DeserializeObject<Monsters>(monsterJson);
                
                if (monstersData.MonsterDict != null)
                {
                    // 몬스터 데이터를 로드하고 ASCII 아트도 함께 로드
                    var updatedMonsters = new Dictionary<string, MonsterData>();
                    
                    foreach (var kvp in monstersData.MonsterDict)
                    {
                        var monsterData = kvp.Value;
                        
                        // ASCII 아트 로드
                        monsterData.LoadAsciiArt();
                        
                        updatedMonsters[kvp.Key] = monsterData;
                    }
                    
                    Monsters = updatedMonsters;
                    
                    foreach (var monster in Monsters)
                    {
                        int asciiLines = monster.Value.AsciiArt?.Count ?? 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Monsters = new Dictionary<string, MonsterData>();
            }
        }

        // 몬스터 데이터 가져오기 (ASCII 아트 포함)
        public MonsterData? GetMonsterData(string monsterKey)
        {
            if (Monsters.ContainsKey(monsterKey))
            {
                return Monsters[monsterKey];
            }
            return null;
        }
    }
}
