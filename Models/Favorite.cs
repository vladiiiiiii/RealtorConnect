
using RealtorConnect.Models;
using System.ComponentModel.DataAnnotations;

namespace RealtorConnect.Models
{
    public class Favorite
    {
        [Key]
        public int Id { get; set; }
        public int ClientId { get; set; }
        public Client Client { get; set; } = null!;

        public int ApartmentId { get; set; }
        public Apartment Apartment { get; set; } = null!;

    }
}

