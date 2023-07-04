using ChatGPT.Models.GPT;
using ChatGPT.Providers.Interfaces;
using System.Net;
using System.Net.Http.Json;


namespace ChatGPT.Providers
{
    public class ChatGptProvider : IChatGptProvider
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILoggerProvider _logger;
        private readonly string _chatGptToken = "YOR_API_TOKEN";
        private readonly string endpoint = "https://api.openai.com/v1/chat/completions";

        public ChatGptProvider(IHttpClientFactory httpClientFactory, ILoggerProvider logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<DialogMessage> SendMessageAsync(List<DialogMessage> messages)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_chatGptToken}");
                var requestData = new ChatGptRequest()
                {
                    ModelId = "gpt-3.5-turbo",
                    Messages = messages
                };
                var response = await httpClient.PostAsJsonAsync(endpoint, requestData);
                if(response.StatusCode is HttpStatusCode.BadRequest)
                {
                    _logger.LogError($"Ошибка при отправке запроса к ChatGPT API, статус код: {response.StatusCode}");
                    return new DialogMessage { Content = "Произошла ошибка! Пожалуйста попробуйте позже или обратитесь в поддержку." };
                }

                ChatGptResponse? responseData = await response.Content.ReadFromJsonAsync<ChatGptResponse>();
                var choices = responseData?.Choices ?? new List<Choice>();
                var choice = choices[0];
                var responseMessage = choice.Message;
                return responseMessage;
            }
            catch(Exception ex)
            {
                _logger.LogError($"Неизвестная ошибка при отправке запроса к ChatGPT API: {ex.Message}");
                return new DialogMessage { Content = "Произошла ошибка! Пожалуйста попробуйте ещё раз." };
            }
        }
    }
}
