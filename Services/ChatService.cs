using Microsoft.EntityFrameworkCore;
using RealtorConnect.Data;
using RealtorConnect.Models;
using RealtorConnect.Repositories.Interfaces;
using RealtorConnect.Services.Interfaces;

namespace RealtorConnect.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;
        private readonly ApplicationDbContext _context;

        public ChatService(IChatRepository chatRepository, ApplicationDbContext context)
        {
            _chatRepository = chatRepository;
            _context = context;
        }

        public async Task<List<ChatMessage>> GetChatMessagesAsync(int senderId, string senderType, int receiverId, string receiverType)
        {
            return await _chatRepository.GetChatMessagesAsync(senderId, senderType, receiverId, receiverType);
        }

        public async Task SendMessageAsync(ChatMessage message, int clientId)
        {
            // Проверяем, что отправитель — клиент и его ID совпадает с ID авторизованного пользователя
            if (message.SenderType != "Client" || message.SenderId != clientId)
                throw new UnauthorizedAccessException("You can only send messages from your own account as a client");

            // Проверяем, что получатель — риэлтор
            if (message.ReceiverType != "Realtor")
                throw new ArgumentException("Messages can only be sent to realtors");

            // Проверяем, существует ли риэлтор
            var realtor = await _context.Realtors.FindAsync(message.ReceiverId);
            if (realtor == null)
                throw new KeyNotFoundException("Realtor not found");

            // Проверяем, существует ли клиент (дополнительная проверка)
            var client = await _context.Clients.FindAsync(message.SenderId);
            if (client == null)
                throw new KeyNotFoundException("Client not found");

        

            // Устанавливаем время отправки
            message.SentAt = DateTime.UtcNow;

            // Сохраняем сообщение
            await _chatRepository.AddMessageAsync(message);
        }
    }
}