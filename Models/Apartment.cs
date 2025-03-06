
using RealtorConnect.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


public class Apartment
{
    [Key]
    public int Id { get; set; } // Уникальный идентификатор квартиры

    [Required]
    public string Address { get; set; } = null!; // Уникальный адрес квартиры
    public decimal Price { get; set; } // Цена квартиры
    public int Rooms { get; set; } // Количество комнат
    public int Floor { get; set; }
    public string? PhotoUrl { get; set; }
    public string? HouseView { get; set; }
    public double Area { get; set; } // Площадь квартиры
    public string Description { get; set; } = null!; // Описание квартиры
   
    
    public int StatusId { get; set; }

    // Риэлтор, управляющий квартирой
    public int RealtorId { get; set; }

    [ForeignKey(nameof(RealtorId))]
    public Realtor Realtor { get; set; } = null!;

    // Группа риэлторов
    public int RealtorGroupId { get; set; }

    [ForeignKey(nameof(RealtorGroupId))]
    public RealtorGroup RealtorGroup { get; set; } = null!;

    // Связь с избранным
    public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();


}

