using Telegram.Bot.Types;

namespace ChatGPT.Providers.Interfaces
{
    /// <summary>
    /// Логер
    /// </summary>
    public interface ILoggerProvider
    {
        /// <summary>
        /// Лог сообщения полученного из Телеграма
        /// </summary>
        /// <param name="update"></param>
        void LogTelegramMessage(Update update);

        /// <summary>
        /// Лог сообщения полученного от ChatGPT
        /// </summary>
        /// <param name="message"></param>
        void LogGPTMessage(string message);

        /// <summary>
        /// Лог ошибки
        /// </summary>
        /// <param name="message"></param>
        void LogError(string message);

        /// <summary>
        /// Лог информации
        /// </summary>
        /// <param name="message"></param>
        void LogInformation(string message);
    }
}
