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
        // Private fields to hold the services
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ILogger<TicketsController> _logger;

        // *** THIS IS THE CONSTRUCTOR THAT WAS MISSING ***
        // This method receives the required services via Dependency Injection
        // and assigns them to the private fields.
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

        // GET: api/Tickets (with filtering, searching, and sorting)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TicketDto>>> GetMyTickets([FromQuery] TicketQueryDto queryDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Start building the query against the database
            IQueryable<Ticket> query = _context.Tickets
                .Include(t => t.User)
                .Where(t => t.UserId == userId);

            // Apply the dynamic filtering and sorting from our Extension Methods
            query = query.WhereIfStatus(queryDto.Status)
                         .WhereIfPriority(queryDto.Priority)
                         .Search(queryDto.SearchTerm)
                         .ApplySort(queryDto.SortBy, queryDto.SortOrder);

            // Execute the query to get the results from the database
            var tickets = await query.ToListAsync();

            // Map the results to our DTO to send back to the client
            return _mapper.Map<List<TicketDto>>(tickets);
        }

        // POST: api/Tickets
        [HttpPost]
        public async Task<ActionResult<TicketDto>> CreateTicket([FromBody] CreateTicketDto createTicketDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var ticket = _mapper.Map<Ticket>(createTicketDto);

            // Set server-side properties
            ticket.UserId = userId;
            ticket.Status = TicketStatus.Open;
            ticket.CreatedAt = DateTime.UtcNow;

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            // Log the creation of the ticket
            _logger.LogInformation("New ticket created. TicketId: {TicketId}, UserId: {UserId}", ticket.Id, userId);

            // Reload the User navigation property to include the email in the response
            await _context.Entry(ticket).Reference(t => t.User).LoadAsync();

            return Ok(_mapper.Map<TicketDto>(ticket));
        }

        // PUT: api/Tickets/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTicket(int id, [FromBody] UpdateTicketDto updateTicketDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (ticket == null)
            {
                return NotFound("Ticket not found or you do not have permission to edit it.");
            }

            // Manually apply updates for fields that were provided in the request
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
                ticket.Status = Enum.Parse<TicketStatus>(updateTicketDto.Status, true);
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // PATCH: api/Tickets/5/close
        [HttpPatch("{id}/close")]
        public async Task<IActionResult> CloseTicket(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (ticket == null)
            {
                return NotFound("Ticket not found or you do not have permission to close it.");
            }

            // Corrected the casing from .closed to .Closed
            ticket.Status = TicketStatus.Closed;
            await _context.SaveChangesAsync();

            return Ok("Ticket successfully closed.");
        }
    }
}