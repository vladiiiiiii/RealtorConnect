using System.ComponentModel.DataAnnotations;

namespace RealtorConnect.Models
{
    public class Apartment
    {
        public int Id { get; set; }

        public int? RealtorId { get; set; }

        public int? ClientId { get; set; }

        public int StatusId { get; set; }

        [Required]
        public string Address { get; set; }

        [Range(1, int.MaxValue)]
        public int Rooms { get; set; }

        [Range(0, double.MaxValue)]
        public double Area { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        public string? Description { get; set; }

        public string? PhotoUrl { get; set; }

        public Realtor? Realtor { get; set; }

        public Client? Client { get; set; }

        public ApartmentStatus? Status { get; set; }

        public List<Favorite> Favorites { get; set; } = new List<Favorite>();
    }
}