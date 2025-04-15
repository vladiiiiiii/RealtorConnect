using System.ComponentModel.DataAnnotations;

namespace RealtorConnect.Models
{
    public class ChatMessage
    {
        [Key]
        public int Id { get; set; }
        public int SenderId { get; set; }

        [RegularExpression("^(Client|Realtor)$", ErrorMessage = "SenderType must be either 'Client' or 'Realtor'")]
        public string SenderType { get; set; } // "Realtor" или "Client"
        public int ReceiverId { get; set; }

        [RegularExpression("^(Client|Realtor)$", ErrorMessage = "ReceiverType must be either 'Client' or 'Realtor'")]
        public string ReceiverType { get; set; } // "Realtor" или "Client"
        public string Content { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}