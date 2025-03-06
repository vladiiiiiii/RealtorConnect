
public class Favorite
{
    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;

    public int ApartmentId { get; set; }
    public Apartment Apartment { get; set; } = null!;

    public DateTime DateAdded { get; set; } // Дата добавления в избранное
}

