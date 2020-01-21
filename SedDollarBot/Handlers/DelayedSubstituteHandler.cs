using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SedDollarBot.Handlers
{
    public class DelayedSubstituteHandler : IMessageHandler
    {
        // ha-ha databases...
        private static readonly List<(string, string, RegexOptions)> _regexsToHandle = new List<(string, string, RegexOptions)>();

        // ha-ha thread-safety...
        public static void DelaySubstitute((string, string, RegexOptions) substitute)
        {
            _regexsToHandle.Add(substitute);
        }

        // ha-ha!
        public static int Clear()
        {
            int deleted = _regexsToHandle.Count;
            _regexsToHandle.Clear();
            
            return deleted;
        }

        public bool IsAcceptable(Message message) => !string.IsNullOrEmpty(message.Text) && _regexsToHandle.Count > 0;

        public async Task Handle(Message message, TelegramBotClient bot)
        {
            string originalInput = message.Text;
            string output = message.Text;

            foreach ((string pattern, string replacement, RegexOptions options) in _regexsToHandle)
            {
                output = Regex.Replace(output, pattern, replacement, options);
            }

            if (output != originalInput)
            {
                await bot.SendTextMessageAsync(
                    message.Chat.Id,
                    output
                );
            }
        }
    }
}