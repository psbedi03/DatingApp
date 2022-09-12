using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            //we can call the seed class method in Main
            //what goes inside here in Main happens before the application is started
            var host = CreateHostBuilder(args).Build();
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            
            //exception handling to catch exceptions that we can get while seeding our data
            try{
                var context = services.GetRequiredService<DataContext>();
                
                /***
                    rather than dotnet ef update command 
                    we can use MigrateAsync, 
                    it will apply any pending migrations 
                    and if the database doesn't exist, it will create db
                **/
                await context.Database.MigrateAsync();
                await Seed.SeedUsers(context);

            }catch(Exception ex){
                //to log errors on terminal
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An Error occured during migration");
            }

            //after everything is done, we need to run the Run() that we removed earlier
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
