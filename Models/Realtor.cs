using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RealtorConnect.Models
{
    public class Realtor
    {
        public int Id { get; set; }

        public int? GroupId { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Phone { get; set; }

        public string? PhotoUrl { get; set; }

        public RealtorGroup? Group { get; set; } // Навигационное свойство

        [JsonIgnore] // Игнорируем это свойство при сериализации
        public List<Apartment> Apartments { get; set; } = new List<Apartment>(); // Не должно быть [Required]
    }
}