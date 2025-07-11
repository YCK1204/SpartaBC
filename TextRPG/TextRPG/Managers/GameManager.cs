using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextRPG.Utils;

namespace TextRPG.Managers
{
    public class GameManager
    {
        Dictionary<PlayerState, Action> _stateActions = new Dictionary<PlayerState, Action>();
        public void Init()
        {

        }
        public void Start()
        {
            while (Player.State != ExitState.Instance)
            {
                Manager.UI.ShowMainScreen();
                Player.Update();
            }
        }
    }
}
