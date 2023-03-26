using System;
using System.IO;
using ChatGPT;
using ChatGPT.Bot;
using ChatGPT.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http;

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
            services.AddScoped<IChatGptProvider, ChatGptProvider>();
        }).Build();
await host.RunAsync();

