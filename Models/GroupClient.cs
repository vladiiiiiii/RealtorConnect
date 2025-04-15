using System.ComponentModel.DataAnnotations;

namespace RealtorConnect.Models
{
    public class GroupClient
    {
        [Key]
        public int Id { get; set; }
        public int GroupId { get; set; }
        public int ClientId { get; set; }

        public RealtorGroup RealtorGroup { get; set; }
        public Client Client { get; set; }
    }
}