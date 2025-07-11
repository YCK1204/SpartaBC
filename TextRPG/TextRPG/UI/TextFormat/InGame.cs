using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextRPG.Utils;

namespace TextRPG.UI
{
    public partial class UIManager
    {
        public T_Tutorial Tutorial = new T_Tutorial();
        public T_Idle Idle = new T_Idle();
        public T_Shop Shop = new T_Shop();
        public T_Inventory Inventory = new T_Inventory();
        public class T_Tutorial
        {
            public UILogHandler AskShowTutorialText = new UILogHandler(UIMsgType.PROMPT_FOR_INPUT, _askShowTutorialText, 2);
            public UILogHandler Tutorial1Text = new UILogHandler(UIMsgType.MSG_WITH_WAIT, _tutorial1Text);
            public UILogHandler Tutorial2Text = new UILogHandler(UIMsgType.MSG_WITH_WAIT, _tutorial2Text);
            public UILogHandler Tutorial3Text = new UILogHandler(UIMsgType.MSG_WITH_WAIT, _tutorial3Text);
            public UILogHandler Tutorial4Text = new UILogHandler(UIMsgType.MSG_WITH_WAIT, _tutorial4Text);
            public UILogHandler EndTutorialText = new UILogHandler(UIMsgType.MSG_WITH_WAIT, _endTutorialText);
            public UILogHandler FailedTutorialText = new UILogHandler(UIMsgType.MSG_WITH_WAIT, _failedTutorialText);
            const string _askShowTutorialText =
@"튜토리얼을 보시겠습니까?
1. 예
2. 아니오";
            const string _tutorial1Text =
@"TextRPG세계에 어서오세요. 적당히 몬스터 잡고 레벨업하는 세계입니다.
좌측에 현재 위치와 방향키, 우측에 캐릭터 정보가 나옵니다.
몬스터와 전투시 좌측에 몬스터 정보가 나옵니다.";
            const string _tutorial2Text =
@"방향키를 이동해서 캐릭터를 움직일 수 있습니다.
포탈에 접근 시 연결된 맵으로 이동합니다.
상점에 접근 시 상점 씬으로 전환됩니다.
상점에서 장비들을 구입할 수 있습니다.";
            const string _tutorial3Text =
@"게임 데이터는 데이터가 업데이트 되는 매 순간 자동적으로 저장됩니다.";
            const string _tutorial4Text =
@"캐릭터 사망 시 현재 캐릭터의 모든 정보가 삭제되고
처음부터 다시 시작됩니다.";
            const string _endTutorialText =
@"마지막으로 뭔가 오류가 생겨서 게임이 터진다면
그건 개발자 잘못이 아닙니다.
그럼 수고용";
            const string _failedTutorialText =
@"지금은 안돼요.";
        }
        public class T_Idle
        {
            public UILogHandler AskPortalText = new UILogHandler(UIMsgType.PROMPT_FOR_INPUT, _askPortalText, 2);
            public UILogHandler AskBackToCharacterSelect = new UILogHandler(UIMsgType.PROMPT_FOR_INPUT, _askBackToCharacterSelect, 2);
            const string _askPortalText =
@"{0}로 이동하시겠습니까?
1. 예
2. 아니오";
            const string _askBackToCharacterSelect =
@"캐릭터 선택 화면으로 돌아가시겠습니까?
1. 예
2. 아니오";
        }
        public class T_Inventory
        {
            public UILogHandler AskShowInventoryText = new UILogHandler(UIMsgType.PROMPT_FOR_INPUT, _askShowInventoryText, 2);
            public UILogHandler SelectItemKindText = new UILogHandler(UIMsgType.PROMPT_FOR_INPUT, _selectItemKindText, 2);
            public UILogHandler SelectPotionText = new UILogHandler(UIMsgType.PROMPT_FOR_INPUT, _selectPotionText);
            public UILogHandler SelectHelmetsText = new UILogHandler(UIMsgType.PROMPT_FOR_INPUT, _selectHelmetsText);
            public UILogHandler SelectArmorsText = new UILogHandler(UIMsgType.PROMPT_FOR_INPUT, _selectArmorsText);
            public UILogHandler SelectShoesText = new UILogHandler(UIMsgType.PROMPT_FOR_INPUT, _selectShoesText);
            public UILogHandler SelectSwordsText = new UILogHandler(UIMsgType.PROMPT_FOR_INPUT, _selectSworsText);
            public UILogHandler ShowSelectedItemText = new UILogHandler(UIMsgType.PROMPT_FOR_INPUT, _showSelectedItemText, 2);
            public UILogHandler FailedCellItemText = new UILogHandler(UIMsgType.MSG_WITH_WAIT, _failedCellItemText);

            const string _askShowInventoryText =
@"0. 인벤토리 나가기
1. 아이템 종류 선택";
            const string _selectItemKindText =
@"보고싶은 아이템 종류 선택
0. 뒤로가기
1. 포션
2. 투구
3. 갑옷
4. 신발
5. 무기";
            const string _selectPotionText =
@"------- 포션 목록 -------
0. 뒤로가기
{0}";
            const string _selectHelmetsText =
@"------- 투구 목록 -------
0. 뒤로가기
{0}";
            const string _selectArmorsText =
@"------- 갑옷 목록 -------
0. 뒤로가기
{0}";
            const string _selectShoesText =
@"------- 신발 목록 -------
0. 뒤로가기
{0}";
            const string _selectSworsText =
@"------- 무기 목록 -------
0. 뒤로가기
{0}";
            const string _showSelectedItemText =
@"0. 뒤로가기
1. 장착/사용
2. 되팔기 ({3}Gold)
{0}
설명: {1}
성능: {2}";
            const string _failedCellItemText =
@"상점에서만 아이템을 되팔 수 있습니다.";
        }
        public class T_Shop
        {
            public UILogHandler AskShowShopText = new UILogHandler(UIMsgType.PROMPT_FOR_INPUT, _askShowShopText, 2);
            public UILogHandler SelectItemKindText = new UILogHandler(UIMsgType.PROMPT_FOR_INPUT, _selectItemKindText, 3);
            public UILogHandler SelectPotionText = new UILogHandler(UIMsgType.PROMPT_FOR_INPUT, _selectPotionText);
            public UILogHandler ShowHelmetsText = new UILogHandler(UIMsgType.PROMPT_FOR_INPUT, _showHelmetsText);
            public UILogHandler ShowArmorsText = new UILogHandler(UIMsgType.PROMPT_FOR_INPUT, _showArmorsText);
            public UILogHandler ShowShoesText = new UILogHandler(UIMsgType.PROMPT_FOR_INPUT, _showShoesText);
            public UILogHandler ShowSwordsText = new UILogHandler(UIMsgType.PROMPT_FOR_INPUT, _showSwordsText);
            public UILogHandler ShowItemDetailText = new UILogHandler(UIMsgType.PROMPT_FOR_INPUT, _showItemDetailText, 2);
            public UILogHandler OnPurchaseText = new UILogHandler(UIMsgType.MSG_WITH_WAIT, _onPurchaseText);
            public UILogHandler FailedPurchaseText = new UILogHandler(UIMsgType.MSG_WITH_WAIT, _failedPurchaseText);

            const string _askShowShopText =
@"상점으로 들어가시겠습니까?
1. 예
2. 아니오";
            const string _selectItemKindText =
@"보고싶은 아이템 종류 선택
0. 상점 나가기
1. 포션
2. 투구
3. 값옷
4. 신발
5. 무기";
            const string _selectPotionText =
@"------- 포션 목록 -------
0. 뒤로가기
{0}";

            const string _showHelmetsText =
@"------- 투구 목록 -------
0. 뒤로가기
{0}";
            const string _showArmorsText =
@"------- 갑옷 목록 -------
0. 뒤로가기
{0}";
            const string _showShoesText =
@"------- 신발 목록 -------
0. 뒤로가기
{0}";
            const string _showSwordsText =
@"------- 무기 목록 -------
0. 뒤로가기
{0}";
            const string _showItemDetailText =
@"------- 아이템 정보 -------
{0}
설명: {1}
성능: {2}
가격: {3}Gold
1. 구입
2. ㄴㄴ";
            const string _failedPurchaseText =
@"Gold가 부족합니다.";
            const string _onPurchaseText =
@"{0} 아이템을 구입하였습니다.";
        }
    }
}
