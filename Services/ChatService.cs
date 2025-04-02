using RealtorConnect.Models;
using RealtorConnect.Repositories.Interfaces;

namespace RealtorConnect.Services
{
    public class ChatService
    {
        private readonly IChatRepository _repository;

        public ChatService(IChatRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ChatMessage>> GetChatMessagesAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<ChatMessage> GetChatMessageByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task AddChatMessageAsync(ChatMessage chatMessage)
        {
            chatMessage.SentAt = DateTime.UtcNow;
            await _repository.AddAsync(chatMessage);
        }

        public async Task UpdateChatMessageAsync(ChatMessage chatMessage)
        {
            await _repository.UpdateAsync(chatMessage);
        }

        public async Task DeleteChatMessageAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}