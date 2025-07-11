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
            _currentMap = _loadedMaps["Town"];
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
                    Console.WriteLine("Portals.json 로드 완료");
                }
                else
                {
                    Console.WriteLine("Portals.json 파일을 찾을 수 없습니다.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Portals.json 로드 중 오류 발생: {ex.Message}");
            }
        }
        public void LoadAllMaps()
        {
            try
            {
                // Data/Map 디렉터리가 존재하는지 확인
                if (!Directory.Exists(_mapDataPath))
                {
                    Console.WriteLine($"맵 디렉터리가 존재하지 않습니다: {_mapDataPath}");
                    return;
                }

                // 모든 .txt 파일 검색
                string[] mapFiles = Directory.GetFiles(_mapDataPath, "*.txt");

                if (mapFiles.Length == 0)
                {
                    Console.WriteLine("맵 파일이 존재하지 않습니다.");
                    return;
                }

                Console.WriteLine($"맵 파일 로드 중... ({mapFiles.Length}개 파일)");

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
                            Console.WriteLine($"✓ {mapName} 맵 로드 완료");
                        }
                        else
                        {
                            Console.WriteLine($"✗ {mapName} 맵 로드 실패 (파싱 오류)");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"✗ {mapName} 맵 로드 실패: {ex.Message}");
                    }
                }

                Console.WriteLine($"맵 로드 완료: {loadedCount}/{mapFiles.Length}개");
                Console.WriteLine("로드된 맵 목록:");
                foreach (var mapName in _loadedMaps.Keys)
                {
                    Console.WriteLine($"  - {mapName}");
                }

                // 기본 맵 설정 (Town이 있으면 기본으로 설정)
                if (_loadedMaps.ContainsKey("Town"))
                {
                    _currentMap = _loadedMaps["Town"];
                    Console.WriteLine("기본 맵을 'Town'으로 설정했습니다.");
                }
                else if (_loadedMaps.ContainsKey("town"))
                {
                    _currentMap = _loadedMaps["town"];
                    Console.WriteLine("기본 맵을 'town'으로 설정했습니다.");
                }
                else if (_loadedMaps.Count > 0)
                {
                    // town이 없으면 첫 번째 맵을 기본으로 설정
                    var firstMap = _loadedMaps.First();
                    _currentMap = firstMap.Value;
                    Console.WriteLine($"기본 맵을 '{firstMap.Key}'으로 설정했습니다.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"맵 로드 중 오류 발생: {ex.Message}");
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

            Console.WriteLine($"맵 '{mapName}'이 로드되지 않았습니다.");
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
                Console.WriteLine($"맵 '{mapName}'이 로드되지 않았습니다.");
                return (-1, -1);
            }

            var map = _loadedMaps[mapName];
            var portals = map.PortalTiles;

            // to_id는 목적지 맵에서 해당하는 포탈의 배열 인덱스
            // 즉, Field_001의 to_id 0은 Field_001 포탈 배열의 0번째 포탈
            if (portalId < 0 || portalId >= portals.Count)
            {
                Console.WriteLine($"맵 '{mapName}'에서 포탈 ID {portalId}를 찾을 수 없습니다. (총 {portals.Count}개 포탈 존재)");
                return (-1, -1);
            }

            // 목적지 맵의 portalId번째 포탈의 spawn_pos를 가져옴
            var destinationPortal = portals[portalId];
            var (_, _, spawnX, spawnY) = destinationPortal.GetPortalInfo();
            
            Console.WriteLine($"포탈 이동: {mapName}의 {portalId}번 포탈 spawn_pos = ({spawnX}, {spawnY})");
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
                Console.WriteLine($"[DEBUG] {mapName} 포탈 {i}번 검색: JSON pos({portalX},{portalY}) - 타일타입: {tile?.Type}");
                
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
                    
                    Console.WriteLine($"✓ {mapName} 포탈 {i}번 설정완료: 위치({portalX},{portalY}) → {portalInfo.to_map} spawn_pos({portalInfo.spawn_pos[0]},{portalInfo.spawn_pos[1]})");
                }
                else
                {
                    Console.WriteLine($"✗ {mapName} 포탈 {i}번 실패: 위치({portalX},{portalY})에 포탈 타일이 없습니다!");
                }
            }
            
            Console.WriteLine($"{mapName} 포탈 설정 완료: {mapData.PortalTiles.Count}개 포탈");
        }
        #endregion

        #region 몬스터 스폰 시스템
        // 모든 맵에 몬스터 스폰 (Town 제외)
        private void SpawnMonstersInAllMaps()
        {
            foreach (var kvp in _loadedMaps)
            {
                string mapName = kvp.Key;
                MapData mapData = kvp.Value;

                // Town은 몬스터 스폰 제외
                if (mapName.Equals("Town", StringComparison.OrdinalIgnoreCase))
                    continue;

                SpawnMonstersInMap(mapData, mapName);
            }
        }

        // 특정 맵에 몬스터 스폰
        private void SpawnMonstersInMap(MapData mapData, string mapName)
        {
            // 2~3마리 랜덤 생성
            int monsterCount = _random.Next(2, 4); // 2~3마리
            
            Console.WriteLine($"{mapName}에 {monsterCount}마리 몬스터 스폰 시도...");

            for (int i = 0; i < monsterCount; i++)
            {
                Position? spawnPos = FindRandomGroundPosition(mapData);
                if (spawnPos.HasValue)
                {
                    // 몬스터 생성 (모든 몬스터를 "Dooly"로 생성)
                    Monster monster = new Monster(spawnPos.Value, "Dooly");
                    
                    // 전역 몬스터 딕셔너리에 추가
                    _allMonsters[monster.ID] = monster;
                    
                    // 해당 위치를 몬스터 타일로 설정
                    var tile = mapData.GetTile(spawnPos.Value.X, spawnPos.Value.Y);
                    if (tile != null)
                    {
                        tile.Type = MapTileType.Monster;
                        tile.SetMonsterInfo(monster.ID);
                        mapData.MonsterTiles.Add(tile);
                        Console.WriteLine($"✓ {mapName}에 {monster} 스폰: ({spawnPos.Value.X}, {spawnPos.Value.Y})");
                    }
                }
                else
                {
                    Console.WriteLine($"✗ {mapName}에서 몬스터 스폰 위치를 찾지 못했습니다.");
                }
            }
            
            Console.WriteLine($"{mapName} 몬스터 스폰 완료: {mapData.MonsterTiles.Count}마리");
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
                
                Console.WriteLine($"몬스터 {monster.Name} (ID: {monsterId}) 제거됨");
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
                Console.WriteLine("로드된 맵이 없습니다.");
                return;
            }

            Console.WriteLine($"=== {_currentMap.MapName} 맵 ===");

            for (int y = 0; y < _currentMap.Height; y++)
            {
                for (int x = 0; x < _currentMap.Width; x++)
                {
                    MapTile tile = _currentMap.GetTile(x, y);

                    if (x == playerX && y == playerY)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write('@');
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

                        Console.Write(displayChar);
                        Console.ResetColor();
                    }
                    else
                    {
                        // null 타일 (맵 밖 영역)은 공백으로 출력
                        Console.Write(' ');
                    }
                }
                Console.WriteLine();
            }
        }

        public MapTile GetPlayerSpawnPoint()
        {
            return _currentMap?.PlayerSpawnPoint;
        }
    }
}