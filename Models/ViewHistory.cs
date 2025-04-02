using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealtorConnect.Models
{
    public class ViewHistory
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int ApartmentId { get; set; }
        public DateTime ViewedAt { get; set; }  = DateTime.UtcNow;

        public Client Client { get; set; } = null!;
        public Apartment Apartment { get; set; } = null!;
    }
}
