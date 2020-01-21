using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SedDollarBot.Handlers
{
    public class HelpHandler : IMessageHandler
    {
        public bool IsAcceptable(Message message) => message?.Text?.ToLower()?.StartsWith("/help") ?? false

        public Task Handle(Message message, TelegramBotClient bot) =>
            bot.SendTextMessageAsync(
                message.Chat.Id,
                "Currently bot supports these commands:\n\n" +
                "s/<regex>/<replacement>/[m][a][i]\n" +
                "/clear\n" +
                "/remove nums...\n" +
                "/list",
                replyToMessageId: message.MessageId
            );
    }
}