using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ChatGPT.Providers;
using ChatGPT.Models;

namespace ChatGPT.Bot
{

    public class ChatGPTbot
    {
        private ITelegramBotClient? _bot;
        private CancellationTokenSource? _cancellationToken;
        private static IChatGptProvider _chatGptProvider;
        private readonly ILogger<ChatGPTbot> _logger;
        private bool isConnected = true;
        private static List<DialogMessage> messages;

        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="chatGptProvider"></param>
        public ChatGPTbot(ILogger<ChatGPTbot> logger, IChatGptProvider chatGptProvider)
        {
            _logger = logger;
            _chatGptProvider = chatGptProvider;
            messages = new List<DialogMessage>();
        }

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

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                var message = update.Message;
                if (message.Text.ToLower() == "/start")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Добро пожаловать! Просто напиши мне свой вопрос.");
                    return;
                }
                else if(message.Text.ToLower() == "/stop")
                {
                    messages.Clear();
                    await botClient.SendTextMessageAsync(message.Chat, "История сообщений очищена..");
                    return;
                }

                messages.Add(new DialogMessage
                {
                    Role = "user",
                    Content = message.Text
                });
                var gptResponseData = await _chatGptProvider.SendMessageAsync(messages);
                messages.Add(gptResponseData);
                await botClient.SendTextMessageAsync(message.Chat, gptResponseData.Content);
            }
        }

        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            // Некоторые действия
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }

        /// <summary>
        /// Остановка бота
        /// </summary>
        public void StopBot()
        {
            _cancellationToken.Cancel();
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