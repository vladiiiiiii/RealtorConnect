using RealtorConnect.Models;

namespace RealtorConnect.Services.Interfaces
{
    public interface IChatService
    {
        Task<List<ChatMessage>> GetChatMessagesAsync(int senderId, string senderType, int receiverId, string receiverType);
        Task SendMessageAsync(ChatMessage message, int clientId);
    }
}