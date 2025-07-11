using TextRPG.Utils;

namespace TextRPG.UI
{
    public partial class UIManager
    {
        public T_ETC ETC = new T_ETC();
        public T_Intro Intro = new T_Intro();
        public T_MakeCharacter MakeCharacter = new T_MakeCharacter();
        public T_SelectCharacter SelectCharacter = new T_SelectCharacter();
        public class T_ETC
        {
            public UILogHandler ContinueText = new UILogHandler(UIMsgType.MSG_WITH_WAIT, _continueText);
            public UILogHandler FailedUnknownErrorText = new UILogHandler(UIMsgType.MSG_WITH_WAIT, _failedUnknownErrorText);
            public UILogHandler FailedOutOfRangeText = new UILogHandler(UIMsgType.MSG_WITH_WAIT, _failedOutOfRangeText);
            const string _continueText =
@"계속하려면 아무키나 누르세요...";
            const string _failedUnknownErrorText =
@"알 수 없는 오류가 발생했습니다. 다시 시도해주세요.";
            const string _failedOutOfRangeText =
@"입력값이 범위를 벗어났습니다. 다시 입력해주세요.";
        }
        public class T_Intro
        {
            public UILogHandler GameStartText = new UILogHandler(UIMsgType.MSG_WITH_WAIT, _gameStartText);
            public UILogHandler IntroText = new UILogHandler(UIMsgType.MSG_WITH_WAIT, _introText);
            const string _gameStartText =
@"게임을 시작하려면 아무키나 누르세요...";
            const string _introText =
@"TextRPG세계에 오신것을 환영합니다.
TextRPG세계는 대충 몬스터가 있고 모험가가 있는 그런 세계입니다.
당신은 적당히 캐릭터를 만들거나 선택해서 시작할 수 있습니다.";
        }
        public class T_MakeCharacter
        {
            public UILogHandler MakeOrSelectCharacterText = new UILogHandler(UIMsgType.PROMPT_FOR_INPUT, _makeOrSelectCharacterText, 3);
            public UILogHandler SelectCharacterJobsText = new UILogHandler(UIMsgType.PROMPT_FOR_INPUT, _selectCharacterJobsText);
            public UILogHandler CharacterSummaryText = new UILogHandler(UIMsgType.PROMPT_FOR_INPUT, _characterSummaryText);
            public UILogHandler SetCharacterNameText = new UILogHandler(UIMsgType.PROMPT_FOR_INPUT, _setCharacterNameText);
            public UILogHandler ReAskSetCharacterNameText = new UILogHandler(UIMsgType.PROMPT_FOR_INPUT, _reAskSetCharacterNameText, 2);
            public UILogHandler FailedSetCharacterNameText = new UILogHandler(UIMsgType.MSG_WITH_WAIT, _failedSetCharacterNameText);
            public UILogHandler OnSetCharacterNameText = new UILogHandler(UIMsgType.MSG_WITH_WAIT, _onSetCharacterNameText);
            public UILogHandler FailedOverNumOfCharacterCount = new UILogHandler(UIMsgType.MSG_WITH_WAIT, _failedOverNumOfCharacterCount);


            const string _makeOrSelectCharacterText =
@"0. 게임 종료
1. 캐릭터를 생성(최대 9개까지 생성 가능)
2. 기존 캐릭터 선택";
            // 0 넘버, 1 직업명
            // string selectCharacterJobsTextHandler = "{0}. {1}"
            const string _selectCharacterJobsText =
@"캐릭터 직업을 선택하세요.
0. 뒤로가기
{0}";

            // --------------------- 매번 Format 필요 ---------------------
            const string _characterSummaryText =
@"{0}
{1}로 선택하시겠습니까?
1. ㅇㅇ
2. ㄴㄴ";
            const string _setCharacterNameText =
@"캐릭터 닉네임을 설정하세요 (최소2자 ~ 최대6자):";
            const string _reAskSetCharacterNameText =
@"ㄹㅇ로 캐릭터 닉네임을 그렇게 할건가요?
1. ㅇㅇ
2. ㄴㄴ";
            const string _failedSetCharacterNameText =
@"닉네임길이는 2자 ~ 6자 입력할 수 있습니다.";
            const string _onSetCharacterNameText =
@"당신의 {0} 캐릭터 닉네임은 {1}입니다.";
            const string _failedOverNumOfCharacterCount =
@"캐릭터가 이미 9개나 있어서 안되는데요?";
        }
        public class T_SelectCharacter
        {
            public UILogHandler SelectCharacterText = new UILogHandler(UIMsgType.PROMPT_FOR_INPUT, _selectCharacterText);
            public UILogHandler FailedSelectCharacterByEmptyListText = new UILogHandler(UIMsgType.MSG_WITH_WAIT, _failedSelectCharacterByEmptyListText);

            // 0 넘버, 1레벨, 2 캐릭터 이름, 3 직업명
            // string selectCharacterTextHandler = "{0}. Lv{1} {2}({3})"
            const string _selectCharacterText =
@"캐릭터를 선택하세요.
0. 뒤로가기
{0}";
            const string _failedSelectCharacterByEmptyListText =
@"캐릭터가 없는데요? 캐릭터를 생성해주세요~";

        }
    }
}
