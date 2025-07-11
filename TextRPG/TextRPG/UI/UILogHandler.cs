using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TextRPG.Managers;
using TextRPG.Utils;

namespace TextRPG.UI
{
    public class UILogHandler
    {
        UIMsgType _msgType;
        string _originalMessage;
        string[] _messages;

        public int ChoiceCount { get; set; }
        public UILogHandler(UIMsgType msgType, string message, int choiceCount = 0)
        {
            _msgType = msgType;
            _originalMessage = message;
            ChoiceCount = choiceCount;

            if (!HasPlaceholders(message))
                _messages = _originalMessage.Split('\n');
        }
        private bool HasPlaceholders(string text)
        {
            return Regex.IsMatch(text, @"\{\d+\}");
        }
        public void FormatMessage(Func<string, string> formatHandler)
        {
            _messages = formatHandler(_originalMessage).Split('\n');
        }
        public void DisplayMessage()
        {
            // UIManager의 입력 프레임 업데이트 메서드 사용
            Manager.UI.UpdateInputFrame(_messages);
            
            if (_msgType == UIMsgType.MSG_WITH_WAIT)
            {
                // 메시지와 함께 대기하는 경우
                Console.ReadKey(true);
                // 키 입력 후 빈 입력 프레임으로 복원
                Manager.UI.DrawInputFrame();
            }
        }
    }
}
