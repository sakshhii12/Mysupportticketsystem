namespace MysupportticketsystemBackend.Models.DTOs
{
    public class TicketDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public string Category { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserEmail { get; set; }

        public string UserId { get; set; }
    }
}
