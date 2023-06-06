using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatGPT.Models.User
{
    public class Topic
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public List<Message>? Messages { get; set; }
        public TelegramUser? User { get; set; }
        public bool IsDeleted { get; set; }
    }
}
