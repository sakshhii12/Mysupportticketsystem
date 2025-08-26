namespace MysupportticketsystemBackend.Models.DTOs
{
    public class DashboardAnalyticsDto
    {
        public int TotalTickets { get; set; }
        public int OpenTickets { get; set; }
        public int ResolvedTickets { get; set; }
        public int ClosedTickets { get; set; }

        // These will be used for charting
        public Dictionary<string, int> TicketsByCategory { get; set; }
        public Dictionary<string, int> TicketsByPriority { get; set; }
    }
}
