using RealtorConnect.Models;
using System.ComponentModel.DataAnnotations;

public class Client
{
    public int Id { get; set; } // Уникальный идентификатор клиента
    public string Name { get; set; } = null!; // Имя клиента

    [Required, EmailAddress, MaxLength(100)]
    public string Email { get; set; } = null!; // Уникальный email

    public string PasswordHash { get; set; } = null;

    public ICollection<GroupClient> GroupClients { get; set; } = new List<GroupClient>();

    // Связь с избранным
    public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
}


