using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SedDollarBot.Handlers
{
    public class RemoveSubstitutionsHandler : AdminRequiredHandler
    {
        private readonly IDelayedSubstitutions _delayedSubstitutions;

        public RemoveSubstitutionsHandler(IDelayedSubstitutions delayedSubstitutions)
        {
            _delayedSubstitutions = delayedSubstitutions;
        }

        public override bool IsAcceptable(Message message) => message.Text.ToLower().StartsWith("/remove");

        protected override async Task HandleImplementation(Message message, TelegramBotClient bot)
        {
            string[] input = message.Text?.Split() ?? Array.Empty<string>();

            if (input.Length == 1)
            {
                await bot.SendTextMessageAsync(
                    message.Chat.Id,
                    "Seems like you forgot to list ids of substitutions from /list",
                    replyToMessageId: message.MessageId
                );
            }
            else
            {
                var toDelete = new List<int>();
                int substitutionsCount = _delayedSubstitutions.ListSubstitutes().Length;

                if (substitutionsCount == 0)
                {
                    await bot.SendTextMessageAsync(
                        message.Chat.Id,
                        "No substitutions delayed, sorry.",
                        replyToMessageId: message.MessageId
                    );
                            
                    return;
                }

                foreach (string sid in input.Skip(1))
                {
                    if (int.TryParse(sid, out int id))
                    {
                        if (id > substitutionsCount || id <= 0)
                        {
                            await bot.SendTextMessageAsync(
                                message.Chat.Id,
                                $"Use whole numbers in range [1, {substitutionsCount}]. Aborting.",
                                replyToMessageId: message.MessageId
                            );
                            
                            return;
                        }   
                        
                        toDelete.Add(id -1);
                    }
                    else
                    {
                        await bot.SendTextMessageAsync(
                            message.Chat.Id,
                            $"Can't understand \"{sid}\" use only positive whole numbers. Aborting",
                            replyToMessageId: message.MessageId
                        );
                        
                        return;
                    }
                }

                toDelete.Sort();
                toDelete.Reverse();

                foreach (int deletionIndex in toDelete)
                {
                    _delayedSubstitutions.RemoveAt(deletionIndex);
                }

                string indexes = string.Join(", ", toDelete.Select(i => (i + 1).ToString()));
                
                await bot.SendTextMessageAsync(
                    message.Chat.Id,
                    $"Deleted indexes are {indexes}",
                    replyToMessageId: message.MessageId
                );
            }
        }
    }
}