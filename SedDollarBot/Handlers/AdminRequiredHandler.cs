using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SedDollarBot.Handlers
{
    public abstract class AdminRequiredHandler : IMessageHandler
    {
        public abstract bool IsAcceptable(Message message);

        public async Task Handle(Message message, TelegramBotClient bot)
        {
            if (message.Chat.Id == message.From.Id)
            {
                await HandleImplementation(message, bot);
                return;
            }
            
            ChatMember chatMember = await bot.GetChatMemberAsync(message.Chat.Id, message.From.Id);

            if (chatMember.Status == ChatMemberStatus.Administrator || chatMember.Status == ChatMemberStatus.Creator)
            {
                await HandleImplementation(message, bot);
            }
            else
            {
                await bot.SendTextMessageAsync(
                    message.Chat.Id,
                    "Only admin can do that..",
                    replyToMessageId: message.MessageId
                );   
            }
        }

        protected abstract Task HandleImplementation(Message message, TelegramBotClient bot);
    }
}