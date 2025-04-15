using System.ComponentModel.DataAnnotations;

namespace RealtorConnect.Models.Dto
{
    public class SendMessageRequest
    {
        [Required(ErrorMessage = "Message cannot be empty")]
        [StringLength(400, ErrorMessage = "Message cannot exceed 400 characters")]
        public string Content { get; set; }
    }
}