using ChatGPT.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatGPT.Providers.Interfaces
{
    public interface IUserProvider
    {
        Task<TelegramUser> GetTelegramUser(Telegram.Bot.Types.Message message);
    }
}
