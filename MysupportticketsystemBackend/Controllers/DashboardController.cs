using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MysupportticketsystemBackend.Data;
using MysupportticketsystemBackend.Models;
using MysupportticketsystemBackend.Models.DTOs;
using System.Security.Claims;

namespace MysupportticketsystemBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // This is the NEW, corrected GetAnalytics method
        [HttpGet("analytics")]
        public async Task<ActionResult<DashboardAnalyticsDto>> GetAnalytics()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Get the base set of tickets for the user first.
            // We use ToListAsync() here to execute the query and bring the data into memory.
            var userTickets = await _context.Tickets
                                            .Where(t => t.UserId == userId)
                                            .ToListAsync();

            // Now that the data is in memory, we can perform complex operations on it using LINQ to Objects,
            // which can handle anything, unlike the EF Core translator.

            if (!userTickets.Any())
            {
                // If the user has no tickets, return a zeroed-out DTO immediately.
                return Ok(new DashboardAnalyticsDto
                {
                    TotalTickets = 0,
                    OpenTickets = 0,
                    ResolvedTickets = 0,
                    ClosedTickets = 0,
                    TicketsByCategory = new Dictionary<string, int>(),
                    TicketsByPriority = new Dictionary<string, int>()
                });
            }

            // Now, perform all the calculations on the in-memory list.
            var analyticsDto = new DashboardAnalyticsDto
            {
                TotalTickets = userTickets.Count,
                OpenTickets = userTickets.Count(t => t.Status == TicketStatus.Open),
                ResolvedTickets = userTickets.Count(t => t.Status == TicketStatus.Resolved),
                ClosedTickets = userTickets.Count(t => t.Status == TicketStatus.Closed),

                TicketsByCategory = userTickets.GroupBy(t => t.Category)
                                             .ToDictionary(g => g.Key.ToString(), g => g.Count()),

                TicketsByPriority = userTickets.GroupBy(t => t.Priority)
                                             .ToDictionary(g => g.Key.ToString(), g => g.Count())
            };

            return Ok(analyticsDto);
        }
    }
}