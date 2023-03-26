using ChatGPT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatGPT.Providers
{
    public interface IChatGptProvider
    {
        Task<DialogMessage> SendMessageAsync(List<DialogMessage> messages);
    }
}
