using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextRPG.Utils;
using Newtonsoft.Json;

namespace TextRPG.Managers
{
    public class MapManager
    {
        private Dictionary<string, MapData> _loadedMaps;
        private MapData _currentMap;
        private string _mapDataPath;
        private MapInfoRoot _mapInfo;
        private Random _random;
        private Dictionary<int, Monster> _allMonsters; // 모든 몬스터 관리

        public MapData CurrentMap => _currentMap;

        public MapManager()
        {
            _loadedMaps = new Dictionary<string, MapData>();
            _mapDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Map");
            _random = new Random();
            _allMonsters = new Dictionary<int, Monster>();
        }

        public void Init()
        {
            LoadMapInfo();
            LoadAllMaps();
            SpawnMonstersInAllMaps();
            _currentMap = _loadedMaps["떡잎마을"];
        }
        #region 맵 로드 및 파싱
        private void LoadMapInfo()
        {
            try
            {
                string mapInfoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Portals.json");
                if (File.Exists(mapInfoPath))
                {
                    string jsonContent = File.ReadAllText(mapInfoPath, Encoding.UTF8);
                    _mapInfo = JsonConvert.DeserializeObject<MapInfoRoot>(jsonContent);
                }
                else
                {
                }
            }
            catch (Exception ex)
            {
            }
        }
        public void LoadAllMaps()
        {
            try
            {
                // Data/Map 디렉터리가 존재하는지 확인
                if (!Directory.Exists(_mapDataPath))
                {
                    return;
                }

                // 모든 .txt 파일 검색
                string[] mapFiles = Directory.GetFiles(_mapDataPath, "*.txt");

                if (mapFiles.Length == 0)
                {
                    return;
                }


                int loadedCount = 0;
                foreach (string mapFilePath in mapFiles)
                {
                    // 파일명에서 확장자 제거하여 맵 이름 추출
                    string mapName = Path.GetFileNameWithoutExtension(mapFilePath);

                    try
                    {
                        string[] lines = File.ReadAllLines(mapFilePath, Encoding.UTF8);
                        MapData mapData = ParseMapFromText(mapName, lines);

                        if (mapData != null)
                        {
                            // 포탈 정보 설정
                            SetPortalInfo(mapData, mapName);
                            _loadedMaps[mapName] = mapData;
                            loadedCount++;
                        }
                        else
                        {
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }

                foreach (var mapName in _loadedMaps.Keys)
                {
                }

                // 기본 맵 설정 (떡잎마을이 있으면 기본으로 설정)
                if (_loadedMaps.ContainsKey("떡잎마을"))
                {
                    _currentMap = _loadedMaps["떡잎마을"];
                }
                else if (_loadedMaps.ContainsKey("떡잎마을"))
                {
                    _currentMap = _loadedMaps["떡잎마을"];
                }
                else if (_loadedMaps.Count > 0)
                {
                    // 떡잎마을이 없으면 첫 번째 맵을 기본으로 설정
                    var firstMap = _loadedMaps.First();
                    _currentMap = firstMap.Value;
                }
            }
            catch (Exception ex)
            {
            }
        }

        public bool LoadMap(string mapName)
        {
            // 이미 로드된 맵이면 캐시에서 가져오기
            if (_loadedMaps.ContainsKey(mapName))
            {
                _currentMap = _loadedMaps[mapName];
                return true;
            }

            return false;
        }

        public bool SetCurrentMap(string mapName)
        {
            if (_loadedMaps.ContainsKey(mapName))
            {
                _currentMap = _loadedMaps[mapName];
                return true;
            }
            return false;
        }

        public List<string> GetLoadedMapNames()
        {
            return new List<string>(_loadedMaps.Keys);
        }

        // 목적지 맵에서 특정 포탈 ID의 spawn 위치 가져오기
        public (int spawnX, int spawnY) GetDestinationPortalSpawnPosition(string mapName, int portalId)
        {
            // 맵이 로드되지 않은 경우
            if (!_loadedMaps.ContainsKey(mapName))
            {
                return (-1, -1);
            }

            var map = _loadedMaps[mapName];
            var portals = map.PortalTiles;

            // to_id는 목적지 맵에서 해당하는 포탈의 배열 인덱스
            // 즉, 도라에몽의 쉼터1의 to_id 0은 도라에몽의 쉼터1 포탈 배열의 0번째 포탈
            if (portalId < 0 || portalId >= portals.Count)
            {
                return (-1, -1);
            }

            // 목적지 맵의 portalId번째 포탈의 spawn_pos를 가져옴
            var destinationPortal = portals[portalId];
            var (_, _, spawnX, spawnY) = destinationPortal.GetPortalInfo();
            
            return (spawnX, spawnY);
        }

        private MapData ParseMapFromText(string mapName, string[] lines)
        {
            if (lines == null || lines.Length == 0)
                return null;

            // 맵 크기 계산 (가장 긴 줄을 기준으로 너비 결정)
            int maxWidth = lines.Max(line => line.Length);
            int height = lines.Length;

            MapData mapData = new MapData(mapName, maxWidth, height);

            // 각 줄을 파싱하여 2차원 배열로 변환 (비정형 맵 지원)
            for (int y = 0; y < height; y++)
            {
                string line = lines[y];
                for (int x = 0; x < maxWidth; x++)
                {
                    char tileChar;

                    // 현재 줄이 x 좌표보다 짧으면 공백으로 처리
                    if (x < line.Length)
                    {
                        tileChar = line[x];
                    }
                    else
                    {
                        tileChar = ' '; // 줄이 짧은 경우 공백으로 채움
                    }

                    MapTileType tileType = ParseTileType(tileChar);

                    // 공백 영역은 null로 처리하여 맵 밖임을 표시
                    if (tileChar == ' ')
                    {
                        mapData.SetTile(x, y, null);
                    }
                    else
                    {
                        MapTile tile = new MapTile(x, y, tileType);
                        mapData.SetTile(x, y, tile);

                        // 플레이어 스폰 포인트 설정 (첫 번째 이동 가능한 지점)
                        if (mapData.PlayerSpawnPoint == null && tile.IsWalkable)
                        {
                            mapData.PlayerSpawnPoint = tile;
                        }
                    }
                }
            }

            return mapData;
        }

        private MapTileType ParseTileType(char tileChar)
        {
            return tileChar switch
            {
                '#' => MapTileType.Wall,
                '.' => MapTileType.Ground,
                'S' => MapTileType.Shop,
                '_' => MapTileType.Portal,
                'M' => MapTileType.Monster,
                'P' => MapTileType.Player,
                ' ' => MapTileType.Wall,
                // 숫자는 포탈로 처리 (0-9)
                >= '0' and <= '9' => MapTileType.Portal,
                _ => MapTileType.Ground
            };
        }
        private void SetPortalInfo(MapData mapData, string mapName)
        {
            if (_mapInfo?.portals == null || _mapInfo.portals.Length == 0)
                return;

            // JSON에서 해당 맵의 포탈 정보 찾기
            var mapPortalData = _mapInfo.portals[0]; // 첫 번째 요소가 맵 데이터
            if (!mapPortalData.ContainsKey(mapName))
                return;

            var portals = mapPortalData[mapName];

            // 기존 PortalTiles 리스트 클리어 (올바른 순서로 다시 추가하기 위해)
            mapData.PortalTiles.Clear();

            // JSON 배열 순서대로 포탈 정보 설정 및 PortalTiles에 추가
            for (int i = 0; i < portals.Length; i++)
            {
                var portalInfo = portals[i];
                int portalX = portalInfo.pos[0];
                int portalY = portalInfo.pos[1];

                var tile = mapData.GetTile(portalX, portalY);
                
                if (tile?.Type == MapTileType.Portal)
                {
                    // 포탈 정보 설정
                    tile.SetPortalInfo(
                        portalInfo.to_map,
                        portalInfo.to_id,
                        portalInfo.spawn_pos[0],
                        portalInfo.spawn_pos[1]
                    );

                    // JSON 배열 순서대로 PortalTiles에 추가
                    mapData.PortalTiles.Add(tile);
                    
                }
                else
                {
                }
            }
            
        }
        #endregion

        #region 몬스터 스폰 시스템
        // 모든 맵에 몬스터 스폰 (떡잎마을 제외)
        private void SpawnMonstersInAllMaps()
        {
            foreach (var kvp in _loadedMaps)
            {
                string mapName = kvp.Key;
                MapData mapData = kvp.Value;

                // 떡잎마을은 몬스터 스폰 제외
                if (mapName.Equals("떡잎마을", StringComparison.OrdinalIgnoreCase))
                    continue;

                SpawnMonstersInMap(mapData, mapName);
            }
        }

        // 특정 맵에 몬스터 스폰
        private void SpawnMonstersInMap(MapData mapData, string mapName)
        {
            // 맵별로 다른 몬스터 생성 규칙
            switch (mapName)
            {
                case "도라에몽의 쉼터1":
                    SpawnMonstersByType(mapData, new[] { "Doraemon" }, 4, 7);
                    break;
                case "둘리의 팥빵 제작소":
                    SpawnMonstersByType(mapData, new[] { "Doraemon", "Dooly" }, 3, 5);
                    break;
                case "고길동의 수련장":
                    SpawnFixedMonster(mapData, "GoGildong", 11, 2);
                    break;
                case "도라에몽의 쉼터2":
                    SpawnMonstersByType(mapData, new[] { "Doraemon" }, 5, 8);
                    break;
                case "도우너와 둘리의 은밀한 공간":
                    SpawnField005Monsters(mapData);
                    break;
                case "페페의 컴퓨터실":
                    SpawnMonstersByType(mapData, new[] { "PePe" }, 2, 3);
                    break;
                default:
                    // 기본값 (기존 방식)
                    SpawnMonstersByType(mapData, new[] { "Dooly" }, 2, 3);
                    break;
            }
        }

        // 특정 위치에 고정 몬스터 스폰
        private void SpawnFixedMonster(MapData mapData, string monsterType, int x, int y)
        {
            var tile = mapData.GetTile(x, y);
            if (tile != null && tile.Type == MapTileType.Ground)
            {
                Monster monster = new Monster(new Position(x, y), monsterType);
                
                // 전역 몬스터 딕셔너리에 추가
                _allMonsters[monster.ID] = monster;
                
                // 해당 위치를 몬스터 타일로 설정
                tile.Type = MapTileType.Monster;
                tile.SetMonsterInfo(monster.ID);
                tile.Monster = monster;
                mapData.MonsterTiles.Add(tile);
            }
        }

        // 특정 타입의 몬스터들을 랜덤 생성
        private void SpawnMonstersByType(MapData mapData, string[] monsterTypes, int minCount, int maxCount)
        {
            int monsterCount = _random.Next(minCount, maxCount + 1);
            
            for (int i = 0; i < monsterCount; i++)
            {
                Position? spawnPos = FindRandomGroundPosition(mapData);
                if (spawnPos.HasValue)
                {
                    // 몬스터 타입 랜덤 선택
                    string monsterType = monsterTypes[_random.Next(monsterTypes.Length)];
                    Monster monster = new Monster(spawnPos.Value, monsterType);
                    
                    // 전역 몬스터 딕셔너리에 추가
                    _allMonsters[monster.ID] = monster;
                    
                    // 해당 위치를 몬스터 타일로 설정
                    var tile = mapData.GetTile(spawnPos.Value.X, spawnPos.Value.Y);
                    if (tile != null)
                    {
                        tile.Type = MapTileType.Monster;
                        tile.SetMonsterInfo(monster.ID);
                        tile.Monster = monster;
                        mapData.MonsterTiles.Add(tile);
                    }
                }
            }
        }

        // 도우너와 둘리의 은밀한 공간 전용 몬스터 생성 (Dooly가 3마리 이상, Dooly가 더 많아야 함)
        private void SpawnField005Monsters(MapData mapData)
        {
            int totalCount = _random.Next(4, 7); // 4~6마리
            int dollyCount = Math.Max(3, totalCount - 2); // 최소 3마리, 전체에서 더 많은 비율
            int dounaCount = totalCount - dollyCount;
            
            // Dooly 스폰
            for (int i = 0; i < dollyCount; i++)
            {
                Position? spawnPos = FindRandomGroundPosition(mapData);
                if (spawnPos.HasValue)
                {
                    Monster monster = new Monster(spawnPos.Value, "Dooly");
                    
                    _allMonsters[monster.ID] = monster;
                    
                    var tile = mapData.GetTile(spawnPos.Value.X, spawnPos.Value.Y);
                    if (tile != null)
                    {
                        tile.Type = MapTileType.Monster;
                        tile.SetMonsterInfo(monster.ID);
                        tile.Monster = monster;
                        mapData.MonsterTiles.Add(tile);
                    }
                }
            }
            
            // Douna 스폰
            for (int i = 0; i < dounaCount; i++)
            {
                Position? spawnPos = FindRandomGroundPosition(mapData);
                if (spawnPos.HasValue)
                {
                    Monster monster = new Monster(spawnPos.Value, "Douna");
                    
                    _allMonsters[monster.ID] = monster;
                    
                    var tile = mapData.GetTile(spawnPos.Value.X, spawnPos.Value.Y);
                    if (tile != null)
                    {
                        tile.Type = MapTileType.Monster;
                        tile.SetMonsterInfo(monster.ID);
                        tile.Monster = monster;
                        mapData.MonsterTiles.Add(tile);
                    }
                }
            }
        }

        // 랜덤한 Ground 위치 찾기
        private Position? FindRandomGroundPosition(MapData mapData)
        {
            List<Position> groundPositions = new List<Position>();

            // 모든 Ground 타일 위치 수집
            for (int y = 0; y < mapData.Height; y++)
            {
                for (int x = 0; x < mapData.Width; x++)
                {
                    var tile = mapData.GetTile(x, y);
                    if (tile?.Type == MapTileType.Ground)
                    {
                        groundPositions.Add(new Position(x, y));
                    }
                }
            }

            // 랜덤 위치 선택
            if (groundPositions.Count > 0)
            {
                int randomIndex = _random.Next(groundPositions.Count);
                return groundPositions[randomIndex];
            }

            return null;
        }

        // 몬스터 ID로 몬스터 가져오기
        public Monster GetMonsterById(int monsterId)
        {
            return _allMonsters.ContainsKey(monsterId) ? _allMonsters[monsterId] : null;
        }

        // 특정 위치의 몬스터 가져오기
        public Monster GetMonsterAt(int x, int y)
        {
            var tile = _currentMap?.GetTile(x, y);
            if (tile?.Type == MapTileType.Monster)
            {
                int monsterId = tile.GetMonsterId();
                return GetMonsterById(monsterId);
            }
            return null;
        }

        // 몬스터 제거 (전투 승리 시 등)
        public void RemoveMonster(int monsterId)
        {
            if (_allMonsters.ContainsKey(monsterId))
            {
                var monster = _allMonsters[monsterId];
                _allMonsters.Remove(monsterId);

                // 맵에서도 제거
                var tile = _currentMap?.GetTile(monster.Pos.X, monster.Pos.Y);
                if (tile?.Type == MapTileType.Monster)
                {
                    tile.Type = MapTileType.Ground;
                    _currentMap.MonsterTiles.Remove(tile);
                }
                
            }
        }
        #endregion

        public bool IsValidMove(Position from, Position direction)
        {
            if (_currentMap == null) return false;

            // 목적지가 이동 가능한지 확인
            return _currentMap.IsWalkable(from + direction);
        }

        public MapTile GetTileAt(Position pos)
        {
            return _currentMap?.GetTile(pos.X, pos.Y);
        }

        public List<MapTile> GetSpecialTiles(MapTileType tileType)
        {
            if (_currentMap == null) return new List<MapTile>();

            return tileType switch
            {
                MapTileType.Shop => _currentMap.ShopTiles,
                MapTileType.Portal => _currentMap.PortalTiles,
                MapTileType.Monster => _currentMap.MonsterTiles,
                _ => new List<MapTile>()
            };
        }

        public void DisplayMap(int playerX = -1, int playerY = -1)
        {
            if (_currentMap == null)
            {
                return;
            }


            for (int y = 0; y < _currentMap.Height; y++)
            {
                for (int x = 0; x < _currentMap.Width; x++)
                {
                    MapTile tile = _currentMap.GetTile(x, y);

                    if (x == playerX && y == playerY)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.ResetColor();
                    }
                    else if (tile != null)
                    {
                        char displayChar = (char)tile.Type;

                        // 타일 타입에 따른 색상 설정
                        switch (tile.Type)
                        {
                            case MapTileType.Wall:
                                Console.ForegroundColor = ConsoleColor.Gray;
                                break;
                            case MapTileType.Shop:
                                Console.ForegroundColor = ConsoleColor.Green;
                                break;
                            case MapTileType.Portal:
                                Console.ForegroundColor = ConsoleColor.Magenta;
                                break;
                            case MapTileType.Monster:
                                Console.ForegroundColor = ConsoleColor.Red;
                                break;
                            default:
                                Console.ResetColor();
                                break;
                        }

                        Console.ResetColor();
                    }
                    else
                    {
                        // null 타일 (맵 밖 영역)은 공백으로 출력
                    }
                }
            }
        }

        public MapTile GetPlayerSpawnPoint()
        {
            return _currentMap?.PlayerSpawnPoint;
        }
    }
}