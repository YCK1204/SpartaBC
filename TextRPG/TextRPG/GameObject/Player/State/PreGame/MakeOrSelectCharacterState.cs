using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextRPG.Managers;
using TextRPG.Utils;
using static TextRPG.UI.UIManager;

public class MakeOrSelectCharacterState : IPlayerState<MakeOrSelectCharacterState, MakeOrSelectCharacterState.SubState>
{
    public static MakeOrSelectCharacterState Instance { get; } = new MakeOrSelectCharacterState();
    public enum SubState
    {
        MakeOrSelect,
        SelectCharacterJob,
        CharacterSummary,
        SetCharacterName,
        ReAskSetCharacterName,
        FailedSetCharacterName,
        OnSetCharacterName,
        SelectCharacter,
        FailedSelectCharacterByEmptyList,
        FailedOutOfRange,
    }
    public StateStack<SubState> StateStack { get; set; } = new StateStack<SubState>();
    public string InputStr { get; set; }
    public int InputInt { get; set; }
    public ConsoleKey InputKey { get; set; }

    CharacterStatus InputStatus = new CharacterStatus();

    public void Enter()
    {
        Manager.Data.LoadData();
    }

    public void Exit()
    {
        StateStack.Clear();
    }

    public void Update()
    {
        SubState State = StateStack.Pop();
        //FailedSelectCharacterByEmptyList,
        switch (State)
        {
            case SubState.MakeOrSelect:
                Manager.UI.MakeCharacter.MakeOrSelectCharacterText.DisplayMessage();
                InputKey = Manager.UI.GetInputAsKeyInfo();
                StateStack.Push(State);
                switch (InputKey)
                {
                    case ConsoleKey.D0:
                    case ConsoleKey.Escape:
                        Player.ChangeState<ExitState, ExitState.SubState>();
                        break;
                    case ConsoleKey.D1:
                        if (Manager.Data.PlayerCharacters.Count >= 9)
                            StateStack.Push(SubState.FailedSetCharacterName);
                        else
                            StateStack.Push(SubState.SelectCharacterJob);
                        break;
                    case ConsoleKey.D2:
                        StateStack.Push(SubState.SelectCharacter);
                        break;
                    default:
                        StateStack.Push(SubState.FailedOutOfRange);
                        break;
                }
                break;
            case SubState.SelectCharacterJob:
                Manager.UI.MakeCharacter.SelectCharacterJobsText.DisplayMessage();
                InputKey = Manager.UI.GetInputAsKeyInfo();
                StateStack.Push(State);
                switch (InputKey)
                {
                    case ConsoleKey.D0:
                    case ConsoleKey.Escape:
                        StateStack.Pop();
                        return;
                    default:
                        InputInt = InputKey - ConsoleKey.D0;
                        if (0 <= InputInt && InputInt <= 9)
                            StateStack.Push(SubState.CharacterSummary);
                        else
                            StateStack.Push(SubState.FailedOutOfRange);
                        break;
                }
                break;
            case SubState.CharacterSummary:
                InputStatus = Manager.Data.CharacterStatuses[InputInt];
                Manager.UI.MakeCharacter.CharacterSummaryText.FormatMessage(originalMsg => { return string.Format(originalMsg, InputStatus.CharacterSummary, InputStatus.JobName); });
                Manager.UI.MakeCharacter.CharacterSummaryText.DisplayMessage();
                InputKey = Manager.UI.GetInputAsKeyInfo();
                StateStack.Push(State);
                switch (InputKey)
                {
                    case ConsoleKey.D1:
                        StateStack.Push(SubState.SetCharacterName);
                        break;
                    case ConsoleKey.D2:
                    case ConsoleKey.Escape:
                        StateStack.Pop();
                        break;
                    default:
                        StateStack.Push(SubState.FailedOutOfRange);
                        break;
                }
                break;
            case SubState.SetCharacterName:
                Manager.UI.MakeCharacter.SetCharacterNameText.DisplayMessage();
                InputStr = Manager.UI.GetInput();
                if (2 <= InputStr.Length && InputStr.Length <= 6)
                {
                    StateStack.Push(SubState.ReAskSetCharacterName);
                }
                else
                {
                    StateStack.Push(State);
                    StateStack.Push(SubState.FailedSetCharacterName);
                }
                break;
            case SubState.ReAskSetCharacterName:
                Manager.UI.MakeCharacter.ReAskSetCharacterNameText.DisplayMessage();
                InputKey = Manager.UI.GetInputAsKeyInfo();
                StateStack.Push(State);
                switch (InputKey)
                {
                    case ConsoleKey.D1:
                        StateStack.Push(SubState.OnSetCharacterName);
                        break;
                    case ConsoleKey.D2:
                    case ConsoleKey.Escape:
                        StateStack.Pop();
                        break;
                    default:
                        StateStack.Push(SubState.FailedOutOfRange);
                        break;
                }
                break;
            case SubState.OnSetCharacterName:
                InputStatus.DisplayName = InputStr;
                Player.Status = InputStatus;
                Player.ID = Manager.Data.PlayerCharacters.Count + 1;
                Manager.Data.AddPlayerCharacterAndSave(InputStatus);
                Manager.UI.MakeCharacter.OnSetCharacterNameText.FormatMessage(originalMsg => { return string.Format(originalMsg, InputStatus.JobName, InputStr); });
                Manager.UI.MakeCharacter.OnSetCharacterNameText.DisplayMessage();
                Player.OnSet();
                Player.ChangeState<IdleState, IdleState.SubState>();
                Player.Position = new Position(15, 9);
                break;
            case SubState.SelectCharacter:
                if (Manager.Data.PlayerCharacters.Count == 0)
                {
                    StateStack.Push(SubState.FailedSelectCharacterByEmptyList);
                    return;
                }
                Manager.UI.SelectCharacter.SelectCharacterText.DisplayMessage();
                InputKey = Manager.UI.GetInputAsKeyInfo();
                StateStack.Push(State);
                var list = Manager.Data.PlayerCharacters.ToList();
                switch (InputKey)
                {
                    case ConsoleKey.D0:
                    case ConsoleKey.Escape:
                        StateStack.Pop();
                        return;
                    default:
                        InputInt = InputKey - ConsoleKey.D0;
                        if (InputInt < 0 || InputInt > list.Count)
                        {
                            StateStack.Push(SubState.FailedOutOfRange);
                            return;
                        }
                        var c = list[InputInt - 1];
                        Player.ID = c.Value.CharacterId;
                        Player.Status = c.Value.Status;
                        Player.OnSet();
                        Player.ChangeState<IdleState, IdleState.SubState>();
                        Player.Position = new Position(15, 9);
                        break;
                }
                break;
            case SubState.FailedSelectCharacterByEmptyList:
                Manager.UI.SelectCharacter.FailedSelectCharacterByEmptyListText.DisplayMessage();
                break;
            case SubState.FailedOutOfRange:
                Manager.UI.ETC.FailedOutOfRangeText.DisplayMessage();
                break;
        }
    }
}
