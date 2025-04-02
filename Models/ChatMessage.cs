using System.ComponentModel.DataAnnotations;

namespace RealtorConnect.Models
{
    public class ChatMessage
    {
        [Key]
        public int Id { get; set; }

        public int? SenderClientId { get; set; }
        public int? SenderRealtorId { get; set; }

        public int? ReceiverClientId { get; set; }
        public int? ReceiverRealtorId { get; set; }

        [Required]
        public string MessageContent { get; set; }

        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        public virtual Client SenderClient { get; set; }
        public virtual Realtor SenderRealtor { get; set; }
        public virtual Client ReceiverClient { get; set; }
        public virtual Realtor ReceiverRealtor { get; set; }
    }
}