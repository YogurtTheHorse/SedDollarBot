using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MihaZupan;
using SedDollarBot.Handlers;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;

namespace SedDollarBot
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            IConfigurationRoot configuration = BuildConfiguration();

            IConfigurationSection proxyConfiguration = configuration.GetSection("Bot:Proxy");
            string token = configuration.GetSection("Bot:Token").Value;

            TelegramBotClient bot = null;

            if (proxyConfiguration.Exists())
            {
                var proxy = new HttpToSocks5Proxy(
                    proxyConfiguration["Host"],
                    int.Parse(proxyConfiguration["Port"]),
                    proxyConfiguration["User"],
                    proxyConfiguration["Password"]
                );
                proxy.ResolveHostnamesLocally = true;

                bot = new TelegramBotClient(token, proxy);
            }
            else
            {
                bot = new TelegramBotClient(token);
            }


            User botInfo = await bot.GetMeAsync();

            Console.Title = botInfo.Username;

            var cancellationTokenSource = new CancellationTokenSource();
            var handler = new UpdateHandler(bot, new IMessageHandler[]
            {
                new SubstituteHandler(),
                new ClearHandler(),
                new DelayedSubstituteHandler(),
            });

            bot.StartReceiving(handler, cancellationTokenSource.Token);

            Console.WriteLine($"Start listening for @{botInfo.Username}");
            Console.ReadLine();

            cancellationTokenSource.Cancel();
        }

        private static IConfigurationRoot BuildConfiguration()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.json", optional: true, reloadOnChange: true);

            return builder.Build();
        }
    }
}