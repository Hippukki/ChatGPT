using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;

namespace ChatGPT.Models.User
{
    public class TelegramUser
    {
        public Guid Id { get; set; }
        public long ChatId { get; set; }
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public ChatType Type { get; set; }
        public List<Topic>? Topics { get; set; }
        public List<Message>? Messages { get; set; }
    }
}
