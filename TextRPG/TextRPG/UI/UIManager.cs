using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextRPG.Managers;

namespace TextRPG.UI
{
    public partial class UIManager
    {
        #region Screen Settings
        public const int SCREEN_WIDTH = 120;
        public const int SCREEN_HEIGHT = 40;
        public void ShowMainScreen()
        {
            DrawFrame();
            DrawTitle();
            if (Player.State != IntroState.Instance && Player.State != MakeOrSelectCharacterState.Instance)
            {
                DrawPlayerStatus();
                DrawCurrentLocation();
                DrawInfoPanel();
                
                // 전투 상태가 아닐 때만 맵 표시
                if (!IsInBattle())
                {
                    DrawMap();
                }
                else
                {
                    // 전투 상태일 때 몬스터 ASCII 아트를 중앙에 표시
                    DrawBattleScreen();
                }
            }
            DrawInputFrame(); // 항상 입력 프레임 표시
        }
        private void DrawFrame()
        {
            // 콘솔 창 크기 설정
            try
            {
                Console.SetWindowSize(SCREEN_WIDTH, SCREEN_HEIGHT);
                Console.SetBufferSize(SCREEN_WIDTH, SCREEN_HEIGHT);
            }
            catch (Exception)
            {
                // 콘솔 크기 설정 실패 시 기본 크기로 조정
            }
            
            Console.Clear();
            string topBorder = "┌" + new string('─', SCREEN_WIDTH - 2) + "┐";
            string bottomBorder = "└" + new string('─', SCREEN_WIDTH - 2) + "┘";
            string sideBorder = "│" + new string(' ', SCREEN_WIDTH - 2) + "│";

            // 정확한 위치에 출력 (WriteLine 대신 Write + SetCursorPosition 사용)
            Console.SetCursorPosition(0, 0);
            Console.Write(topBorder);

            for (int i = 1; i < SCREEN_HEIGHT - 1; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write(sideBorder);
            }

            Console.SetCursorPosition(0, SCREEN_HEIGHT - 1);
            Console.Write(bottomBorder);
        }
        private void DrawTitle()
        {
            string title = "*** TextRPG ***";
            int titleX = (SCREEN_WIDTH - title.Length) / 2;
            Console.SetCursorPosition(titleX, 2);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(title);
            Console.ResetColor();
        }
        private void DrawPlayerStatus()
        {
            string playerName = Player.Status.DisplayName;
            // 한글은 2바이트이므로 실제 출력 길이는 문자 개수 * 2
            int nameDisplayLength = playerName.Length * 2;

            // 스탯 값들
            int level = Player.Status.Level;
            string health = $"{Player.Status.HP.ToString()}/{Player.Status.MaxHP.ToString()}";
            int attack = Player.Status.AttackPower;
            int defense = Player.Status.Armor;
            int gold = Player.Status.Gold;
            string job = Player.Status.JobName;

            int levelLength = 6 + level.ToString().Length;
            int healthLength = 6 + health.Length;
            int attackLength = 6 + attack.ToString().Length;
            int defenseLength = 6 + defense.ToString().Length;
            int goldLength = 6 + gold.ToString().Length;
            int jobLength = 6 + job.Length * 2;

            // 가장 긴 스탯 길이 찾기
            int maxStatLength = Math.Max(levelLength,
                               Math.Max(healthLength,
                               Math.Max(attackLength,
                               Math.Max(defenseLength,
                               Math.Max(goldLength, jobLength)))));

            // 프레임 너비 결정 (이름 길이, 스탯 길이 중 최대값 + 여백)
            int minWidth = 18;
            int nameBasedWidth = nameDisplayLength + 6; // 이름 + 여백
            int statBasedWidth = maxStatLength + 4; // 스탯 + 여백
            int statusWidth = Math.Max(minWidth, Math.Max(nameBasedWidth, statBasedWidth));

            int statusX = SCREEN_WIDTH - statusWidth - 2;
            int statusY = 1;
            int statusHeight = 9;

            // 상태창 프레임 그리기
            string statusTopBorder = "┌" + new string('─', statusWidth - 2) + "┐";
            string statusBottomBorder = "└" + new string('─', statusWidth - 2) + "┘";
            string statusSideBorder = "│" + new string(' ', statusWidth - 2) + "│";

            // 상태창 프레임 출력
            Console.SetCursorPosition(statusX, statusY);
            Console.Write(statusTopBorder);

            for (int i = 1; i < statusHeight - 1; i++)
            {
                Console.SetCursorPosition(statusX, statusY + i);
                Console.Write(statusSideBorder);
            }

            Console.SetCursorPosition(statusX, statusY + statusHeight - 1);
            Console.Write(statusBottomBorder);

            // 플레이어 이름을 프레임 맨 위 중간에 출력
            // 한글은 2바이트이므로 실제 출력 길이는 문자 개수 * 2
            int nameX = statusX + (statusWidth - nameDisplayLength) / 2;
            Console.SetCursorPosition(nameX, statusY);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(playerName);
            Console.ResetColor();

            // 플레이어 상태 정보 출력 (값을 오른쪽 정렬)
            int labelWidth = 5; // "레벨:", "체력:" 등의 레이블 길이 (한글 2글자 + 콜론)
            int valueWidth = statusWidth - labelWidth - 3; // 값 표시 공간 (여백 제외)

            // 장비 보너스 계산
            int totalAttack = Player.GetTotalAttackPower();
            int totalDefense = Player.GetTotalArmor();
            int baseAttack = Player.AttackPower;
            int baseDefense = Player.Armor;
            
            // 장비 보너스가 있는지 확인
            int attackBonus = totalAttack - baseAttack;
            int defenseBonus = totalDefense - baseDefense;
            
            string attackDisplay = attackBonus > 0 ? $"{baseAttack}(+{attackBonus})" : baseAttack.ToString();
            string defenseDisplay = defenseBonus > 0 ? $"{baseDefense}(+{defenseBonus})" : baseDefense.ToString();

            Console.SetCursorPosition(statusX + 1, statusY + 1);
            Console.Write("레벨:" + level.ToString().PadLeft(valueWidth));
            Console.SetCursorPosition(statusX + 1, statusY + 2);
            Console.Write("체력:" + health.PadLeft(valueWidth));
            Console.SetCursorPosition(statusX + 1, statusY + 3);
            Console.Write("공격력:" + attackDisplay.PadLeft(valueWidth - 2));
            Console.SetCursorPosition(statusX + 1, statusY + 4);
            Console.Write("방어력:" + defenseDisplay.PadLeft(valueWidth - 2));
            Console.SetCursorPosition(statusX + 1, statusY + 5);
            Console.Write("Gold:" + gold.ToString().PadLeft(valueWidth));
            Console.SetCursorPosition(statusX + 1, statusY + 6);
            Console.Write("직업:" + job.PadLeft(valueWidth - 2));
            Console.SetCursorPosition(statusX + 1, statusY + 7);
            Console.Write($"경험치: {Player.Experience}/{Player.Level*100}");
        }

        private void DrawCurrentLocation()
        {
            // 현재 위치 정보 (나중에 JSON에서 가져올 예정)
            string currentLocation = GetCurrentLocationName();

            // 한글 길이 계산 (한글 1글자 = 2바이트)
            int locationDisplayLength = CalculateDisplayLength(currentLocation);

            // 위치명 길이에 따른 프레임 크기 정확 계산
            int padding = 2; // 좌우 최소 여백
            int frameContentWidth = locationDisplayLength + (padding * 2); // 내용물 + 좌우 여백

            // 짝수/홀수에 따른 프레임 크기 조정 (완벽한 중앙 정렬을 위해)
            int locationFrameWidth;
            if (frameContentWidth % 2 == 0)
            {
                // 짝수: 그대로 사용
                locationFrameWidth = frameContentWidth + 2; // 테두리 추가
            }
            else
            {
                // 홀수: 1 추가해서 짝수로 만들기
                locationFrameWidth = frameContentWidth + 3; // 테두리 + 1 추가
            }

            int locationFrameHeight = 3;

            // 위치 프레임 좌표 (왼쪽 상단)
            int locationFrameX = 2;
            int locationFrameY = 1;

            // 위치 프레임 그리기
            string locationTopBorder = "┌" + new string('─', locationFrameWidth - 2) + "┐";
            string locationBottomBorder = "└" + new string('─', locationFrameWidth - 2) + "┘";
            string locationSideBorder = "│" + new string(' ', locationFrameWidth - 2) + "│";

            // 프레임 테두리 그리기
            Console.SetCursorPosition(locationFrameX, locationFrameY);
            Console.Write(locationTopBorder);

            for (int i = 1; i < locationFrameHeight - 1; i++)
            {
                Console.SetCursorPosition(locationFrameX, locationFrameY + i);
                Console.Write(locationSideBorder);
            }

            Console.SetCursorPosition(locationFrameX, locationFrameY + locationFrameHeight - 1);
            Console.Write(locationBottomBorder);

            // 위치명 완벽한 중앙 정렬 계산
            int availableSpace = locationFrameWidth - 2; // 테두리 제외한 내부 공간
            int leftPadding = (availableSpace - locationDisplayLength) / 2;
            int locationNameX = locationFrameX + 1 + leftPadding;

            Console.SetCursorPosition(locationNameX, locationFrameY + 1);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(currentLocation);
            Console.ResetColor();
        }

        // 현재 위치명 가져오기 (나중에 JSON에서 로드)
        private string GetCurrentLocationName()
        {
            return Manager.Map.CurrentMap.MapName;
        }

        // 한글과 영어 혼합 문자열의 실제 출력 길이 계산
        private int CalculateDisplayLength(string text)
        {
            int length = 0;
            foreach (char c in text)
            {
                // 한글 범위 체크 (유니코드 기준)
                if (c >= 0xAC00 && c <= 0xD7A3) // 한글 완성형
                {
                    length += 2;
                }
                else if (c >= 0x3131 && c <= 0x318E) // 한글 자모
                {
                    length += 2;
                }
                else
                {
                    length += 1; // 영어, 숫자, 기호
                }
            }
            return length;
        }

        private void DrawInfoPanel()
        {
            // 게임 상태에 따라 프레임 크기 결정
            int infoPanelWidth, infoPanelHeight;
            string currentLocation = GetCurrentLocationName();

            // 기본: 중간 크기 (필드 맵에서도 조작법 표시)
            infoPanelWidth = 16;
            infoPanelHeight = 15; // I(인벤토리) 키 추가로 높이 증가

            if (IsInBattle())
            {
                // 전투 중인 경우: 몬스터 정보 표시
                infoPanelWidth = 20; // 몬스터 정보가 더 많으므로 넓게
                infoPanelHeight = 10; // 높이는 줄임
            }

            int infoPanelX = 2;
            int infoPanelY = 5; // 현재 위치 프레임 아래

            // 정보 패널 프레임 그리기
            string infoTopBorder = "┌" + new string('─', infoPanelWidth - 2) + "┐";
            string infoBottomBorder = "└" + new string('─', infoPanelWidth - 2) + "┘";
            string infoSideBorder = "│" + new string(' ', infoPanelWidth - 2) + "│";

            // 프레임 테두리 그리기
            Console.SetCursorPosition(infoPanelX, infoPanelY);
            Console.Write(infoTopBorder);

            for (int i = 1; i < infoPanelHeight - 1; i++)
            {
                Console.SetCursorPosition(infoPanelX, infoPanelY + i);
                Console.Write(infoSideBorder);
            }

            Console.SetCursorPosition(infoPanelX, infoPanelY + infoPanelHeight - 1);
            Console.Write(infoBottomBorder);

            // 현재 게임 상태에 따른 정보 표시
            DisplayInfoContent(infoPanelX, infoPanelY, infoPanelWidth, infoPanelHeight);
        }

        private void DisplayInfoContent(int frameX, int frameY, int frameWidth, int frameHeight)
        {
            // 게임 상태에 따라 다른 정보 표시
            string currentLocation = GetCurrentLocationName();

            if (IsInBattle())
            {
                DisplayMonsterInfo(frameX, frameY, frameWidth, frameHeight);
            }
            else
            {
                DisplayMovementControls(frameX, frameY, frameWidth, frameHeight);
            }
        }

        private void DisplayMovementControls(int frameX, int frameY, int frameWidth, int frameHeight)
        {
            // 제목을 프레임 위쪽 중앙에 표시
            string title = "조작법";
            int titleX = frameX + (frameWidth - title.Length * 2) / 2; // 한글 길이 고려
            Console.SetCursorPosition(titleX, frameY);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(title);
            Console.ResetColor();

            frameX += 1;
            // 방향키 설명
            Console.SetCursorPosition(frameX, frameY + 2);
            Console.Write("↑ : 위");
            Console.SetCursorPosition(frameX, frameY + 3);
            Console.Write("↓ : 아래");
            Console.SetCursorPosition(frameX, frameY + 4);
            Console.Write("← : 왼쪽");
            Console.SetCursorPosition(frameX, frameY + 5);
            Console.Write("→ : 오른쪽");

            // 기능 키들
            Console.SetCursorPosition(frameX, frameY + 6);
            Console.Write("H : 도움말");
            Console.SetCursorPosition(frameX, frameY + 7);
            Console.Write("I : 인벤토리");
            Console.SetCursorPosition(frameX, frameY + 8);
            Console.Write("ESC : 캐릭선택");

            // 맵 요소 설명
            Console.SetCursorPosition(frameX, frameY + 10);
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("_ : 포탈");
            Console.ResetColor();

            Console.SetCursorPosition(frameX, frameY + 11);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("S : 상점");
            Console.ResetColor();

            Console.SetCursorPosition(frameX, frameY + 12);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("M : 몬스터");
            Console.ResetColor();
        }

        private void DisplayMonsterInfo(int frameX, int frameY, int frameWidth, int frameHeight)
        {
            // 몬스터 정보 (예시 - 나중에 실제 몬스터 데이터로 대체)
            Monster monster = Player.Monster;
            int level = monster.Level;
            string monsterName = monster.Name;
            int monsterHP = monster.HP;
            int monsterAttack = monster.Power;
            int monsterDefense = monster.Armor;

            // 제목
            Console.SetCursorPosition(frameX + 2, frameY + 1);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("적 정보");
            Console.ResetColor();

            // 몬스터 이름
            Console.SetCursorPosition(frameX + 2, frameY + 3);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"Lv. {level}");
            Console.SetCursorPosition(frameX + 2, frameY + 4);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"이름: {monsterName}");
            Console.ResetColor();

            // 몬스터 스탯
            Console.SetCursorPosition(frameX + 2, frameY + 6);
            Console.Write($"체력: {monsterHP}");
            Console.SetCursorPosition(frameX + 2, frameY + 7);
            Console.Write($"공격력: {monsterAttack}");
            Console.SetCursorPosition(frameX + 2, frameY + 8);
            Console.Write($"방어력: {monsterDefense}");
            Console.ResetColor();
        }


        // 전투 중인지 확인하는 메서드 (나중에 실제 전투 상태로 대체)
        private bool IsInBattle()
        {
            // TODO: 실제 전투 상태 확인 로직
            return Player.State == BattleState.Instance; // 임시로 false 반환
        }

        // 전투 화면 그리기
        private void DrawBattleScreen()
        {
            // 임시로 고블린 몬스터 데이터를 사용 (나중에 실제 전투 중인 몬스터로 대체)
            var monsterData = Manager.Data.GetMonsterData(Player.Monster.NameHide);
            
            if (monsterData.HasValue)
            {
                var monster = monsterData.Value;
                
                // ASCII 아트가 있는 경우 화면 중앙에 표시
                if (monster.AsciiArt != null && monster.AsciiArt.Count > 0)
                {
                    // 화면 중앙 계산
                    int artWidth = monster.AsciiArt.Max(line => line.Length);
                    int artHeight = monster.AsciiArt.Count;
                    
                    int startX = (SCREEN_WIDTH - artWidth) / 2;
                    int startY = (SCREEN_HEIGHT - artHeight) / 2;
                    
                    // ASCII 아트 출력
                    for (int i = 0; i < monster.AsciiArt.Count; i++)
                    {
                        int lineX = startX;
                        int lineY = startY + i;
                        
                        // 화면 범위 내에 있을 때만 출력
                        if (lineY > 0 && lineY < SCREEN_HEIGHT - 1)
                        {
                            Console.SetCursorPosition(lineX, lineY);
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(monster.AsciiArt[i]);
                            Console.ResetColor();
                        }
                    }
                    
                    // 몬스터 이름을 ASCII 아트 아래에 표시
                    string monsterName = $"[{monster.Name}]";
                    int nameX = (SCREEN_WIDTH - monsterName.Length * 2) / 2; // 한글 고려
                    int nameY = startY + artHeight + 1;
                    
                    if (nameY < SCREEN_HEIGHT - 3)
                    {
                        Console.SetCursorPosition(nameX, nameY);
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write(monsterName);
                        Console.ResetColor();
                    }
                }
                else
                {
                    // ASCII 아트가 없는 경우 기본 몬스터 표시
                    string monsterDisplay = $"[{monster.Name}]";
                    int displayX = (SCREEN_WIDTH - monsterDisplay.Length * 2) / 2;
                    int displayY = SCREEN_HEIGHT / 2;
                    
                    Console.SetCursorPosition(displayX, displayY);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(monsterDisplay);
                    Console.ResetColor();
                }
            }
            else
            {
                // 몬스터 데이터가 없는 경우 기본 전투 화면
                string battleText = "[전투 중]";
                int textX = (SCREEN_WIDTH - battleText.Length * 2) / 2;
                int textY = SCREEN_HEIGHT / 2;
                
                Console.SetCursorPosition(textX, textY);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(battleText);
                Console.ResetColor();
            }
        }

        private void DrawMap()
        {
            if (Manager.Map.CurrentMap == null)
                return;

            var currentMap = Manager.Map.CurrentMap;

            // 맵 출력 위치 계산 (화면 중앙)
            int mapStartX = (SCREEN_WIDTH - currentMap.Width) / 2;
            int mapStartY = (SCREEN_HEIGHT - currentMap.Height) / 2;

            // 플레이어 위치 가져오기
            var playerPos = Player.Position;

            // 맵 출력
            for (int y = 0; y < currentMap.Height; y++)
            {
                for (int x = 0; x < currentMap.Width; x++)
                {
                    var tile = currentMap.GetTile(x, y);

                    // 화면 좌표로 변환
                    int screenX = mapStartX + x;
                    int screenY = mapStartY + y;

                    // 화면 범위 내에 있을 때만 출력
                    if (screenX > 0 && screenX < SCREEN_WIDTH - 1 &&
                        screenY > 0 && screenY < SCREEN_HEIGHT - 1)
                    {
                        Console.SetCursorPosition(screenX, screenY);

                        // 플레이어 위치인지 확인
                        if (x == playerPos.X && y == playerPos.Y)
                        {
                            // 플레이어 표시
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write('P');
                            Console.ResetColor();
                        }
                        else if (tile != null)
                        {
                            // 타일 타입에 따른 색상 설정
                            switch (tile.Type)
                            {
                                case Utils.MapTileType.Wall:
                                    Console.ForegroundColor = ConsoleColor.Gray;
                                    Console.Write('#');
                                    break;
                                case Utils.MapTileType.Ground:
                                    Console.ForegroundColor = ConsoleColor.White;
                                    Console.Write('.');
                                    break;
                                case Utils.MapTileType.Shop:
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.Write('S');
                                    break;
                                case Utils.MapTileType.Portal:
                                    Console.ForegroundColor = ConsoleColor.Magenta;
                                    Console.Write('_');  // 포탈은 플레이어에게 _로 표시
                                    break;
                                case Utils.MapTileType.Monster:
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.Write('M');
                                    break;
                                default:
                                    Console.ResetColor();
                                    Console.Write('.');
                                    break;
                            }
                            Console.ResetColor();
                        }
                        else
                        {
                            // null 타일은 공백으로 출력 (맵 밖 영역)
                            Console.Write(' ');
                        }
                    }
                }
            }
        }

        public void DrawInputFrame(string[] messages = null)
        {
            // 메시지가 없으면 빈 프레임만 표시
            int messageLineCount = 0;
            bool hasMessages = messages != null && messages.Length > 0;

            if (hasMessages)
            {
                messageLineCount = messages.Length;
            }

            // 메시지 줄 수에 따라 입력 프레임 높이 자동 계산
            int minFrameHeight = 5; // 최소 프레임 높이 (테두리 + 입력줄만)
            int inputFrameHeight = hasMessages ?
                Math.Max(minFrameHeight, messageLineCount + 3) : // 메시지 + 테두리 + 입력줄
                minFrameHeight; // 메시지 없으면 최소 높이

            // 프레임 위치 계산
            int inputFrameY = SCREEN_HEIGHT - inputFrameHeight - 1;
            int inputFrameX = 2;
            int inputFrameWidth = SCREEN_WIDTH - 4;

            // 입력 프레임 그리기
            DrawInputFrameBorder(inputFrameX, inputFrameY, inputFrameWidth, inputFrameHeight);

            // 메시지가 있을 때만 출력
            if (hasMessages)
            {
                for (int i = 0; i < messages.Length; i++)
                {
                    Console.SetCursorPosition(inputFrameX + 1, inputFrameY + 1 + i);

                    // 메시지가 프레임 너비를 초과하면 자르기
                    string message = messages[i];
                    int maxMessageWidth = inputFrameWidth - 2; // 좌우 여백 제외
                    if (message.Length > maxMessageWidth)
                        message = message.Substring(0, maxMessageWidth);

                    Console.Write(message);
                }
            }

            // 입력 프롬프트 표시
            Console.SetCursorPosition(inputFrameX + 1, inputFrameY + inputFrameHeight - 2);
        }

        // 입력 프레임 테두리만 그리기
        private static void DrawInputFrameBorder(int x, int y, int width, int height)
        {
            string topBorder = "┌" + new string('─', width - 2) + "┐";
            string bottomBorder = "└" + new string('─', width - 2) + "┘";
            string sideBorder = "│" + new string(' ', width - 2) + "│";

            // 상단 테두리
            Console.SetCursorPosition(x, y);
            Console.Write(topBorder);

            // 좌우 테두리
            for (int i = 1; i < height - 1; i++)
            {
                Console.SetCursorPosition(x, y + i);
                Console.Write(sideBorder);
            }

            // 하단 테두리
            Console.SetCursorPosition(x, y + height - 1);
            Console.Write(bottomBorder);
        }

        // 메시지와 함께 입력 프레임 업데이트
        public void UpdateInputFrame(string[] messages)
        {
            DrawInputFrame(messages);
        }

        // 단일 메시지로 입력 프레임 업데이트
        public void UpdateInputFrame(string message)
        {
            DrawInputFrame(new string[] { message });
        }

        #endregion
        public void Init()
        {
            Console.Title = "TextRPG";
            Console.WindowWidth = 80;
            Console.WindowHeight = 45;

            #region Intro
            MakeCharacter.SelectCharacterJobsText.FormatMessage((originalMsg) =>
            {
                // 0넘버  1 직업명
                string formatHandler = "{0}. {1}\n";
                string f = "";

                MakeCharacter.SelectCharacterJobsText.ChoiceCount = Manager.Data.CharacterStatuses.Count;

                foreach (var characterStatus in Manager.Data.CharacterStatuses)
                {
                    f += string.Format(formatHandler,
                        characterStatus.Key,
                        characterStatus.Value.JobName);
                }
                if (f.Length > 0 && f[f.Length - 1] == '\n')
                    f = f.Substring(0, f.Length - 1); // 마지막 줄바꿈 제거
                return string.Format(originalMsg, f);
            });
            SelectCharacter.SelectCharacterText.FormatMessage((originalMsg) =>
            {
                // 0 넘버, 1 레벨, 2 캐릭터 이름, 3 직업명
                string formatHandler = "{0}. Lv{1} {2}({3})\n";
                string f = "";

                SelectCharacter.SelectCharacterText.ChoiceCount = Manager.Data.PlayerCharacters.Count;
                var list = Manager.Data.PlayerCharacters.ToList();
                for (int i = 0; i < list.Count; i++)
                {
                    var characterInfo = list[i];
                    f += string.Format(formatHandler,
                        i + 1,
                        characterInfo.Value.Status.Level,
                        characterInfo.Value.Status.DisplayName,
                        characterInfo.Value.Status.JobName);
                }
                if (f.Length > 0 && f[f.Length - 1] == '\n')
                    f = f.Substring(0, f.Length - 1);
                return string.Format(originalMsg, f);
            });
            #endregion
            #region InGame
            Shop.SelectPotionText.FormatMessage((originalMsg) =>
            {
                // 0 넘버, 1 아이템 이름, 2 가격
                string formatHandler = "{0}. {1} ({2} Gold)\n";
                string f = "";

                Shop.SelectPotionText.ChoiceCount = Manager.Data.Potions.Count + 1;
                for (int i = 0; i < Manager.Data.Potions.Count; i++)
                {
                    var potion = Manager.Data.Potions[i];
                    f += string.Format(formatHandler,
                        i + 1,
                        potion.Name,
                        potion.Gold);
                }
                return string.Format(originalMsg, f);
            });
            Shop.ShowArmorsText.FormatMessage((originalMsg) =>
            {
                // 0 넘버, 1 아이템 이름, 2 가격
                string formatHandler = "{0}. {1} ({2} Gold)\n";
                string f = "";

                Shop.ShowArmorsText.ChoiceCount = Manager.Data.Armors.Count + 1;
                for (int i = 0; i < Manager.Data.Armors.Count; i++)
                {
                    var armor = Manager.Data.Armors[i];
                    f += string.Format(formatHandler,
                        i + 1,
                        armor.Name,
                        armor.Gold);
                }
                return string.Format(originalMsg, f);
            });
            Shop.ShowHelmetsText.FormatMessage((originalMsg) =>
            {
                // 0 넘버, 1 아이템 이름, 2 가격
                string formatHandler = "{0}. {1} ({2} Gold)\n";
                string f = "";

                Shop.ShowHelmetsText.ChoiceCount = Manager.Data.Helmets.Count + 1;
                for (int i = 0; i < Manager.Data.Helmets.Count; i++)
                {
                    var helmet = Manager.Data.Helmets[i];
                    f += string.Format(formatHandler,
                        i + 1,
                        helmet.Name,
                        helmet.Gold);
                }
                return string.Format(originalMsg, f);
            });
            Shop.ShowShoesText.FormatMessage((originalMsg) =>
            {
                // 0 넘버, 1 아이템 이름, 2 가격
                string formatHandler = "{0}. {1} ({2} Gold)\n";
                string f = "";

                Shop.ShowShoesText.ChoiceCount = Manager.Data.Shoes.Count + 1;
                for (int i = 0; i < Manager.Data.Shoes.Count; i++)
                {
                    var shoe = Manager.Data.Shoes[i];
                    f += string.Format(formatHandler,
                        i + 1,
                        shoe.Name,
                        shoe.Gold);
                }
                return string.Format(originalMsg, f);
            });
            Shop.ShowSwordsText.FormatMessage((originalMsg) =>
            {
                // 0 넘버, 1 아이템 이름, 2 가격
                string formatHandler = "{0}. {1} ({2} Gold)\n";
                string f = "";

                Shop.ShowSwordsText.ChoiceCount = Manager.Data.Swords.Count + 1;
                for (int i = 0; i < Manager.Data.Swords.Count; i++)
                {
                    var sword = Manager.Data.Swords[i];
                    f += string.Format(formatHandler,
                        i + 1,
                        sword.Name,
                        sword.Gold);
                }
                return string.Format(originalMsg, f);
            });
            #endregion
        }
        public string GetInput()
        {
            Console.Write(">> ");

            // 입력 버퍼 클리어
            while (Console.KeyAvailable)
            {
                Console.ReadKey(true);
            }

            ConsoleKeyInfo keyInfo;

            // 숫자 키만 처리 (메뉴 선택용)
            do
            {
                keyInfo = Console.ReadKey(false);

                // 숫자 키 (0-9)면 바로 반환
                if (char.IsDigit(keyInfo.KeyChar))
                {
                    return keyInfo.KeyChar.ToString();
                }

                // 영어 키 (a-z, A-Z)면 바로 반환
                if (char.IsLetter(keyInfo.KeyChar) && keyInfo.KeyChar <= 127) // ASCII 범위
                {
                    return keyInfo.KeyChar.ToString();
                }

                // 한글이나 기타 문자는 엔터까지 입력 받기
                if (keyInfo.KeyChar > 127) // 비ASCII 문자 (한글 등)
                {
                    string input = keyInfo.KeyChar.ToString();
                    ConsoleKeyInfo nextKey;

                    do
                    {
                        nextKey = Console.ReadKey(false);
                        if (nextKey.Key != ConsoleKey.Enter && nextKey.KeyChar != '\0')
                        {
                            input += nextKey.KeyChar;
                        }
                    } while (nextKey.Key != ConsoleKey.Enter);

                    return input;
                }

            } while (true);
        }
        public int GetInputAsInt()
        {
            if (int.TryParse(GetInput(), out int ret) == true)
                return ret;
            return -1;
        }
        public ConsoleKey GetInputAsKeyInfo()
        {
            Console.Write(">> ");
            ConsoleKey key = Console.ReadKey(true).Key;
            switch (key)
            {
                case ConsoleKey.NumPad0:
                    return ConsoleKey.D0;
                case ConsoleKey.NumPad1:
                    return ConsoleKey.D1;
                case ConsoleKey.NumPad2:
                    return ConsoleKey.D2;
                case ConsoleKey.NumPad3:
                    return ConsoleKey.D3;
                case ConsoleKey.NumPad4:
                    return ConsoleKey.D4;
                case ConsoleKey.NumPad5:
                    return ConsoleKey.D5;
                case ConsoleKey.NumPad6:
                    return ConsoleKey.D6;
                case ConsoleKey.NumPad7:
                    return ConsoleKey.D7;
                case ConsoleKey.NumPad8:
                    return ConsoleKey.D8;
                case ConsoleKey.NumPad9:
                    return ConsoleKey.D9;
                case ConsoleKey.A:
                    return ConsoleKey.LeftArrow;
                case ConsoleKey.D:
                    return ConsoleKey.RightArrow;
                case ConsoleKey.W:
                    return ConsoleKey.UpArrow;
                case ConsoleKey.S:
                    return ConsoleKey.DownArrow;
                default:
                    return key;
            }
        }
    }
}
