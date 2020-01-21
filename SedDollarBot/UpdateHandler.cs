using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SedDollarBot.Handlers;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SedDollarBot
{
    public class UpdateHandler : IUpdateHandler
    {
        private readonly TelegramBotClient _bot;
        private readonly IMessageHandler[] _messageHandlers;

        public UpdateHandler(TelegramBotClient bot, IEnumerable<IMessageHandler> messageHandlers)
        {
            _bot = bot;
            _messageHandlers = messageHandlers.ToArray();
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

        public async Task HandleUpdate(Update update, CancellationToken cancellationToken)
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
                await HandleError(exception, cancellationToken);
            }
        }

        public async Task HandleError(Exception exception, CancellationToken cancellationToken)
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

        public UpdateType[] AllowedUpdates => new[]
        {
            UpdateType.Message,
            UpdateType.EditedMessage,
            UpdateType.Unknown
        };
    }
}