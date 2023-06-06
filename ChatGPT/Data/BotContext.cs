using ChatGPT.Models.User;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatGPT.Data
{
    public class BotContext : DbContext
    {
        public DbSet<TelegramUser> Users { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Message> Messages { get; set; }

        public BotContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
