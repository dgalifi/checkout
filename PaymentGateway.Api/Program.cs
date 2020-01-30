using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace PaymentGateway.Api
{
    public class Program
    {
        public static string HostUrl { get; set; }
        public static IConfigurationRoot Configuration { get; set; }

        public static void Main(string[] args)
        {
            Configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json")
               .AddEnvironmentVariables()
               .AddCommandLine(args)
               .Build();
            //var url =;

            HostUrl = Configuration["url"];

            if (string.IsNullOrEmpty(HostUrl))
                HostUrl = $"http://{Configuration["hostUrl"]}:{Configuration["hostPort"]}";

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>().UseUrls(HostUrl);
                });
    }
}
