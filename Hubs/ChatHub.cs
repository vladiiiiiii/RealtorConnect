using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace RealtorConnect.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(int senderId, string senderType, int receiverId, string receiverType, string messageContent)
        {
            string groupName = GetChatGroupName(senderId, receiverId);
            await Clients.Group(groupName).SendAsync("ReceiveMessage", senderId, senderType, receiverId, receiverType, messageContent);
        }

        public async Task JoinChat(int userId, int otherUserId)
        {
            string groupName = GetChatGroupName(userId, otherUserId);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task LeaveChat(int userId, int otherUserId)
        {
            string groupName = GetChatGroupName(userId, otherUserId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

        private string GetChatGroupName(int userId1, int userId2)
        {
            int smallerId = Math.Min(userId1, userId2);
            int largerId = Math.Max(userId1, userId2);
            return $"Chat_{smallerId}_{largerId}";
        }
    }
}