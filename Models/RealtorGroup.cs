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

        public virtual ICollection<GroupRealtor> GroupRealtors { get; set; }
        public virtual ICollection<GroupClient> GroupClients { get; set; }
        public virtual ICollection<Apartment> Apartments { get; set; }

    }
}
