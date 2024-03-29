﻿using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Polling;
using Telegram.Bot.Exceptions;
using ChatGPT.Models.GPT;
using ChatGPT.Providers.Interfaces;
using Telegram.Bot.Types.ReplyMarkups;

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
        private static IUserProvider _userProvider;
        private static List<DialogMessage> messages;
        private bool isConnected = true;

        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="chatGptProvider"></param>
        public ChatGPTbot(IChatGptProvider chatGptProvider, ILoggerProvider logger, IUserProvider userProvider)
        {
            _chatGptProvider = chatGptProvider;
            _logger = logger;
            _userProvider = userProvider;
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
                    AllowedUpdates = { },
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
                await HandleMessage(botClient, update);
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
        public static async Task HandleMessage(ITelegramBotClient botClient, Update update)
        {
            var message = update.Message.Text.ToLower();

            if (message == "/start")
            {
                await SendStartMessageAsync(botClient, update);
                return;
            }
            else if(message == "/topics")
            {
                await SendTopicsListAsync(botClient, update);
                return;
            }
            else
            {
                if (!messages.Any())
                {
                    messages.Add(new DialogMessage
                    {
                        Role = "user",
                        Content = $"Придумай название темы, одним-двумя словами, для этого сообщения: {message}"
                    });
                    var topicTitle = await _chatGptProvider.SendMessageAsync(messages);

                    await _userProvider.AddTopicToUser(topicTitle.Content, update.Message);
                    return;
                }

                //var gptResponseData = await _chatGptProvider.SendMessageAsync(messages);
                //messages.Add(gptResponseData);
                //await botClient.SendTextMessageAsync(update.Message.Chat, gptResponseData.Content);
                //_logger.LogGPTMessage(gptResponseData.Content);
            }
        }

        private static async Task SendTopicsListAsync(ITelegramBotClient botClient, Update update)
        {
            var userTopics = await _userProvider.GetUserTopics(update.Message);
            if(userTopics == null)
            {
                await botClient.SendTextMessageAsync(update.Message.Chat, $"Не удалось загрузить темы для {update.Message.From.Username}");
                return;
            }

            var keyBoardButtons = new List<List<InlineKeyboardButton>>();
            for (var index = 0; index < userTopics.Count; index++)
            {
                var topicItem = userTopics[index];
                var inlineButtonText = topicItem.Title;
                var inlineButton = new InlineKeyboardButton(inlineButtonText)
                {
                    CallbackData = $"${topicItem.Id}"
                };

                keyBoardButtons.Last().Add(inlineButton);
            }

            var inlineKeyboard = new InlineKeyboardMarkup(keyBoardButtons);
            await botClient.SendTextMessageAsync(update.Message.Chat, "Ваши предыдущие темы:", replyMarkup: inlineKeyboard);
            return;
        } 

        private static async Task SendStartMessageAsync(ITelegramBotClient botClient, Update update)
        {
            var user = await _userProvider.GetTelegramUser(update.Message);

            await botClient.SendTextMessageAsync(user.ChatId,
                $"Добро пожаловать, {user.UserName}! \n" +
                $"Это чат-бот для общения с искусственным интеллектом,\n" +
                $"разработанным компанией OpenAI - ChatGPT v3.5!\n\n" +
                "Чат-бот абсолютно бесплатен и не ограничивается в использовании.\n" +
                "Исходный код бота вы можете посмотреть на моём GitHub\n" +
                "по этой ссылке: https://github.com/Hippukki/ChatGPT\n\n" +
                "Если вы столкнётесь с какими-нибудь проблемами,\n" +
                "либо у вас возникнут неполадки при использовании чат-бота,\n" +
                "вы можете сообщить мне об этом письмом на почту: gregorhey812@gmail.com");

            await botClient.SendTextMessageAsync(user.ChatId, "А теперь просто напиши мне какой-нибудь вопрос!");
            return;
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
