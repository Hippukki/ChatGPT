using NLog;
using Telegram.Bot.Types;

namespace ChatGPT.Providers
{
    public class LoggerProvider : ILoggerProvider
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public void LogError(string message)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            logger.Error(message);
            Console.ResetColor();
        }

        public void LogInformation(string message)
        {
            Console.BackgroundColor = ConsoleColor.Green;
            logger.Info(message);
            Console.ResetColor();
        }

        public void LogTelegramMessage(Update update)
        {
            //Your custom implemention
        }

        public void LogGPTMessage(string message)
        {
            //Your custom implemention
        }
    }
}
