namespace RealtorConnect.Models
{
    public class Favorite
    {
        public int ClientId { get; set; }
        public int ApartmentId { get; set; }

        public Client Client { get; set; }
        public Apartment Apartment { get; set; }
    }
}