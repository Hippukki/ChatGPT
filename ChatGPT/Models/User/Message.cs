using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatGPT.Models.User
{
    public class Message
    {
        public Guid Id { get; set; }
        public TelegramUser? User { get; set; }
        public Topic? Topic { get; set; }
        public string? Content { get; set; }
        public string? Role { get; set; }
        public DateTime SentDate { get; set; }
    }
}
