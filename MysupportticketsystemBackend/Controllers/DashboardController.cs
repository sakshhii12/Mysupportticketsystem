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

       
        [HttpGet("analytics")]
        public async Task<ActionResult<DashboardAnalyticsDto>> GetAnalytics()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            
            var userTickets = await _context.Tickets
                                            .Where(t => t.UserId == userId)
                                            .ToListAsync();

            

            if (!userTickets.Any())
            {
                
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