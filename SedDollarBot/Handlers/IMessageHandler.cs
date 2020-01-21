using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SedDollarBot.Handlers
{
    public interface IMessageHandler
    {
        bool IsAcceptable(Message message);

        Task Handle(Message message, TelegramBotClient bot);
    }
}