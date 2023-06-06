using ChatGPT.Models.GPT;

namespace ChatGPT.Providers.Interfaces
{
    /// <summary>
    /// Провайдер взаимодействия с ChatGPT
    /// </summary>
    public interface IChatGptProvider
    {
        /// <summary>
        /// Метод отправки сообщений к Open AI API
        /// </summary>
        /// <param name="messages"></param>
        /// <returns></returns>
        Task<DialogMessage> SendMessageAsync(List<DialogMessage> messages);
    }
}
