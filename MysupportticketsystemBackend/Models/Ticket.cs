using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MysupportticketsystemBackend.Models
{
    public enum TicketStatus { Open,InProgress,Resolved,Closed}
    public enum TicketPriority { Low, Medium, High }

    public enum TicketCategory { billing , Technical ,General}
    public class Ticket
    {
        public int Id { get; set; }


        [Required] //this column cannot be null.
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public TicketStatus Status { get; set; }
        public TicketPriority Priority { get; set; }
        public TicketCategory Category { get; set; }
        public DateTime CreatedAt { get; set; }

        
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
