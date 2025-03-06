using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealtorConnect.Models
{
    public class RealtorGroup
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        // Связь с риэлторами
        public ICollection<Realtor> Realtors { get; set; } = new List<Realtor>();

        // Связь с клиентами
        public ICollection<GroupClient> GroupClients { get; set; } = new List<GroupClient>();

        // Квартиры, доступные группе
        public ICollection<Apartment> Apartments { get; set; } = new List<Apartment>();
    }
}
