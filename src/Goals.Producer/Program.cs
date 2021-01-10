using Goals.Producer.Config;
using Goals.Producer.Repositories;
using Goals.Producer.Services;
using System;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;


namespace Goals.Producer
{

    public record Goal(string Scorer, bool FirstHalf, int Minute);


    class Program
    {
        static async Task Main(string[] args)
        {
            await Bootstrap(args).RunAsync();
        }


        static IHost Bootstrap(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                var environmentName = context.HostingEnvironment.EnvironmentName.ToLower();

                config.SetBasePath(Directory.GetCurrentDirectory());
                config.AddCommandLine(args);
                config.AddEnvironmentVariables("GOALS__");
                config.AddJsonFile($"appsettings.{environmentName}.json", true);
            })
            .ConfigureServices((context, services) =>
            {
                var footballConfig = context.Configuration.GetSection("footballapi").Get<FootballApiConfig>();
                var kafkaConfig = context.Configuration.GetSection("kafka").Get<KafkaConfig>();

                services.AddSingleton<FootballApiConfig>(footballConfig);
                services.AddSingleton<KafkaConfig>(kafkaConfig);
                services.AddSingleton<FootballDataSource>();
                services.AddSingleton<FootballDataTarget>();
                services.AddHostedService<GamesMonitor>();
            })
            .UseSerilog((context, config) =>
            {
                config
                    .ReadFrom.Configuration(context.Configuration)
                    .MinimumLevel.Information()
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                ;
            })
            .UseConsoleLifetime()
            .Build()
        ;
    }
}
