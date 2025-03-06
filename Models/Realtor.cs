using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealtorConnect.Models
{
    public class Realtor
    {
        [Key]
        public int Id { get; set; } // Уникальный идентификатор риэлтора

        [Required]
        public string Name { get; set; } = null!; // Имя риэлтора

        [Required, EmailAddress, MaxLength(100)]
        public string Email { get; set; } = null!; // Уникальный email риэлтора

        [Required]
        public string PasswordHash { get; set; } = null!; // Хэш пароля

        // Связь с квартирами, за которые отвечает риэлтор
        public ICollection<Apartment> Apartments { get; set; } = new List<Apartment>();

        // Связь с группой риэлторов
        public int RealtorGroupId { get; set; }

        [ForeignKey(nameof(RealtorGroupId))]
        public RealtorGroup RealtorGroup { get; set; } = null!;

        public ICollection<GroupRealtor>? GroupRealtors { get; set; }
    }
}
