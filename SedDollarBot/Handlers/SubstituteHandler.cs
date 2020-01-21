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
            return message.Type == MessageType.Text && message.Text.StartsWith("s/");
        }

        public async Task Handle(Message message, TelegramBotClient bot)
        {
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
                string flags = replacement != leftRegex
                    ? leftRegex.Substring(replacement.Length + 1)
                    : string.Empty;

                RegexOptions options = ToRegexOptions(flags);

                if (flags.Contains("a"))
                {
                    DelayedSubstituteHandler.DelaySubstitute(
                        (pattern, replacement, options)
                    );

                    await bot.SendTextMessageAsync(
                        message.Chat.Id,
                        "Added substitution to delayed (until restart)",
                        replyToMessageId: message.MessageId
                    );
                }
                else if (message.ReplyToMessage != null)
                {
                    string input = message.ReplyToMessage.Text;
                    string result = Regex.Replace(input, pattern, replacement, options);

                    await bot.SendTextMessageAsync(
                        message.Chat.Id,
                        result,
                        replyToMessageId: message.MessageId
                    );
                }
                else
                {
                    await bot.SendTextMessageAsync(
                        message.Chat.Id,
                        "You forgot to reply either 'a' flag",
                        replyToMessageId: message.MessageId
                    );
                }
            }
        }

        private RegexOptions ToRegexOptions(string flags)
        {
            RegexOptions o = default;

            if (flags.Contains("i"))
            {
                o |= RegexOptions.IgnoreCase;
            }

            if (flags.Contains("m"))
            {
                o |= RegexOptions.Multiline;
            }

            return o;
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