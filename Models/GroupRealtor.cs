using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealtorConnect.Models
{
    public class GroupRealtor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int GroupId { get; set; }

        [ForeignKey(nameof(GroupId))]
        public RealtorGroup? RealtorGroup { get; set; }

        [Required]
        public int RealtorId { get; set; }

        [ForeignKey(nameof(RealtorId))]
        public Realtor? Realtor { get; set; }
    }
}
