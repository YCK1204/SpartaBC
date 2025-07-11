using TextRPG.Utils;
using TextRPG.Managers;

public class Monster : GameObject
{
    private static int _nextId = 1;

    public int ID { get; private set; }
    public string MonsterKey { get; private set; }

    // 몬스터 기본 정보 (Monster.json에서 로드)
    public string Name { get; set; }
    public string NameHide { get; set; }
    public int HP { get; set; }
    public int MaxHP { get; set; }
    public int Power { get; set; }
    public int Armor { get; set; }
    public int Experience { get; set; }
    public int Level { get; set; }
    public int Gold { get; set; }
    public List<string> AttackMsgs { get; set; }
    public string AsciiPath { get; set; }

    public Monster(Position position, string monsterKey = "Dooly")
    {
        base.Pos = position;
        ID = _nextId++;
        MonsterKey = monsterKey;
        LoadMonsterData();
    }

    public Monster(int x, int y, string monsterKey = "Dooly")
    {
        base.Pos = new Position(x, y);
        ID = _nextId++;
        MonsterKey = monsterKey;
        LoadMonsterData();
    }
    public void Reset()
    {
        HP = MaxHP;
    }
    // Monster.json에서 데이터 로드
    private void LoadMonsterData()
    {
        var monsterData = Manager.Data.GetMonsterData(MonsterKey);
        
        if (monsterData.HasValue)
        {
            var data = monsterData.Value;
            Name = data.Name;
            NameHide = data.NameHide;
            Level = data.Level;
            MaxHP = data.MaxHP;
            HP = data.HP;
            Power = data.Power;
            Armor = data.Armor;
            Experience = data.Experience;
            Gold = data.Gold;
            AttackMsgs = data.AttackMsgs ?? new List<string>();
            AsciiPath = data.AsciiPath;
            
        }
        else
        {
            // 기본값 설정 (데이터 로드 실패 시)
            Name = $"Unknown Monster {ID}";
            Level = 1;
            MaxHP = 50;
            HP = MaxHP;
            Power = 10;
            Armor = 2;
            Experience = 25;
            Gold = 50;
            AttackMsgs = new List<string> { "공격!" };
            AsciiPath = "";
            
        }
    }

    // 몬스터 상태 체크
    public bool IsAlive => HP > 0;

    // 데미지 받기
    public void TakeDamage(int damage)
    {
        int actualDamage = Math.Max(1, damage - Armor);
        HP = Math.Max(0, HP - actualDamage);
    }

    // 몬스터 정보 출력
    public override string ToString()
    {
        return $"{Name} (ID: {ID}, Lv.{Level}, HP: {HP}/{MaxHP})";
    }
}
