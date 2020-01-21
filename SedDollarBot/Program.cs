using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace SedDollarBot
{
    class Program
    {
        static void Main(string[] args)
        {
            IConfigurationRoot configuration = BuildConfiguration();
        private static IConfigurationRoot BuildConfiguration()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.json", optional: true, reloadOnChange: true);

            return builder.Build();
        }
    }
}
