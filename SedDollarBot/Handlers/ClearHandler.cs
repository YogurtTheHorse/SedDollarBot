using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SedDollarBot.Handlers
{
    public class ClearHandler : AdminRequiredHandler
    {
        private readonly IDelayedSubstitutions _delayedSubstitutions;

        public ClearHandler(IDelayedSubstitutions delayedSubstitutions)
        {
            _delayedSubstitutions = delayedSubstitutions;
        }

        public override bool IsAcceptable(Message message) => message.Text.ToLower().StartsWith("/clear");

        protected override async Task HandleImplementation(Message message, TelegramBotClient bot)
        {
            int deleted = _delayedSubstitutions.Clear();

            await bot.SendTextMessageAsync(
                message.Chat.Id,
                $"Deleted {deleted} delayed substitutions.",
                replyToMessageId: message.MessageId
            );
        }
    }
}