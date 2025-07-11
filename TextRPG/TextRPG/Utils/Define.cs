using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextRPG.Utils
{
    public enum CharacterType
    {
        SwordMaster,
        MagicSwordMaster,
    }
    public enum ItemType
    {
        Helmet,
        Armor,
        Shoes,
        Sword,
        Potion,
    }
    public enum PlayerState
    {
        INTRO,
        MAKE_OR_SELECT_CHARACTER,
        IDLE,
        MOVE,
        BATTLE,
        EXIT
    }
    public enum UIMsgType
    {
        NONE,
        MSG_WITH_WAIT,
        PROMPT_FOR_INPUT,
    }
    public struct Position
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Position(int x, int y) { X = x; Y = y; }
        public void AddX(int x) { X += x; }
        public void AddY(int y) { Y += y; }
        public void Add(Position pos) { X += pos.X; Y += pos.Y; }
        public void Add(int x, int y) { X += x; Y += y; }
        public static Position operator +(Position a, Position b) { return new Position(a.X + b.X, a.Y + b.Y); }
        public static Position operator -(Position a, Position b) { return new Position(a.X - b.X, a.Y - b.Y); }
        public static Position up { get { return new Position(0, -1); } }
        public static Position down { get { return new Position(0, 1); } }
        public static Position left { get { return new Position(-1, 0); } }
        public static Position right { get { return new Position(1, 0); } }
    }
}
