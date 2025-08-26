namespace MysupportticketsystemBackend.Models.DTOs
{
    public class TicketQueryDto
    {
        public string? Status { get; set; }
        public string? Priority { get; set; }

        public string? SearchTerm { get; set; }

        public string? SortBy { get; set; }

        public string? SortOrder { get; set; }
    }
}
