using System.ComponentModel.DataAnnotations;
namespace MysupportticketsystemBackend.Models.DTOs
{
    public class CreateTicketDto
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        // We will expect the user to send "Low", "Medium", or "High"
        public string Priority { get; set; }

        [Required]
        // We will expect "Billing", "Technical", or "General"
        public string Category { get; set; }
    }
}