using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealtorConnect.Models
{
    public class Apartment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Address { get; set; } = null!;
        public decimal Price { get; set; }
        public int Rooms { get; set; }
        public int Floor { get; set; }
        public string? PhotoUrl { get; set; }
        public string? HouseView { get; set; }
        public string Area { get; set; }
        public string Description { get; set; } = null!;
        public decimal SquareArea { get; set; } // Площадь в квадратных метрах

        public ApartmentStatus Status { get; set; } = null!;

        public int? RealtorId { get; set; } // Для квартир риэлтора
        public int? ClientId { get; set; }  // Для квартир клиента

        public virtual ApartmentStatus ApartmentStatus { get; set; }
        public virtual Realtor Realtor { get; set; }
        public virtual Client Client { get; set; }
        public virtual ICollection<Favorite> Favorites { get; set; }
        public virtual ICollection<ViewHistory> ViewHistories { get; set; }
    }
}