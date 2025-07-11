using TextRPG.Utils;
using TextRPG.Managers;

public class Player : GameObject
{
    static Player _instance = new Player();
    CharacterStatus _status = new CharacterStatus();
    IPlayerStateBase _state = IntroState.Instance;
    IPlayerStateBase _prevState = IntroState.Instance;
    int _id;
    public static int ID
    {
        get { return _instance._id; }
        set { _instance._id = value; }
    }
    #region Status
    public static int Gold
    {
        get { return _instance._status.Gold; }
        set
        {
            _instance._status.Gold = value;
            UpdatePlayerDataInManager();
        }
    }
    public static int AttackPower
    {
        get { return _instance._status.AttackPower; }
        set
        {
            _instance._status.AttackPower = value;
            UpdatePlayerDataInManager();
        }
    }
    public static int HP
    {
        get { return _instance._status.HP; }
        set
        {
            _instance._status.HP = value;
            UpdatePlayerDataInManager();
        }
    }
    public static int MaxHP
    {
        get { return _instance._status.MaxHP; }
        set
        {
            _instance._status.MaxHP = value;
            UpdatePlayerDataInManager();
        }
    }
    public static int Level
    {
        get { return _instance._status.Level; }
        set
        {
            _instance._status.Level = value;
            UpdatePlayerDataInManager();
        }
    }
    public static int Experience
    {
        get { return _instance._status.Experience; }
        set
        {
            _instance._status.Experience = value;
            UpdatePlayerDataInManager();
        }
    }
    public static int Armor
    {
        get { return _instance._status.Armor; }
        set
        {
            _instance._status.Armor = value;
            UpdatePlayerDataInManager();
        }
    }
    public static CharacterStatus Status
    {
        get { return _instance._status; }
        set { _instance._status = value; }
    }
    public static List<Item> Potions
    {
        get
        {
            var inventory = Manager.Data.PlayerCharacters[ID].Inventory;
            return inventory.Potions;
        }
        set
        {
            var playerData = Manager.Data.PlayerCharacters[ID];
            var inventory = playerData.Inventory;
            inventory.Potions = value;
            playerData.Inventory = inventory;
            Manager.Data.PlayerCharacters[ID] = playerData;
            Manager.Data.SaveData();
        }
    }
    public static List<Item> Helmets
    {
        get
        {
            var inventory = Manager.Data.PlayerCharacters[ID].Inventory;
            return inventory.Helmets;
        }
        set
        {
            var playerData = Manager.Data.PlayerCharacters[ID];
            var inventory = playerData.Inventory;
            inventory.Helmets = value;
            playerData.Inventory = inventory;
            Manager.Data.PlayerCharacters[ID] = playerData;
            Manager.Data.SaveData();
        }
    }
    public static List<Item> Armors
    {
        get
        {
            var inventory = Manager.Data.PlayerCharacters[ID].Inventory;
            return inventory.Armors;
        }
        set
        {
            var playerData = Manager.Data.PlayerCharacters[ID];
            var inventory = playerData.Inventory;
            inventory.Armors = value;
            playerData.Inventory = inventory;
            Manager.Data.PlayerCharacters[ID] = playerData;
            Manager.Data.SaveData();
        }
    }
    public static List<Item> Shoes
    {
        get
        {
            var inventory = Manager.Data.PlayerCharacters[ID].Inventory;
            return inventory.Shoes;
        }
        set
        {
            var playerData = Manager.Data.PlayerCharacters[ID];
            var inventory = playerData.Inventory;
            inventory.Shoes = value;
            playerData.Inventory = inventory;
            Manager.Data.PlayerCharacters[ID] = playerData;
            Manager.Data.SaveData();
        }
    }
    public static List<Item> Swords
    {
        get
        {
            var inventory = Manager.Data.PlayerCharacters[ID].Inventory;
            return inventory.Swords;
        }
        set
        {
            var playerData = Manager.Data.PlayerCharacters[ID];
            var inventory = playerData.Inventory;
            inventory.Swords = value;
            playerData.Inventory = inventory;
            Manager.Data.PlayerCharacters[ID] = playerData;
            Manager.Data.SaveData();
        }
    }
    public static Inventory Inventory
    {
        get
        {
            return Manager.Data.PlayerCharacters[ID].Inventory;
        }
        set
        {
            var playerData = Manager.Data.PlayerCharacters[ID];
            playerData.Inventory = value;
            Manager.Data.PlayerCharacters[ID] = playerData;
            Manager.Data.SaveData();
        }
    }
    #endregion
    public static Position Position { get { return _instance.Pos; } set { _instance.Pos = value; } }
    public static IPlayerStateBase State { get { return _instance._state; } }
    public static Monster? Monster { get; set; } = null; // 현재 플레이어가 상호작용 중인 몬스터
    public static void ChangeState<T1, T2>() where T1 : IPlayerState<T1, T2>
    {
        _instance._prevState = _instance._state;
        _instance._state.Exit();
        _instance._state = T1.Instance as IPlayerState<T1, T2>;
        _instance._state.Enter();
    }
    public static void OnSet()
    {
        CharacterStatus status = Manager.Data.CharacterStatuses[Status.Id];

        MaxHP = status.MaxHP + (10 * Level);
        HP = MaxHP;
        AttackPower = status.AttackPower + Level;
        Armor = status.Armor + Level;
    }
    public static bool LevelUp()
    {
        int curLevel = Level;
        int desireExp = curLevel * 100;
        int curExp = Experience;
        bool levelUp = false;

        while (desireExp <= curExp)
        {
            curLevel++;
            curExp -= desireExp;
            desireExp += 100;
        }
        if (Level != curLevel)
            levelUp = true;
        Experience = curExp;
        Level = curLevel;
        return levelUp;
    }
    public static void Update()
    {
        State.Update();
    }

    // 특정 위치로 직접 설정 (포탈 이용 시 등)
    public static void SetPosition(int x, int y)
    {
        Position = new Position(x, y);
    }
    #region 아이템, 장비
    // 인벤토리 관련 메서드들
    public static void InitializeInventory()
    {
        var playerData = Manager.Data.PlayerCharacters[ID];
        if (playerData.Inventory.Helmets == null)
        {
            var newInventory = new Inventory
            {
                Helmets = new List<Item>(),
                Armors = new List<Item>(),
                Swords = new List<Item>(),
                Shoes = new List<Item>(),
                Potions = new List<Item>()
            };
            playerData.Inventory = newInventory;
            Manager.Data.PlayerCharacters[ID] = playerData;
        }
    }

    public static void AddItemToInventory(Item item)
    {
        var playerData = Manager.Data.PlayerCharacters[ID];
        var inventory = playerData.Inventory;

        switch (item.Type)
        {
            case ItemType.Helmet:
                inventory.Helmets.Add(item);
                break;
            case ItemType.Armor:
                inventory.Armors.Add(item);
                break;
            case ItemType.Sword:
                inventory.Swords.Add(item);
                break;
            case ItemType.Shoes:
                inventory.Shoes.Add(item);
                break;
            case ItemType.Potion:
                inventory.Potions.Add(item);
                break;
        }

        playerData.Inventory = inventory;
        Manager.Data.PlayerCharacters[ID] = playerData;
        Manager.Data.SaveData();
    }

    public static bool RemoveItemFromInventory(Item item)
    {
        var playerData = Manager.Data.PlayerCharacters[ID];
        var inventory = playerData.Inventory;
        bool removed = false;

        switch (item.Type)
        {
            case ItemType.Helmet:
                removed = inventory.Helmets.Remove(item);
                break;
            case ItemType.Armor:
                removed = inventory.Armors.Remove(item);
                break;
            case ItemType.Sword:
                removed = inventory.Swords.Remove(item);
                break;
            case ItemType.Shoes:
                removed = inventory.Shoes.Remove(item);
                break;
            case ItemType.Potion:
                removed = inventory.Potions.Remove(item);
                break;
        }

        if (removed)
        {
            playerData.Inventory = inventory;
            Manager.Data.PlayerCharacters[ID] = playerData;
            Manager.Data.SaveData();
        }

        return removed;
    }

    public static List<Item> GetInventoryByType(ItemType itemType)
    {
        var playerData = Manager.Data.PlayerCharacters[ID];
        var inventory = playerData.Inventory;

        return itemType switch
        {
            ItemType.Helmet => inventory.Helmets ?? new List<Item>(),
            ItemType.Armor => inventory.Armors ?? new List<Item>(),
            ItemType.Sword => inventory.Swords ?? new List<Item>(),
            ItemType.Shoes => inventory.Shoes ?? new List<Item>(),
            ItemType.Potion => inventory.Potions ?? new List<Item>(),
            _ => new List<Item>()
        };
    }

    public static int GetTotalInventoryCount()
    {
        var playerData = Manager.Data.PlayerCharacters[ID];
        var inventory = playerData.Inventory;

        return (inventory.Helmets?.Count ?? 0) +
               (inventory.Armors?.Count ?? 0) +
               (inventory.Swords?.Count ?? 0) +
               (inventory.Shoes?.Count ?? 0) +
               (inventory.Potions?.Count ?? 0);
    }

    // 장비 착용
    public static void EquipItem(Item item)
    {
        var playerData = Manager.Data.PlayerCharacters[ID];
        var equipment = playerData.Equipment;
        Item? previousItem = null;

        // 기존 장착 아이템 확인 및 교체
        switch (item.Type)
        {
            case ItemType.Helmet:
                previousItem = equipment.Helmet;
                equipment.Helmet = item;
                break;
            case ItemType.Armor:
                previousItem = equipment.Armor;
                equipment.Armor = item;
                break;
            case ItemType.Sword:
                previousItem = equipment.Sword;
                equipment.Sword = item;
                break;
            case ItemType.Shoes:
                previousItem = equipment.Shoes;
                equipment.Shoes = item;
                break;
            default:
                return;
        }

        // 기존 장착 아이템이 있다면 인벤토리에 추가
        if (previousItem.HasValue)
        {
            AddItemToInventory(previousItem.Value);
        }

        // 새로 장착할 아이템을 인벤토리에서 제거
        RemoveItemFromInventory(item);

        // 장비 정보 저장
        playerData.Equipment = equipment;
        Manager.Data.PlayerCharacters[ID] = playerData;
        Manager.Data.SaveData();

    }

    // 장비 해제
    public static Item? UnequipItem(ItemType itemType)
    {
        var playerData = Manager.Data.PlayerCharacters[ID];
        var equipment = playerData.Equipment;
        Item? unequippedItem = null;

        switch (itemType)
        {
            case ItemType.Helmet:
                unequippedItem = equipment.Helmet;
                equipment.Helmet = null;
                break;
            case ItemType.Armor:
                unequippedItem = equipment.Armor;
                equipment.Armor = null;
                break;
            case ItemType.Sword:
                unequippedItem = equipment.Sword;
                equipment.Sword = null;
                break;
            case ItemType.Shoes:
                unequippedItem = equipment.Shoes;
                equipment.Shoes = null;
                break;
        }

        if (unequippedItem.HasValue)
        {
            // 해제한 아이템을 인벤토리에 추가
            AddItemToInventory(unequippedItem.Value);

            playerData.Equipment = equipment;
            Manager.Data.PlayerCharacters[ID] = playerData;
            Manager.Data.SaveData();

        }

        return unequippedItem;
    }

    // 포션 사용
    public static void UsePotion(Item potion)
    {
        if (potion.Type != ItemType.Potion)
        {
            return;
        }

        // HP 회복
        int currentHP = HP;
        int maxHP = MaxHP;
        int healAmount = potion.Power;

        int newHP = Math.Min(maxHP, currentHP + healAmount);
        HP = newHP;


        // 인벤토리에서 포션 제거
        RemoveItemFromInventory(potion);
    }

    // 아이템 되팔기
    public static void SellItem(Item item)
    {
        // 되팔 가격은 원가의 80%
        int sellPrice = (int)(item.Gold * 0.8f);

        // 골드 추가
        Gold += sellPrice;


        // 인벤토리에서 아이템 제거
        RemoveItemFromInventory(item);
    }

    // 장비 보너스 계산된 공격력
    public static int GetTotalAttackPower()
    {
        int baseAttack = _instance._status.AttackPower;
        int equipmentBonus = 0;

        var playerData = Manager.Data.PlayerCharacters[ID];
        var equipment = playerData.Equipment;

        if (equipment.Sword.HasValue)
        {
            equipmentBonus += equipment.Sword.Value.Power;
        }

        return baseAttack + equipmentBonus;
    }

    // 장비 보너스 계산된 방어력
    public static int GetTotalArmor()
    {
        int baseArmor = _instance._status.Armor;
        int equipmentBonus = 0;

        var playerData = Manager.Data.PlayerCharacters[ID];
        var equipment = playerData.Equipment;

        if (equipment.Helmet.HasValue)
        {
            equipmentBonus += equipment.Helmet.Value.Power;
        }
        if (equipment.Armor.HasValue)
        {
            equipmentBonus += equipment.Armor.Value.Power;
        }
        if (equipment.Shoes.HasValue)
        {
            equipmentBonus += equipment.Shoes.Value.Power;
        }

        return baseArmor + equipmentBonus;
    }

    // 장비된 아이템 가져오기
    public static Equipment GetCurrentEquipment()
    {
        var playerData = Manager.Data.PlayerCharacters[ID];
        return playerData.Equipment;
    }

    // PlayerCharacters 딕셔너리 업데이트 및 JSON 저장
    private static void UpdatePlayerDataInManager()
    {
        var playerData = Manager.Data.PlayerCharacters[ID];
        playerData.Status = _instance._status;
        Manager.Data.PlayerCharacters[ID] = playerData;
        Manager.Data.SaveData();
    }
    #endregion
}
