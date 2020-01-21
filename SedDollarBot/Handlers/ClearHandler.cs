using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SedDollarBot.Handlers
{
    public class ClearHandler : IMessageHandler
    {
        public bool IsAcceptable(Message message) => message.Text.ToLower().StartsWith("/clear");

        public async Task Handle(Message message, TelegramBotClient bot)
        {
            ChatMember chatMember = await bot.GetChatMemberAsync(message.Chat.Id, message.From.Id);

            if (chatMember.Status == ChatMemberStatus.Administrator || chatMember.Status == ChatMemberStatus.Creator)
            {
                int deleted = DelayedSubstituteHandler.Clear();
                
                await bot.SendTextMessageAsync(
                    message.Chat.Id,
                    $"Deleted {deleted} delayed substitutions.",
                    replyToMessageId: message.MessageId
                );
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
    }
}