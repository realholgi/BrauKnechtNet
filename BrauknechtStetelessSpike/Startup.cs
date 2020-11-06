using System;
using BrauknechtStateless;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog.Extensions.Logging;

namespace BrauknechtStatelessSpike
{
    class Startup
    {
        private static readonly Maischprogramm M = new Maischprogramm
        {
            EinmaischTemperatur = 70, 
            Rasten = new[]
            {
                new Rast(66, 60),
                new Rast(72, 30)
            }
        };

        private static readonly Kochprogramm K = new Kochprogramm
        {
            Kochdauer = 60, 
            Hopfengaben = new[]
            {
                new Hopfengabe("Magnum", 50),
                new Hopfengabe("Amarillo", 30),
                new Hopfengabe("Simcoe", 5)
            }
        };
        
        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder().Build();
            var servicesProvider =  RegisterServices(config);
            using (servicesProvider as IDisposable)
            {
                servicesProvider.GetRequiredService<MaischautomatikRunner>().Run(M);
                servicesProvider.GetRequiredService<KochautomatikRunner>().Run(K);
            }
        }
        
        private static IServiceProvider RegisterServices(IConfiguration config)
        {
            return new ServiceCollection()
                .AddTransient<Maischautomatik>()
                .AddTransient<Kochautomatik>()
                .AddTransient<MaischautomatikRunner>()
                .AddTransient<KochautomatikRunner>()
                .AddLogging(loggingBuilder =>
                {
                    loggingBuilder.AddNLog(config);
                })
                .BuildServiceProvider(true);
        }
    }
}