using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealtorConnect.Models
{
    public class Client
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [Required, EmailAddress, MaxLength(100)]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public int? GroupClientId { get; set; }
        public virtual GroupClient GroupClient { get; set; }

        // Навигационные свойства с инициализацией
        public virtual ICollection<ChatMessage>? SentMessages { get; set; } = new List<ChatMessage>();
        public virtual ICollection<ChatMessage>? ReceivedMessages { get; set; } = new List<ChatMessage>();
        public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
        public virtual ICollection<Apartment> Apartments { get; set; } = new List<Apartment>();
        public virtual ICollection<ViewHistory> ViewHistories { get; set; } = new List<ViewHistory>();
    }
}