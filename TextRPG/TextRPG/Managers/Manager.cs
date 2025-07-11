using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextRPG.UI;

namespace TextRPG.Managers
{
    public class Manager
    {
        static Manager _instance = new Manager();

        UIManager _ui = new UIManager();
        public static UIManager UI { get { return _instance._ui; } }
        DataManager _data = new DataManager();
        public static DataManager Data { get { return _instance._data; } }
        GameManager _game = new GameManager();
        public static GameManager Game { get { return _instance._game; } }
        MapManager _map = new MapManager();
        public static MapManager Map { get { return _instance._map; } }
        public static void Init()
        {
            Data.LoadData();
            UI.Init();
            Game.Init();
            Map.Init();
        }
    }
}