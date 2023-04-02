using System.Text.Json.Serialization;

namespace ChatGPT.Models
{
    public class ChatGptRequest
    {
        [JsonPropertyName("model")]
        public string ModelId { get; set; } = "";
        [JsonPropertyName("messages")]
        public List<DialogMessage> Messages { get; set; } = new();
    }
}
