using System.ComponentModel.DataAnnotations;

namespace MysupportticketsystemBackend.Models.DTOs
{
    public class UpdateTicketDto
    {
        [StringLength(100)]
        public string? Title { get; set; }

        public string? Description { get; set; }

        
        public string? Priority { get; set; }
        public string? Category { get; set; }

        
        public string? Status { get; set; }
    }
}