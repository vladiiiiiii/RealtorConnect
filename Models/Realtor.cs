using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealtorConnect.Models
{
    public class Realtor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Required, EmailAddress, MaxLength(100)]
        public string Email { get; set; } = null!;

        [Required]
        public string PasswordHash { get; set; } = null!;

        public int? GroupRealtorId { get; set; } // Ссылка на запись в GroupRealtor
        public virtual GroupRealtor GroupRealtor { get; set; } // Один риэлтор — одна группа
        public virtual RealtorGroup RealtorGroup { get; set; }
        public virtual ICollection<Apartment> Apartments { get; set; }
        public virtual ICollection<ChatMessage>? SentMessages { get; set; }
        public virtual ICollection<ChatMessage>? ReceivedMessages { get; set; }

        // Связь с группой риэлторов через промежуточную таблицу
        public ICollection<GroupRealtor> GroupRealtors { get; set; } = new List<GroupRealtor>();
    }
}
