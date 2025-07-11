using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextRPG.Managers;
using TextRPG.UI;
using TextRPG.Utils;

public class TutorialState : IPlayerState<TutorialState, TutorialState.SubState>
{
    public static TutorialState Instance { get; } = new TutorialState();
    public StateStack<SubState> StateStack { get; set; } = new StateStack<SubState>();
    public string InputStr { get; set; }
    public int InputInt { get; set; }
    public ConsoleKey InputKey { get; set; }

    public enum SubState
    {
        AskShowTutorial,
        ShowTutorial1,
        ShowTutorial2,
        ShowTutorial3,
        ShowTutorial4,
        EndTutorial,
        FailedOutOfRange,
    }

    public void Enter()
    {
        StateStack.Push(SubState.AskShowTutorial);
    }

    public void Exit()
    {
        StateStack.Clear();
    }

    public void Update()
    {
        SubState State = StateStack.Pop();
        switch (State)
        {
            case SubState.AskShowTutorial:
                Manager.UI.Tutorial.AskShowTutorialText.DisplayMessage();
                InputKey = Manager.UI.GetInputAsKeyInfo();
                switch (InputKey)
                {
                    case ConsoleKey.D1:
                        StateStack.Push(SubState.ShowTutorial1);
                        break;
                    case ConsoleKey.D2:
                    case ConsoleKey.Escape:
                        Player.ChangeState<IdleState, IdleState.SubState>();
                        break;
                    default:
                        StateStack.Push(State);
                        StateStack.Push(SubState.FailedOutOfRange);
                        break;
                }
                break;
            case SubState.ShowTutorial1:
                Manager.UI.Tutorial.Tutorial1Text.DisplayMessage();
                StateStack.Push(SubState.ShowTutorial2);
                break;
            case SubState.ShowTutorial2:
                Manager.UI.Tutorial.Tutorial2Text.DisplayMessage();
                StateStack.Push(SubState.ShowTutorial3);
                break;
            case SubState.ShowTutorial3:
                Manager.UI.Tutorial.Tutorial3Text.DisplayMessage();
                StateStack.Push(SubState.ShowTutorial4);
                break;
            case SubState.ShowTutorial4:
                Manager.UI.Tutorial.Tutorial4Text.DisplayMessage();
                StateStack.Push(SubState.EndTutorial);
                break;
            case SubState.EndTutorial:
                Manager.UI.Tutorial.EndTutorialText.DisplayMessage();
                Player.ChangeState<IdleState, IdleState.SubState>();
                break;
            case SubState.FailedOutOfRange:
                Manager.UI.ETC.FailedOutOfRangeText.DisplayMessage();
                break;
        }
    }
}
