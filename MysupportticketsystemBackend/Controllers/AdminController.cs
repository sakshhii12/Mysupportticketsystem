using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MysupportticketsystemBackend.Data;
using MysupportticketsystemBackend.Models;
using MysupportticketsystemBackend.Models.DTOs;

namespace MysupportticketsystemBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // <-- This is the key. Only users with the "Admin" role can access this controller.
    public class AdminController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AdminController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // Endpoint for an Admin to assign a role to a user.
        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto assignRoleDto)
        {
            var user = await _userManager.FindByEmailAsync(assignRoleDto.Email);
            if (user == null)
            {
                return NotFound($"User with email '{assignRoleDto.Email}' not found.");
            }

            if (!await _userManager.IsInRoleAsync(user, assignRoleDto.RoleName))
            {
                await _userManager.AddToRoleAsync(user, assignRoleDto.RoleName);
                return Ok($"Role '{assignRoleDto.RoleName}' assigned successfully to user '{user.Email}'.");
            }

            return BadRequest($"User '{user.Email}' already has the role '{assignRoleDto.RoleName}'.");
        }

        // Endpoint for an Admin to assign a ticket to an Agent.
        [HttpPatch("tickets/{ticketId}/assign/{agentEmail}")]
        public async Task<IActionResult> AssignTicket(int ticketId, string agentEmail)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null)
            {
                return NotFound($"Ticket with ID '{ticketId}' not found.");
            }

            var agent = await _userManager.FindByEmailAsync(agentEmail);
            if (agent == null || !await _userManager.IsInRoleAsync(agent, "Agent"))
            {
                return BadRequest($"The user '{agentEmail}' is not a valid agent.");
            }

            ticket.AssignedToAgentId = agent.Id;
            await _context.SaveChangesAsync();

            return Ok($"Ticket #{ticketId} successfully assigned to agent {agentEmail}.");
        }
    }
}
