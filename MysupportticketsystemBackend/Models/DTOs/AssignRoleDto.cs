namespace MysupportticketsystemBackend.Models.DTOs
{
    public class AssignRoleDto
    {
        public string Email { get; set; }
        public string RoleName { get; set; } // e.g., "Admin", "Agent", "User"
    }
}