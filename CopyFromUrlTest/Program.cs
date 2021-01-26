using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.Extensibility.EventCounterCollector;

namespace CopyFromUrlTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddApplicationInsightsTelemetryWorkerService(
                            hostContext.Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"]);

                    var config = new Config();
                    hostContext.Configuration.GetSection("Config").Bind(config);
                    config.Run = Guid.NewGuid().ToString();

                    if (config.Source.Contains("list"))
                    {
                        config.Source = config.Sources.Split("|").ToList();
                    }

                    services.AddSingleton(config);

                    services.AddSingleton<SourceService>();
                    services.AddSingleton<CopyBlobService>();

                    services.AddHostedService<Worker>();
                });
    }
}
