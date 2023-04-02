using ChatGPT;
using ChatGPT.Bot;
using ChatGPT.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog.Web;

var configuration = new ConfigurationBuilder()
    .AddJsonFile($"appsettings.json", true, true)
    .AddEnvironmentVariables()
    .Build();
var host = Host.CreateDefaultBuilder(args)
        .ConfigureServices(services =>
        {
            services.AddHttpClient();
            services.AddHostedService<Worker>();
            services.AddTransient(typeof(ChatGPTbot));
            services.AddSingleton(configuration);
            services.AddTransient<ILoggerProvider, LoggerProvider>();
            services.AddScoped<IChatGptProvider, ChatGptProvider>();
        }).ConfigureLogging(logBuilder => { logBuilder.AddNLog("NLog.config"); })
        .Build();
await host.RunAsync();

