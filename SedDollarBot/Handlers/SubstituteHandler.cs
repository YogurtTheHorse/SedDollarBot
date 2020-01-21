using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.Text.RegularExpressions;
using Telegram.Bot.Types.Enums;

namespace SedDollarBot.Handlers
{
    public class SubstituteHandler : IMessageHandler
    {
        public bool IsAcceptable(Message message)
        {
            return message.Type == MessageType.Text && message.Text.StartsWith("s/") && message.ReplyToMessage != null;
        }

        public async Task Handle(Message message, TelegramBotClient bot)
        {
            string input = message.ReplyToMessage.Text;

            string regexs = message.Text.Substring(2);
            string pattern = ReadTillSeparator(regexs);

            if (pattern == regexs)
            {
                await bot.SendTextMessageAsync(
                    message.Chat.Id,
                    "You've forgot replacement regex.",
                    replyToMessageId: message.MessageId
                );
            }
            else
            {
                string leftRegex = regexs.Substring(pattern.Length + 1);
                string replacement = ReadTillSeparator(leftRegex);

                string result = Regex.Replace(input, pattern, replacement);

                await bot.SendTextMessageAsync(
                    message.Chat.Id,
                    result,
                    replyToMessageId: message.MessageId
                );
            }
        }

        private static string ReadTillSeparator(string s)
        {
            for (var i = 0; i < s.Length; i++)
            {
                if (s[i] == '/' && (i == 0 || s[i - 1] != '\\'))
                {
                    return s.Substring(0, i);
                }
            }

            return s;
        }
    }
}