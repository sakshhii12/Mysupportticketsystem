using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MysupportticketsystemBackend.Data;
using MysupportticketsystemBackend.Extensions;
using MysupportticketsystemBackend.Models;
using MysupportticketsystemBackend.Models.DTOs;
using System.Security.Claims;

namespace MysupportticketsystemBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TicketsController : ControllerBase
    {
        
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ILogger<TicketsController> _logger;

      
        public TicketsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            ILogger<TicketsController> logger)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
            _logger = logger;
        }

        // GET: api/Tickets 
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TicketDto>>> GetMyTickets([FromQuery] TicketQueryDto queryDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); 

            IQueryable<Ticket> query = _context.Tickets.Include(t => t.User);

           

            if (User.IsInRole("Admin"))
            {
                
                _logger.LogInformation("Admin user {UserId} is fetching all tickets.", userId);
            }
            else if (User.IsInRole("Agent"))
            {
               
                _logger.LogInformation("Agent user {UserId} is fetching their assigned tickets.", userId);
                query = query.Where(t => t.AssignedToAgentId == userId);
            }
            else 
            {
                
                _logger.LogInformation("User {UserId} is fetching their own tickets.", userId);
                query = query.Where(t => t.UserId == userId);
            }
            query = query.WhereIfStatus(queryDto.Status)
                         .WhereIfPriority(queryDto.Priority)
                         .Search(queryDto.SearchTerm)
                         .ApplySort(queryDto.SortBy, queryDto.SortOrder);

            var tickets = await query.ToListAsync();

            return _mapper.Map<List<TicketDto>>(tickets);
        }

        // POST: api/Tickets
        [HttpPost]
        public async Task<ActionResult<TicketDto>> CreateTicket([FromBody] CreateTicketDto createTicketDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var ticket = _mapper.Map<Ticket>(createTicketDto);

            
            ticket.UserId = userId;
            ticket.Status = TicketStatus.Open;
            ticket.CreatedAt = DateTime.UtcNow;

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

           
            _logger.LogInformation("New ticket created. TicketId: {TicketId}, UserId: {UserId}", ticket.Id, userId);

           
            await _context.Entry(ticket).Reference(t => t.User).LoadAsync();

            return Ok(_mapper.Map<TicketDto>(ticket));
        }

        // PUT: api/Tickets/id
        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTicket(int id, [FromBody] UpdateTicketDto updateTicketDto)
        {
            var ticket = await _context.Tickets.FindAsync(id);

            if (ticket == null)
            {
                return NotFound("Ticket not found."); 
            }

            
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!User.IsInRole("Admin") && ticket.UserId != userId)
            {
                
                return Forbid(); 
            }
            

            
            if (!string.IsNullOrEmpty(updateTicketDto.Title))
            {
                ticket.Title = updateTicketDto.Title;
            }
            if (!string.IsNullOrEmpty(updateTicketDto.Description))
            {
                ticket.Description = updateTicketDto.Description;
            }
            if (!string.IsNullOrEmpty(updateTicketDto.Priority))
            {
                ticket.Priority = Enum.Parse<TicketPriority>(updateTicketDto.Priority, true);
            }
            if (!string.IsNullOrEmpty(updateTicketDto.Category))
            {
                ticket.Category = Enum.Parse<TicketCategory>(updateTicketDto.Category, true);
            }
            if (!string.IsNullOrEmpty(updateTicketDto.Status))
            {
                
                if (User.IsInRole("Admin"))
                {
                    ticket.Status = Enum.Parse<TicketStatus>(updateTicketDto.Status, true);
                }
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // PATCH: api/Tickets/close
        [HttpPatch("{id}/close")]
        public async Task<IActionResult> CloseTicket(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (ticket == null)
            {
                return NotFound("Ticket not found or you do not have permission to close it.");
            }

           
            ticket.Status = TicketStatus.Closed;
            await _context.SaveChangesAsync();

            return Ok("Ticket successfully closed.");
        }
    }
}