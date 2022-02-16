using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace ClaimsBasedIdentity.Web.UI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Get the program version number from the Assembly
            Dictionary<string, string> newVariables = new Dictionary<string, string>();
            string version = Assembly.GetEntryAssembly()?
                                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                                .InformationalVersion
                                .ToString();
            newVariables.Add("Version", version);

            var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile(path: "appsettings.json", optional: false, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("ApplicationName", typeof(Program).Assembly.GetName().Name)
                .CreateLogger();

            try
            {
                Log.Information("Starting Claims Based Identity");
                CreateHostBuilder(args, newVariables).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        // Create a lambda to build the Host
        public static IHostBuilder CreateHostBuilder(string[] args, Dictionary<string, string> newVariables) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureAppConfiguration(config =>
                {
                    config.AddInMemoryCollection(newVariables);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseContentRoot(Directory.GetCurrentDirectory())
                        //.UseKestrel()
                        //.UseIISIntegration()
                        .UseStartup<Startup>();
                });
    }
}
