using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextRPG.Utils
{
    public struct PlayerCaracter
    {
        [JsonProperty("CharacterStatus")]
        public CharacterStatus Status { get; set; }
        public int CharacterId { get; set; }
        public Inventory Inventory { get; set; }
        public Equipment Equipment { get; set; }
    }
    public struct PlayerCharacters
    {
        public List<PlayerCaracter> Characters { get; set; }
    }
    public struct CharacterStatus
    {
        public int Id { get; set; } // Enum value
        public int HP { get; set; }
        public string JobName { get; set; }
        public string DisplayName { get; set; }
        public int AttackPower { get; set; }
        public int Armor { get; set; }
        public int Level { get; set; }
        public int Experience { get; set; }
        public int Gold { get; set; }
        public int MaxHP { get; set; }
        public string CharacterSummary { get; set; }
        public CharacterType Type { get; set; }
    }
    public struct CharacterStatuses
    {
        [JsonProperty("CharacterStatuses")]
        public List<CharacterStatus> Statuses { get; set; }
    }
    public struct Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ItemType Type { get; set; }
        public int Power { get; set; }
        public int Gold { get; set; }
        public string Summary { get; set; }
    }
    public struct Items
    {
        [JsonProperty("Items")]
        public List<Item> _Items { get; set; }
        public List<Item> Helmets { get; set; }
        public List<Item> Armors { get; set; }
        public List<Item> Swords { get; set; }
        public List<Item> Shoes { get; set; }
        public List<Item> Potions { get; set; }
    }
    public struct Inventory
    {
        public List<Item> Helmets { get; set; }
        public List<Item> Armors { get; set; }
        public List<Item> Swords { get; set; }
        public List<Item> Shoes { get; set; }
        public List<Item> Potions { get; set; }
    }

    public struct Equipment
    {
        public Item? Helmet { get; set; }
        public Item? Armor { get; set; }
        public Item? Sword { get; set; }
        public Item? Shoes { get; set; }
    }

    public struct MonsterData
    {
        public string Name { get; set; }
        public string NameHide { get; set; }
        public List<string> AttackMsgs { get; set; }
        public int Level { get; set; }
        public int HP { get; set; }
        public int MaxHP { get; set; }
        public int Armor { get; set; }
        public int Power { get; set; }
        public int Experience { get; set; }
        public int Gold { get; set; }
        public string AsciiPath { get; set; }
        
        // UIManager에서 사용하는 AttackPower 프로퍼티 추가
        [JsonIgnore]
        public int AttackPower => Power;
        
        // ASCII 아트 데이터 (로드된 후 저장)
        [JsonIgnore]
        public List<string> AsciiArt { get; set; }
        
        // ASCII 아트 로드
        public void LoadAsciiArt()
        {
            if (string.IsNullOrEmpty(AsciiPath))
            {
                AsciiArt = new List<string>();
                return;
            }

            try
            {
                string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AsciiPath);
                if (File.Exists(fullPath))
                {
                    AsciiArt = File.ReadAllLines(fullPath, Encoding.UTF8).ToList();
                }
                else
                {
                    AsciiArt = new List<string>();
                }
            }
            catch (Exception ex)
            {
                AsciiArt = new List<string>();
            }
        }
        
        // ASCII 아트 출력
        public void DisplayAsciiArt(int startX = 0, int startY = 0)
        {
            if (AsciiArt == null || AsciiArt.Count == 0)
            {
                Console.SetCursorPosition(startX, startY);
                return;
            }

            for (int i = 0; i < AsciiArt.Count; i++)
            {
                Console.SetCursorPosition(startX, startY + i);
                Console.WriteLine(AsciiArt[i]);
            }
        }
    }

    public struct Monsters
    {
        [JsonProperty("Monsters")]
        public Dictionary<string, MonsterData> MonsterDict { get; set; }
    }
}