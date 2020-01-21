using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SedDollarBot.Handlers
{
    public class DelayedSubstituteHandler : IMessageHandler
    {
        private readonly IDelayedSubstitutions _delayedSubstitutions;

        public DelayedSubstituteHandler(IDelayedSubstitutions delayedSubstitutions)
        {
            _delayedSubstitutions = delayedSubstitutions;
        }

        public bool IsAcceptable(Message message) =>
            !string.IsNullOrEmpty(message.Text) && _delayedSubstitutions.ListSubstitutes().Length > 0;

        public async Task Handle(Message message, TelegramBotClient bot)
        {
            string originalInput = message.Text;
            string output = _delayedSubstitutions
                .ListSubstitutes()
                .Aggregate(
                    message.Text,
                    (current, s) => Regex.Replace(current, s.Pattern, s.Replacement, s.Options)
                );

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