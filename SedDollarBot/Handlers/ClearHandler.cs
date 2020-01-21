using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SedDollarBot.Handlers
{
    public class ClearHandler : IMessageHandler
    {
        public bool IsAcceptable(Message message) => message.Text.ToLower() == "/clear";

        public async Task Handle(Message message, TelegramBotClient bot)
        {
            int deleted = DelayedSubstituteHandler.Clear();

            await bot.SendTextMessageAsync(
                message.Chat.Id,
                $"Deleted {deleted} delayed substitutions.",
                replyToMessageId: message.MessageId
            );
        }
    }
}