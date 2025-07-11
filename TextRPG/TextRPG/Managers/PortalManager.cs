using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextRPG.Utils;

namespace TextRPG.Managers
{
    public class PortalManager
    {
        // 플레이어가 포탈을 이용할 때 호출
        public static bool UsePortal(int playerX, int playerY)
        {
            var currentMap = Manager.Map.CurrentMap;
            if (currentMap == null) return false;

            var tile = currentMap.GetTile(playerX, playerY);
            if (tile?.Type != MapTileType.Portal) return false;

            var portalInfo = tile.GetPortalInfo();
            if (portalInfo.toMap == null) return false;

            // 목적지 맵으로 이동
            bool success = Manager.Map.SetCurrentMap(portalInfo.toMap);
            if (success)
            {
                // 플레이어 위치를 spawn_pos로 설정
                // TODO: 플레이어 위치 업데이트 (Player 클래스에서 처리)
                return true;
            }

            return false;
        }

        // 포탈 정보 가져오기
        public static (string toMap, int toId, int spawnX, int spawnY) GetPortalInfo(int x, int y)
        {
            var currentMap = Manager.Map.CurrentMap;
            if (currentMap == null) return (null, -1, -1, -1);

            var tile = currentMap.GetTile(x, y);
            if (tile?.Type != MapTileType.Portal) return (null, -1, -1, -1);

            return tile.GetPortalInfo();
        }

        // 현재 맵의 모든 포탈 정보 가져오기
        public static List<(int x, int y, string toMap)> GetAllPortalsInCurrentMap()
        {
            var portals = new List<(int x, int y, string toMap)>();
            var currentMap = Manager.Map.CurrentMap;
            if (currentMap == null) return portals;

            foreach (var portal in currentMap.PortalTiles)
            {
                var info = portal.GetPortalInfo();
                if (info.toMap != null)
                {
                    portals.Add((portal.X, portal.Y, info.toMap));
                }
            }

            return portals;
        }
    }
}