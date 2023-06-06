using ChatGPT.Models.User;
using ChatGPT.Providers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatGPT.Data
{
    public class MessageRepository : BaseRepository
    {
        private readonly BotContext _botContext;
        private readonly ILoggerProvider _loggerProvider;

        public MessageRepository(BotContext botContext, ILoggerProvider loggerProvider) : base(botContext, loggerProvider)
        {
            _botContext = botContext;
            _loggerProvider = loggerProvider;
        }

        public async Task CreateMessage(Message message)
        {
            try
            {
                await _botContext.Messages.AddAsync(message);
                await _botContext.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                _loggerProvider.LogError(ex.Message);
            }
        }
    }
}
