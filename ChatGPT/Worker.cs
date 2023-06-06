using ChatGPT.Bot;
using ChatGPT.Providers.Interfaces;
using Microsoft.Extensions.Hosting;

namespace ChatGPT
{
    /// <summary>
    /// Фоновый процесс
    /// </summary>
    public class Worker : BackgroundService
    {
        private readonly ILoggerProvider _logger;
        private readonly ChatGPTbot _chatGPTbot;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="expeditorBot"></param>
        /// <param name="httpClientProvider"></param>
        public Worker(ILoggerProvider logger, ChatGPTbot chatGPTbot)
        {
            _logger = logger;
            _chatGPTbot = chatGPTbot;
            DoWorkAsyncInfiniteLoop();

        }

        /// <summary>
        /// Запуск приложения
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Инициализация");
        }

        /// <summary>
        /// Старт службы
        /// </summary>
        /// <returns></returns>
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Старт службы...");
            _chatGPTbot.InitializeBot();
        }

        /// <summary>
        /// Остановка службы
        /// </summary>
        /// <returns></returns>
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Остановка службы...");
            _chatGPTbot.StopBot();
        }

        /// <summary>
        /// Фоновый процесс
        /// </summary>
        /// <returns></returns>
        private async Task DoWorkAsyncInfiniteLoop()
        {
            const int pause = 60000;
            while (true)
            {
                await Task.Delay(pause);
                try
                {
                    _logger.LogInformation("Проверка соединения...");
                    await _chatGPTbot.CheckConnection();
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception.Message);
                }
            }
        }
    }
}
