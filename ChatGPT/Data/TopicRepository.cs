using ChatGPT.Models.User;
using ChatGPT.Providers.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ChatGPT.Data
{
    public class TopicRepository : BaseRepository
    {
        private readonly BotContext _botContext;
        private readonly ILoggerProvider _logger;

        public TopicRepository(BotContext botContext, ILoggerProvider loggerProvider) : base(botContext, loggerProvider)
        {
            _botContext = botContext;
            _logger = loggerProvider;
        }

        public async Task CreateTopicAsync(Topic topic)
        {
            try
            {
                await _botContext.Topics.AddAsync(topic);
                await _botContext.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        public async Task<Topic> GetTopicByPredicate(Expression<Func<Topic, bool>> predicate)
        {
            try
            {
                return await _botContext.Topics.Include(t => t.Messages)
                    .Include(t => t.User)
                    .FirstOrDefaultAsync(predicate);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<List<Topic>> GetTopicsListByPredicate(Expression<Func<Topic, bool>> predicate)
        {
            try
            {
                return await _botContext.Topics.Include(t => t.Messages)
                   .Include(t => t.User)
                   .Where(predicate)
                   .ToListAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }
    }
}
