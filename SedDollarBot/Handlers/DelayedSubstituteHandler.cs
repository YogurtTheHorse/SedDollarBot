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
            !string.IsNullOrEmpty(message.Text) && _delayedSubstitutions.ListSubstitutes(message.Chat.Id).Length > 0;

        public async Task Handle(Message message, TelegramBotClient bot)
        {
            var applied = false;
            var deleteAfter = false;
            
            string output = _delayedSubstitutions
                .ListSubstitutes(message.Chat.Id)
                .Aggregate(
                    message.Text,
                    (current, s) =>
                    {
                        string newString = Regex.Replace(current, s.Pattern, s.Replacement, s.Options);

                        applied = newString != current;
                        deleteAfter |= s.DeleteAfter;
                        
                        return newString;
                    }
                );

            if (applied)
            {
                await bot.SendTextMessageAsync(
                    message.Chat.Id,
                    output
                );

                if (deleteAfter)
                {
                    await bot.DeleteMessageAsync(message.Chat.Id, message.MessageId);
                }
            }
        }
    }
}