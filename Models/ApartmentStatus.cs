using System.ComponentModel.DataAnnotations;

namespace RealtorConnect.Models
{
    public class ApartmentStatus
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}