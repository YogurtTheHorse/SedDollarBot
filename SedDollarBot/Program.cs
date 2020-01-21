using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
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

            TelegramBotClient bot = SetupBot(proxyConfiguration, token);
            
            User botInfo = await bot.GetMeAsync();
            Console.Title = botInfo.Username;

            var cancellationTokenSource = new CancellationTokenSource();
            
            var iocContainerBuilder = new ContainerBuilder();

            iocContainerBuilder
                .RegisterType<InMemoryDelayedSubstitutions>()
                .As<IDelayedSubstitutions>()
                .SingleInstance();
            
            iocContainerBuilder.RegisterInstance(bot).As<TelegramBotClient>();
            iocContainerBuilder.RegisterType<UpdateHandler>().As<IUpdateHandler>();
            
            iocContainerBuilder.RegisterType<SubstituteHandler>().As<IMessageHandler>();
            iocContainerBuilder.RegisterType<ListSubstitutionsHandler>().As<IMessageHandler>().PreserveExistingDefaults();
            iocContainerBuilder.RegisterType<ClearHandler>().As<IMessageHandler>().PreserveExistingDefaults();
            iocContainerBuilder.RegisterType<DelayedSubstituteHandler>().As<IMessageHandler>().PreserveExistingDefaults();

            IContainer container = iocContainerBuilder.Build();

            using (ILifetimeScope scope = container.BeginLifetimeScope())
            {
                bot.StartReceiving(scope.Resolve<IUpdateHandler>(), cancellationTokenSource.Token);

                Console.WriteLine($"Start listening for @{botInfo.Username}");
                Console.ReadLine();
            }

            cancellationTokenSource.Cancel();
        }

        private static TelegramBotClient SetupBot(IConfigurationSection proxyConfiguration, string token)
        {
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

            return bot;
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