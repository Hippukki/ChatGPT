using ChatGPT.Data;
using ChatGPT.Models.User;
using ChatGPT.Providers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;

namespace ChatGPT.Providers
{
    public class UserProvider : IUserProvider
    {
        private readonly UserRepository _userRepository;
        private readonly TopicRepository _topicRepository;
        private readonly MessageRepository _messageRepository;
        private readonly ILoggerProvider _loggerProvider;

        public UserProvider(UserRepository userRepository, TopicRepository topicRepository, MessageRepository messageRepository, ILoggerProvider loggerProvider)
        {
            _userRepository = userRepository;
            _topicRepository = topicRepository;
            _messageRepository = messageRepository;
            _loggerProvider = loggerProvider;
        }

        public async Task<TelegramUser> GetTelegramUser(Telegram.Bot.Types.Message message)
        {
            var username = message.From.Username;
            var chatId = message.Chat.Id;

            var userByUserName = await _userRepository.GetUserByPredicate(u => u.UserName == username);
            if(userByUserName == null)
            {
                var userByChat = await _userRepository.GetUserByPredicate(u => u.ChatId == chatId);
                if(userByChat != null)
                {
                    if (username != null)
                    {
                        userByChat.UserName = username;
                        await _userRepository.UpdateAsync();
                    }

                    return userByChat;
                }

                return await AddTelegramUser(message);
            }

            return userByUserName;
        }

        public async Task AddTopicToUser(string title, Telegram.Bot.Types.Message message)
        {
            var user = await GetTelegramUser(message);

            var firstMessage = new Message
            {
                Id = Guid.NewGuid(),
                User = user,
                Content = message.Text,
                Role = "user",
                SentDate = DateTime.Now
            };

            var newTopic = new Topic
            {
                Id = Guid.NewGuid(),
                Title = title,
                User = user,
                Messages = new List<Message>
                {
                    firstMessage
                },
                IsDeleted = false
            };

            await _topicRepository.CreateTopicAsync(newTopic);
            return;
        }

        public async Task<List<Topic>> GetUserTopics(Telegram.Bot.Types.Message message)
        {
            var chatId = message.Chat.Id;
            return await _topicRepository.GetTopicsListByPredicate(t => t.User.ChatId == chatId);
        }

        private async Task<TelegramUser> AddTelegramUser(Telegram.Bot.Types.Message message)
        {
            if(message.Chat.Type == ChatType.Private)
            {
                var newUser = new TelegramUser
                {
                    Id = Guid.NewGuid(),
                    ChatId = message.Chat.Id,
                    UserName = message.From.Username,
                    FirstName = message.From.FirstName ?? String.Empty,
                    LastName = message.From.LastName ?? String.Empty,
                    Type = message.Chat.Type,
                };

                await _userRepository.CreateUserAsync(newUser);
                return await _userRepository.GetUserByPredicate(u => u.Id == newUser.Id);
            }

            _loggerProvider.LogError($"Невозможно создать пользователя типа {message.Chat.Type}");
            return null;
        }
    }
}
