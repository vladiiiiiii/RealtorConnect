using System.ComponentModel.DataAnnotations;

namespace RealtorConnect.Models
{
    public class RealtorGroup
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        public List<Realtor> Realtors { get; set; } = new List<Realtor>();
    }
}