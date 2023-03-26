using ChatGPT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ChatGPT.Providers
{
    public class ChatGptProvider : IChatGptProvider
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ChatGptProvider> _logger;
        private readonly string _chatGptToken = "YOUR_API_KEY";
        private readonly string endpoint = "https://api.openai.com/v1/chat/completions";

        public ChatGptProvider(IHttpClientFactory httpClientFactory, ILogger<ChatGptProvider> logger)
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
                ChatGptResponse? responseData = await response.Content.ReadFromJsonAsync<ChatGptResponse>();

                var choices = responseData?.Choices ?? new List<Choice>();
                if (choices.Count == 0)
                {
                    return new DialogMessage
                    {
                        Content = "..."
                    };
                }
                var choice = choices[0];
                var responseMessage = choice.Message;
                return responseMessage;
            }
            catch(Exception ex)
            {
                _logger.LogError($"Произошла ошибка при отправке сообщения ChatGPT API или его получени: {ex.Message}");
                return new DialogMessage { Content = "Произошла ошибка! Пожалуйста попробуйте ещё раз." };
            }
        }
    }
}
