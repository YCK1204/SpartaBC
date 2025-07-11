using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextRPG.Managers;
using TextRPG.Utils;

public class IntroState : IPlayerState<IntroState, IntroState.SubState>
{
    public static IntroState Instance { get; } = new IntroState();
    public StateStack<SubState> StateStack { get; set; } = new StateStack<SubState>();
    public string InputStr { get; set; }
    public int InputInt { get; set; }
    public ConsoleKey InputKey { get; set; }

    public enum SubState
    {
        GameStart,
        ExplainWorld,
    }
    public void Enter()
    {
        Manager.Init();
    }
    public void Exit()
    {
    }
    public void Update()
    {
        var State = StateStack.Pop();
        switch (State)
        {
            case SubState.GameStart:
                Manager.UI.Intro.GameStartText.DisplayMessage();
                StateStack.Push(SubState.ExplainWorld);
                break;
            case SubState.ExplainWorld:
                Manager.UI.Intro.IntroText.DisplayMessage();
                Player.ChangeState<MakeOrSelectCharacterState, MakeOrSelectCharacterState.SubState>();
                break;
        }
    }
}
