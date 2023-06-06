using System.Text.Json.Serialization;

namespace ChatGPT.Models.GPT
{
    public class Choice
    {
        [JsonPropertyName("index")]
        public int Index { get; set; }
        [JsonPropertyName("message")]
        public DialogMessage Message { get; set; } = new();
        [JsonPropertyName("finish_reason")]
        public string FinishReason { get; set; } = "";
    }
}
