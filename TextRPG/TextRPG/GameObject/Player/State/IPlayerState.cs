using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class StateStack<_Type_S>
{
    Stack<_Type_S> _stateStack = new Stack<_Type_S>();
    public void Push(_Type_S state)
    {
        _stateStack.Push(state);
    }
    public _Type_S Pop()
    {
        if (_stateStack.Count > 0)
        {
            _Type_S state = _stateStack.Peek();
            _stateStack.Pop();
            return state;
        }
        return default(_Type_S);
    }
    public _Type_S Top()
    {
        if (_stateStack.Count > 0)
        {
            return _stateStack.Peek();
        }
        return default(_Type_S);
    }
    public void Clear()
    {
        _stateStack.Clear();
    }
}
public interface IPlayerStateBase
{
    void Enter();
    void Update();
    void Exit();
}
public interface IPlayerState<_Type_Ins, _Type_S> : IPlayerStateBase
{
    static abstract _Type_Ins Instance { get; }
    StateStack<_Type_S> StateStack { get; set; }
    string InputStr { get; set; }
    int InputInt { get; set; }
    ConsoleKey InputKey { get; set; }
}
