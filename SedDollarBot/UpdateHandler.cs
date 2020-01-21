using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SedDollarBot.Handlers;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace SedDollarBot
{
    public class UpdateHandler
    {
        private readonly TelegramBotClient _bot;
        private readonly IMessageHandler[] _messageHandlers;

        public UpdateHandler(TelegramBotClient bot, IMessageHandler[] messageHandlers)
        {
            _bot = bot;
            _messageHandlers = messageHandlers;
        }

        public async Task HandleErrorAsync(Exception exception, CancellationToken cancellationToken)
        {
            string errorMessage = exception switch
            {
                ApiRequestException apiRequestException => "Telegram API Error:\n" +
                                                           $"[{apiRequestException.ErrorCode}]\n" +
                                                           $"{apiRequestException.Message}",

                _ => exception.ToString()
            };

            await Console.Error.WriteLineAsync(errorMessage);
        }


        public async Task HandleUpdateAsync(Update update, CancellationToken cancellationToken)
        {
            Task handler = update.Type switch
            {
                UpdateType.Message => BotOnMessageReceived(update.Message),
                UpdateType.EditedMessage => BotOnMessageReceived(update.Message),
                
                _ => UnknownUpdateHandlerAsync(update)
            };

            try
            {
                await handler;
            }
            catch (Exception exception)
            {
                await HandleErrorAsync(exception, cancellationToken);
            }
        }

        private async Task BotOnMessageReceived(Message message)
        {
            foreach (IMessageHandler handler in _messageHandlers)
            {
                if (handler.IsAcceptable(message))
                {
                    await handler.Handle(message, _bot);
                    break;
                }
            }
        }

        private Task UnknownUpdateHandlerAsync(Update update)
        {
            Console.WriteLine($"Unknown update type: {update.Type}");

            return Task.CompletedTask;
        }

    }
}