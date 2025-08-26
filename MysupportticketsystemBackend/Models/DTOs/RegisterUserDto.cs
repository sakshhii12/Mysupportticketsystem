using System.ComponentModel.DataAnnotations;

namespace MysupportticketsystemBackend.Models.DTOs
{
    public class RegisterUserDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}