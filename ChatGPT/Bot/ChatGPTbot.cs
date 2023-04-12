using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Polling;
using ChatGPT.Providers;
using ChatGPT.Models;
using Telegram.Bot.Exceptions;

namespace ChatGPT.Bot
{
    /// <summary>
    /// Бот
    /// </summary>
    public class ChatGPTbot
    {
        private ITelegramBotClient? _bot;
        private CancellationTokenSource? _cancellationToken;
        private static IChatGptProvider _chatGptProvider;
        private static ILoggerProvider _logger;
        private bool isConnected = true;
        private static List<DialogMessage> messages;

        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="chatGptProvider"></param>
        public ChatGPTbot(IChatGptProvider chatGptProvider, ILoggerProvider logger)
        {
            _chatGptProvider = chatGptProvider;
            messages = new List<DialogMessage>();
            _logger = logger;
        }

        /// <summary>
        /// Инициализация бота
        /// </summary>
        public void InitializeBot()
        {
            try
            {
                _bot = new TelegramBotClient("YOUR_API_KEY");
                _cancellationToken = new CancellationTokenSource();
                var cancellationToken = _cancellationToken.Token;
                var receiverOptions = new ReceiverOptions
                {
                    AllowedUpdates = { }, // receive all update types
                };
                _bot.StartReceiving(
                    HandleUpdateAsync,
                    HandleErrorAsync,
                    receiverOptions,
                    cancellationToken
                );
            }
            catch(Exception ex)
            {
                _logger.LogError($"Произошла ошибка при инициализации бота: {ex.Message}");
            }
        }

        /// <summary>
        /// Обработка обновлений
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="update"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            _logger.LogTelegramMessage(update);
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                await SendMessage(botClient, update);
            }
        }

        /// <summary>
        /// Обработка ошибок
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="exception"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is ApiRequestException)
            {
                _logger.LogError($"Ошибка при отправке запроса к Телеграм API. {exception.Message}");
            }
            else
            {
                _logger.LogError($"Неизвестная ошибка: {exception.Message}");
            }
        }

        /// <summary>
        /// Остановка бота
        /// </summary>
        public void StopBot()
        {
            _cancellationToken.Cancel();
        }

        /// <summary>
        /// Метод обработки сообщений
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="update"></param>
        /// <returns></returns>
        public static async Task SendMessage(ITelegramBotClient botClient, Update update)
        {
            var message = update.Message.Text.ToLower();
            if (message == "/start")
            {
                await botClient.SendTextMessageAsync(update.Message.Chat,
                    "Добро пожаловать! Это чат-бот для общения с искусственным интеллектом, разработанным компанией OpenAI - ChatGPT v3.5! \n" +
                    "Чат-бот абсолютно бесплатен и не ограничивается в использовании. \n" +
                    "Исходный код бота вы можете посмотреть на моём GitHub по этой ссылке: https://github.com/Hippukki/ChatGPT \n" +
                    "Если вы столкнётесь с какими-нибудь проблемами, либо у вас возникнут неполадки при использовании чат-бота, вы можете сообщить мне об этом письмом на почту: gregorhey812@gmail.com");
                await botClient.SendTextMessageAsync(update.Message.Chat, "А теперь просто напиши мне какой-нибудь вопрос!");
                return;
            }
            else if (message == "/clear")
            {
                messages.Clear();
                await botClient.SendTextMessageAsync(update.Message.Chat, "Тема диалога очищена, но вы можете задать мне другой вопрос!");
                return;
            }

            messages.Add(new DialogMessage
            {
                Role = "user",
                Content = message
            });
            var gptResponseData = await _chatGptProvider.SendMessageAsync(messages);
            messages.Add(gptResponseData);
            await botClient.SendTextMessageAsync(update.Message.Chat, gptResponseData.Content);
            _logger.LogGPTMessage(gptResponseData.Content);
        }

        /// <summary>
        /// Проверка соединения с Телеграм API
        /// </summary>
        /// <returns></returns>
        public async Task CheckConnection()
        {
            try
            {
                await _bot.TestApiAsync();
                if (!isConnected)
                {
                    _logger.LogInformation("Соединение с Телеграм API восстановлено");
                    InitializeBot();
                    isConnected = true;
                }
            }
            catch (Exception ex)
            {
                if (isConnected)
                {
                    _logger.LogError($"{ex.Message}: Не удалось подключиться к Телеграм API, возможно отсутствует соединение с интернетом");
                    StopBot();
                    isConnected = false;
                }
            }
        }
    }
}