using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealtorConnect.Models
{
    public class GroupClient
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int GroupId { get; set; }

        [ForeignKey(nameof(GroupId))]
        public RealtorGroup? RealtorGroup { get; set; }

        [Required]
        public int ClientId { get; set; }

        [ForeignKey(nameof(ClientId))]
        public Client? Client { get; set; }
    }
}
