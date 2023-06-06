using ChatGPT.Providers.Interfaces;
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
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            logger.Info($"------------------------------------------ \n" +
                $"Получено сообщение! \n" +
                $"------------------------------------------ \n" +
                $"Имя: {update.Message.From.FirstName} \n" +
                $"Фамилия: {update.Message.From.LastName} \n" +
                $"Username: {update.Message.From.Username} \n" +
                $"Отправлено: {update.Message.Date} \n" +
                $"Тип чата: {update.Message.Chat.Type} \n" +
                $"Текст сообщения: {update.Message.Text} \n" +
                $"------------------------------------------");
            Console.ResetColor();
        }

        public void LogGPTMessage(string message)
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            logger.Info($"ChatGPT: \n" +
                $"------------------------------------------ \n" +
                $"Ответ: {message} \n" +
                $"------------------------------------------");
            Console.ResetColor();
        }
    }
}
