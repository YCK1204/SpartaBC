using TextRPG.Managers;
using TextRPG.UI;
using TextRPG.Utils;
public class BattleState : IPlayerState<BattleState, BattleState.SubState>
{
    public static BattleState Instance { get; } = new BattleState();
    public StateStack<SubState> StateStack { get; set; } = new StateStack<SubState>();
    public string InputStr { get; set; }
    public int InputInt { get; set; }
    PortalInfo NextPortal { get; set; } = null;
    public ConsoleKey InputKey { get; set; }
    Monster Monster { get { return Player.Monster; } }
    Random random = new Random();

    public void Enter()
    {
        StateStack.Push(SubState.StartBattleText);
    }
    public void Exit()
    {
        StateStack.Clear();
    }
    public enum SubState
    {
        StartBattleText,
        PlayerTurnText,
        ReAskRunBattleText,
        PlayerAttackText,
        MonsterAttackText,
        OnMonsterAttackText,
        OnPlayerDeadText,
        OnMonsterDeadText,
        FailedOutOfRangeText,
        OnLevelUpText,
    }
    bool _isDefense = false;
    public void Update()
    {
        SubState State = StateStack.Pop();

        switch (State)
        {
            case SubState.StartBattleText:
                Manager.UI.Battle.StartBattleText.FormatMessage(originalMsg => { return string.Format(originalMsg, Monster.Name); });
                Manager.UI.Battle.StartBattleText.DisplayMessage();
                StateStack.Push(SubState.PlayerTurnText);
                break;
            case SubState.PlayerTurnText:
                Manager.UI.Battle.PlayerTurnText.DisplayMessage();
                InputKey = Manager.UI.GetInputAsKeyInfo();

                switch (InputKey)
                {
                    case ConsoleKey.D0:
                        StateStack.Push(State);
                        StateStack.Push(SubState.ReAskRunBattleText);
                        break;
                    case ConsoleKey.D1:
                        StateStack.Push(SubState.PlayerAttackText);
                        break;
                    case ConsoleKey.D2:
                        _isDefense = true;
                        StateStack.Push(SubState.MonsterAttackText);
                        break;
                    default:
                        StateStack.Push(State);
                        StateStack.Push(SubState.FailedOutOfRangeText);
                        break;
                }
                break;
            case SubState.ReAskRunBattleText:
                Manager.UI.Battle.ReAskRunBattleText.DisplayMessage();
                InputKey = Manager.UI.GetInputAsKeyInfo();
                switch (InputKey)
                {
                    case ConsoleKey.D1:
                        Player.Gold = 0;
                        Player.ChangeState<IdleState, IdleState.SubState>();
                        break;
                    case ConsoleKey.D2:
                        break;
                    default:
                        StateStack.Push(State);
                        StateStack.Push(SubState.FailedOutOfRangeText);
                        break;
                }
                break;
            case SubState.PlayerAttackText:
                int attackPower = Player.GetTotalAttackPower();
                int ranInt = random.Next(attackPower - 30, attackPower + 30 + Player.Level);
                int dmg = Math.Max(0, ranInt - Monster.Armor);
                Manager.UI.Battle.PlayerAttackText.FormatMessage(originalMsg => { return string.Format(originalMsg, Monster.Name, dmg); });
                Manager.UI.Battle.PlayerAttackText.DisplayMessage();
                Monster.HP -= dmg;
                if (Monster.HP <= 0)
                {
                    Monster.HP = 0;
                    StateStack.Push(SubState.OnMonsterDeadText);
                }
                else
                {
                    StateStack.Push(SubState.MonsterAttackText);
                }
                break;
            case SubState.MonsterAttackText:
                int armorPower = Player.GetTotalArmor();
                ranInt = random.Next(0, Monster.AttackMsgs.Count - 1);
                ranInt = _isDefense ? ranInt / 2 : ranInt;
                dmg = Math.Max(0, ranInt - armorPower);
                _isDefense = false;
                Manager.UI.Battle.MonsterAttackText.FormatMessage(originalMsg => { return string.Format(originalMsg, Monster.Name, Monster.AttackMsgs[ranInt]); });
                Manager.UI.Battle.MonsterAttackText.DisplayMessage();
                StateStack.Push(SubState.OnMonsterAttackText);
                break;
            case SubState.OnMonsterAttackText:
                ranInt = random.Next(Monster.Power - 30, Monster.Power + 30 + Monster.Level);
                dmg = Math.Max(0, ranInt - Player.Armor);
                Manager.UI.Battle.OnMonsterAttackText.FormatMessage(originalMsg => { return string.Format(originalMsg, Monster.Name, dmg); });
                Manager.UI.Battle.OnMonsterAttackText.DisplayMessage();
                Player.HP -= ranInt;
                if (Player.HP <= 0)
                {
                    Player.HP = 0;
                    StateStack.Push(SubState.OnPlayerDeadText);
                }
                else
                {
                    StateStack.Push(SubState.PlayerTurnText);
                }
                break;
            case SubState.OnPlayerDeadText:
                Manager.UI.Battle.OnPlayerDeadText.FormatMessage(originalMsg =>
                {
                    return string.Format(originalMsg, Monster.Name);
                });
                Manager.UI.Battle.OnPlayerDeadText.DisplayMessage();
                Manager.Data.PlayerCharacters.Remove(Player.ID);
                Manager.Data.SaveData();
                Player.ChangeState<IntroState, IntroState.SubState>();
                break;
            case SubState.OnMonsterDeadText:
                int gold = random.Next(Monster.Gold, Monster.Gold * 2);
                int experience = random.Next(Monster.Experience, (int)(Monster.Experience * 1.5f));
                Manager.UI.Battle.OnMonsterDeadText.FormatMessage(originalMsg => 
                { 
                    return string.Format(originalMsg, 
                        Monster.Name,
                        gold,
                        experience); 
                });
                Manager.UI.Battle.OnMonsterDeadText.DisplayMessage();
                Player.Gold += random.Next(Monster.Gold, Monster.Gold * 2);
                Player.Experience += random.Next(Monster.Experience, (int)(Monster.Experience * 1.5f));
                if (Player.LevelUp())
                {
                    Manager.UI.Battle.OnLevelUpText.FormatMessage(origianlMsg => { return string.Format(origianlMsg, Player.Level); });
                    Manager.UI.Battle.OnLevelUpText.DisplayMessage();
                }    
                Player.ChangeState<IdleState, IdleState.SubState>();
                break;
            case SubState.FailedOutOfRangeText:
                Manager.UI.ETC.FailedOutOfRangeText.DisplayMessage();
                break;
            case SubState.OnLevelUpText:
                break;
        }
    }
}
