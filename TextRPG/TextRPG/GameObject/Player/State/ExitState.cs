using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class ExitState : IPlayerState<ExitState, ExitState.SubState>
{
    public static ExitState Instance { get; } = new ExitState();
    public StateStack<SubState> StateStack { get; set; } = new StateStack<SubState>();
    public string InputStr { get; set; }
    public int InputInt { get; set; }
    public ConsoleKey InputKey { get; set; }

    public enum SubState
    {

    }
    public void Enter()
    {
    }

    public void Exit()
    {
    }

    public void Update()
    {
    }
}
