using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMq.ExcelCreate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitMq.ExcelCreate
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var appdbcontext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var usermanager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                appdbcontext.Database.Migrate();
                if (!appdbcontext.Users.Any())
                {
                    usermanager.CreateAsync(new IdentityUser()
                    {
                        UserName = "samet",
                        Email = "asametyorgun60@gmail.com"
                    }, "Yorgun.1292").Wait();

                    usermanager.CreateAsync(new IdentityUser()
                    {
                        UserName = "serkan",
                        Email = "serkankaradag@gmail.com"
                    }, "Yorgun.1292").Wait();
                }
            }




            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
