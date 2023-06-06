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
    public class UserRepository : BaseRepository
    {
        private readonly BotContext _botContext;
        private readonly ILoggerProvider _logger;

        public UserRepository(BotContext botContext, ILoggerProvider loggerProvider) : base(botContext, loggerProvider)
        {
            _botContext = botContext;
            _logger = loggerProvider;
        }

        public async Task CreateUserAsync(TelegramUser user)
        {
            try
            {
                await _botContext.Users.AddAsync(user);
                await _botContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        public async Task<TelegramUser> GetUserByPredicate(Expression<Func<TelegramUser, bool>> predicate)
        {
            try
            {
                return await _botContext.Users.Include(u => u.Topics)
                .ThenInclude(t => t.Messages)
                .FirstOrDefaultAsync(predicate);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<List<TelegramUser>> GetUsersListByPredicate(Expression<Func<TelegramUser, bool>> predicate)
        {
            try
            {
                return await _botContext.Users.Include(u => u.Topics)
                    .ThenInclude(t => t.Messages)
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
