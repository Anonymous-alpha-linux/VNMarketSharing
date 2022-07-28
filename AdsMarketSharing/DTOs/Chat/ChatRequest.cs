using System.ComponentModel.DataAnnotations;

namespace AdsMarketSharing.DTOs.Chat
{
    public class ChatRequest
    {
        [Required(ErrorMessage ="the username is required")]
        public string Username { get; set; }
        [Required(ErrorMessage ="this message acquired the specific content")]
        [MaxLength(50)]
        public string Message { get; set; } 
    }
}
