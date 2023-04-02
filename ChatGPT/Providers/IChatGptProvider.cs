using ChatGPT.Models;

namespace ChatGPT.Providers
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
