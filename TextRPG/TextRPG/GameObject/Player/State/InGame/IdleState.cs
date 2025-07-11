using TextRPG.Managers;
using TextRPG.Utils;
public class IdleState : IPlayerState<IdleState, IdleState.SubState>
{
    public static IdleState Instance { get; } = new IdleState();
    public StateStack<SubState> StateStack { get; set; } = new StateStack<SubState>();
    public string InputStr { get; set; }
    public int InputInt { get; set; }
    PortalInfo NextPortal { get; set; } = null;
    public ConsoleKey InputKey { get; set; }

    public void Enter()
    {
        StateStack.Push(SubState.None);
    }
    public void Exit()
    {
        StateStack.Clear();
    }
    public enum SubState
    {
        None,
        AskPortal,
        AskShop,
        AskBackToCharacterSelect,
    }
    void HandleInput()
    {
        InputKey = Manager.UI.GetInputAsKeyInfo();
        MapTile tile = null;

        switch (InputKey)
        {
            case ConsoleKey.LeftArrow:
                tile = Manager.Map.GetTileAt(Player.Position + Position.left);
                break;
            case ConsoleKey.RightArrow:
                tile = Manager.Map.GetTileAt(Player.Position + Position.right);
                break;
            case ConsoleKey.DownArrow:
                tile = Manager.Map.GetTileAt(Player.Position + Position.down);
                break;
            case ConsoleKey.UpArrow:
                tile = Manager.Map.GetTileAt(Player.Position + Position.up);
                break;
            case ConsoleKey.I:
                Player.ChangeState<InventoryState, InventoryState.SubState>();
                break;
            case ConsoleKey.H:
                Player.ChangeState<TutorialState, TutorialState.SubState>();
                break;
            case ConsoleKey.Escape:
                StateStack.Push(SubState.AskBackToCharacterSelect);
                break;
        }

        if (tile != null)
        {
            switch (tile.Type)
            {
                case MapTileType.Ground:
                    // 일반 지면 - 이동 가능
                    Player.Position = new Position(tile.X, tile.Y);
                    break;
                case MapTileType.Shop:
                    Player.ChangeState<ShopState, ShopState.SubState>();
                    break;
                case MapTileType.Portal:
                    (string toMap, int toId, int spawnX, int spawnY) = tile.GetPortalInfo();
                    NextPortal = new PortalInfo
                    {
                        to_map = toMap,
                        to_id = toId,
                        spawn_pos = new int[2] { spawnX, spawnY }
                    };
                    StateStack.Push(SubState.AskPortal);
                    break;
                case MapTileType.Monster:
                    // 현재 위치의 몬스터 정보 가져오기
                    var currentTile = Manager.Map.CurrentMap.GetTile(tile.X, tile.Y);
                    if (currentTile != null && currentTile.Monster != null)
                    {
                        // 타일에 있는 Monster 객체를 Player.Monster에 설정
                        Player.Monster = currentTile.Monster;
                        Player.ChangeState<BattleState, BattleState.SubState>();
                    }
                    break;
            }
        }
    }
    public void Update()
    {
        SubState State = StateStack.Pop();

        switch (State)
        {
            case SubState.None:
                StateStack.Push(State);
                HandleInput();
                break;
            case SubState.AskPortal:
                // 포탈 목적지 이름으로 메시지 포맷팅
                Manager.UI.Idle.AskPortalText.FormatMessage(originalMsg =>
                {
                    return string.Format(originalMsg, NextPortal.to_map);
                });
                Manager.UI.Idle.AskPortalText.DisplayMessage();
                InputKey = Manager.UI.GetInputAsKeyInfo();
                switch (InputKey)
                {
                    case ConsoleKey.D1:
                        // 연결된 맵으로 이동
                        UsePortal();
                        break;
                    case ConsoleKey.D2:
                    case ConsoleKey.Escape:
                        // 포탈 선택 취소
                        NextPortal = null;
                        break;
                    default:
                        StateStack.Push(State);
                        Manager.UI.ETC.FailedOutOfRangeText.DisplayMessage();
                        break;
                }
                break;
            case SubState.AskShop:
                Manager.UI.Idle.AskPortalText.DisplayMessage();
                InputKey = Manager.UI.GetInputAsKeyInfo();
                switch (InputKey)
                {
                    case ConsoleKey.D1:
                        Player.ChangeState<ShopState, ShopState.SubState>();
                        break;
                    case ConsoleKey.D2:
                    case ConsoleKey.Escape:
                        break;
                    default:
                        StateStack.Push(State);
                        Manager.UI.ETC.FailedOutOfRangeText.DisplayMessage();
                        break;
                }
                break;
            case SubState.AskBackToCharacterSelect:
                Manager.UI.Idle.AskBackToCharacterSelect.DisplayMessage();
                InputKey = Manager.UI.GetInputAsKeyInfo();
                switch (InputKey)
                {
                    case ConsoleKey.D1:
                        Player.ChangeState<MakeOrSelectCharacterState, MakeOrSelectCharacterState.SubState>();
                        break;
                    case ConsoleKey.D2:
                    case ConsoleKey.Escape:
                        // 현재 상태 유지
                        break;
                    default:
                        StateStack.Push(State);
                        Manager.UI.ETC.FailedOutOfRangeText.DisplayMessage();
                        break;
                }
                break;
        }
    }

    private void UsePortal()
    {
        if (NextPortal != null)
        {

            // 목적지 맵으로 변경
            bool mapLoaded = Manager.Map.LoadMap(NextPortal.to_map);
            if (!mapLoaded)
            {
                NextPortal = null;
                return;
            }

            // 목적지 맵에서 연결된 포탈의 spawn 위치 가져오기
            var (spawnX, spawnY) = Manager.Map.GetDestinationPortalSpawnPosition(NextPortal.to_map, NextPortal.to_id);


            if (spawnX != -1 && spawnY != -1)
            {
                // 플레이어 위치를 목적지 포탈의 spawn 위치로 설정
                Player.SetPosition(spawnX, spawnY);
            }
            else
            {
            }

            // 포탈 정보 초기화
            NextPortal = null;

            // UI 갱신을 위해 메인 화면 다시 그리기
            Manager.UI.ShowMainScreen();
        }
    }
}
