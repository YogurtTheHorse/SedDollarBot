using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SedDollarBot.Handlers
{
    public class ListSubstitutionsHandler : IMessageHandler
    {
        private readonly IDelayedSubstitutions _delayedSubstitutions;

        public ListSubstitutionsHandler(IDelayedSubstitutions delayedSubstitutions)
        {
            _delayedSubstitutions = delayedSubstitutions;
        }

        public bool IsAcceptable(Message message) => message.Text?.StartsWith("/list") ?? false;

        public async Task Handle(Message message, TelegramBotClient bot)
        {
            Substitution[] substitutions = _delayedSubstitutions.ListSubstitutes();

            if (substitutions.Length == 0)
            {
                await bot.SendTextMessageAsync(
                    message.Chat.Id,
                    "Currently there is no delayed substitutions.",
                    replyToMessageId: message.MessageId
                );
            }
            else
            {
                string text = $"Currently bot contains {substitutions.Length} substitutions:\n\n";
                text += string.Join("\n",
                    substitutions.Select((s, i) => 
                            $"{i + 1}. {s}"
                        )
                );
                
                await bot.SendTextMessageAsync(
                    message.Chat.Id,
                    text,
                    replyToMessageId: message.MessageId
                );
            }
        }
    }
}