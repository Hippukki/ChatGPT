using ChatGPT;
using ChatGPT.Bot;
using ChatGPT.Data;
using ChatGPT.Providers;
using ChatGPT.Providers.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog.Web;

namespace ChatGPT
{
    public class Program
    {
        public static IConfiguration Configuration { get; private set; }

        public static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json", true, true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();

            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    var connectionString = Configuration.GetConnectionString("DefaultConnection");
                    var serverVersion = new MySqlServerVersion(new Version(8, 0, 27));
                    services.AddDbContext<BotContext>(
                    dbContextOptions => dbContextOptions
                        .UseMySql(connectionString, serverVersion));

                    services.AddHttpClient();
                    services.AddHostedService<Worker>();
                    services.AddTransient(typeof(ChatGPTbot));
                    services.AddTransient<ILoggerProvider, LoggerProvider>();
                    services.AddScoped<IChatGptProvider, ChatGptProvider>();
                    services.AddScoped<MessageRepository>();
                    services.AddScoped<TopicRepository>();
                    services.AddScoped<UserRepository>();
                    services.AddScoped<IUserProvider, UserProvider>();
                }).ConfigureLogging(logBuilder => { logBuilder.AddNLog("NLog.config"); })
                .Build();

            host.Run();
        }
    }
}

