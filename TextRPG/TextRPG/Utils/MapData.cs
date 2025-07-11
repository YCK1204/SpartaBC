using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextRPG.Utils
{
    public enum MapTileType
    {
        Wall = '#',      // 벽 - 이동 불가
        Ground = '.',    // 일반 지면 - 이동 가능
        Shop = 'S',      // 상점
        Portal = '_',    // 포탈
        Monster = 'M',   // 몬스터
        Player = 'P'     // 플레이어 위치 (런타임에만 사용)
    }

    public class MapTile
    {
        public MapTileType Type { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public bool IsWalkable { get; set; }
        public Dictionary<string, object> Properties { get; set; }

        public MapTile(int x, int y, MapTileType type)
        {
            X = x;
            Y = y;
            Type = type;
            Properties = new Dictionary<string, object>();
            
            // 타일 타입에 따른 이동 가능 여부 설정
            IsWalkable = type switch
            {
                MapTileType.Wall => false,
                MapTileType.Ground => true,
                MapTileType.Shop => true,
                MapTileType.Portal => true,
                MapTileType.Monster => true,
                MapTileType.Player => true,
                _ => false
            };
        }

        // 포탈 정보 설정
        public void SetPortalInfo(string toMap, int toId, int spawnX, int spawnY)
        {
            if (Type == MapTileType.Portal)
            {
                Properties["to_map"] = toMap;
                Properties["to_id"] = toId;
                Properties["spawn_x"] = spawnX;
                Properties["spawn_y"] = spawnY;
            }
        }

        // 몬스터 정보 설정
        public void SetMonsterInfo(int monsterId)
        {
            if (Type == MapTileType.Monster)
            {
                Properties["monster_id"] = monsterId;
            }
        }

        // 포탈 정보 가져오기
        public (string toMap, int toId, int spawnX, int spawnY) GetPortalInfo()
        {
            if (Type == MapTileType.Portal && Properties.ContainsKey("to_map"))
            {
                return (
                    Properties["to_map"].ToString(),
                    (int)Properties["to_id"],
                    (int)Properties["spawn_x"],
                    (int)Properties["spawn_y"]
                );
            }
            return (null, -1, -1, -1);
        }

        // 몬스터 정보 가져오기
        public int GetMonsterId()
        {
            if (Type == MapTileType.Monster && Properties.ContainsKey("monster_id"))
            {
                return (int)Properties["monster_id"];
            }
            return -1;
        }
    }

    public class MapData
    {
        public string MapName { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public MapTile[,] Tiles { get; set; }
        public List<MapTile> ShopTiles { get; set; }
        public List<MapTile> PortalTiles { get; set; }
        public List<MapTile> MonsterTiles { get; set; }
        public MapTile PlayerSpawnPoint { get; set; }

        public MapData(string mapName, int width, int height)
        {
            MapName = mapName;
            Width = width;
            Height = height;
            Tiles = new MapTile[width, height];
            ShopTiles = new List<MapTile>();
            PortalTiles = new List<MapTile>();
            MonsterTiles = new List<MapTile>();
        }

        public bool IsValidPosition(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        public bool IsWalkable(Position pos)
        {
            if (!IsValidPosition(pos.X, pos.Y)) return false;
            return Tiles[pos.X, pos.Y]?.IsWalkable ?? false;
        }

        public MapTile GetTile(int x, int y)
        {
            if (!IsValidPosition(x, y)) return null;
            return Tiles[x, y];
        }

        public void SetTile(int x, int y, MapTile tile)
        {
            if (!IsValidPosition(x, y)) return;
            Tiles[x, y] = tile;

            // null이 아닌 타일에 대해서만 특수 타일 리스트에 추가
            if (tile != null)
            {
                switch (tile.Type)
                {
                    case MapTileType.Shop:
                        ShopTiles.Add(tile);
                        break;
                    case MapTileType.Portal:
                        // 포탈은 SetPortalInfo에서 JSON 배열 순서대로 추가하므로 여기서는 추가하지 않음
                        break;
                    case MapTileType.Monster:
                        MonsterTiles.Add(tile);
                        break;
                }
            }
        }
    }
}