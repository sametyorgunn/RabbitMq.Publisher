using FileCreateWorkerService.Models;
using FileCreateWorkerService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileCreateWorkerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private IServiceProvider provider;
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration configuration = hostContext.Configuration;
                    services.AddDbContext<AdventureWorks2019Context>(opt =>
                    {
                        opt.UseSqlServer(configuration.GetConnectionString("SqlServer"));
                    });
                    IConfiguration Configuration = hostContext.Configuration;
                    services.AddSingleton<RabbitMQClientService>();

                    services.AddSingleton(sp => new ConnectionFactory()
                    {
                        Uri = new Uri(Configuration.GetConnectionString("RabbitMq")),
                        DispatchConsumersAsync = true
                    });
                    services.AddHostedService<Worker>();
                });
    }
}
