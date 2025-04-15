namespace RealtorConnect.Models
{
    public class JoinRequest
    {
        public int Id { get; set; }
        public int RealtorId { get; set; }
        public int GroupId { get; set; }
        public string Status { get; set; } = "Pending"; // "Pending", "Approved", "Rejected"
        public Realtor Realtor { get; set; }
        public RealtorGroup Group { get; set; }
    }
}