using System.ComponentModel.DataAnnotations;

namespace RealtorConnect.Models
{
    public class GroupRealtor
    {
        [Key]
        public int Id { get; set; }
        public int GroupId { get; set; }
        public int RealtorId { get; set; }

        public RealtorGroup RealtorGroup { get; set; }
        public Realtor Realtor { get; set; }
    }
}