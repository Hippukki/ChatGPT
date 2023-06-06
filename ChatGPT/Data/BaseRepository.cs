using ChatGPT.Providers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatGPT.Data
{
    public abstract class BaseRepository
    {
        private readonly BotContext _botContext;
        private readonly ILoggerProvider _logger;

        public BaseRepository(BotContext botContext, ILoggerProvider loggerProvider)
        {
            _botContext = botContext;
            _logger = loggerProvider;
        }

        public async Task UpdateAsync()
        {
            try
            {
                await _botContext.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }
}
