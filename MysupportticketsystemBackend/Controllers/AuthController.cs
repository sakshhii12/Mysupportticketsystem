using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MysupportticketsystemBackend.Models;
using MysupportticketsystemBackend.Models.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MysupportticketsystemBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;


        public AuthController(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

       
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDto)
        {
           
            var userExists = await _userManager.FindByEmailAsync(registerDto.Email);
            if (userExists != null)
            {
                return BadRequest("User with this email already exists.");
            }

            
            var newUser = new ApplicationUser()
            {
                Email = registerDto.Email,
                UserName = registerDto.Email, 
                SecurityStamp = Guid.NewGuid().ToString()
            };

  
            var result = await _userManager.CreateAsync(newUser, registerDto.Password);

            if (!result.Succeeded)
            {
              
                return BadRequest(result.Errors);
            }

            return Ok("User created successfully!");
        }

        
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto loginDto)
        {
          
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

          
            if (user != null && await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
               
                var tokenString = await GenerateJwtToken(user);

                user.RefreshToken = Guid.NewGuid().ToString();
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                await _userManager.UpdateAsync(user);
                return Ok(new {
                    token = tokenString,
                    refreshToken = user.RefreshToken
                });
            }

            
            return Unauthorized("Invalid email or password.");
        }


        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var authClaims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

   
            // Get the roles for the user 
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }
            

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] TokenRequestDto tokenRequest)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshToken == tokenRequest.RefreshToken);

            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return BadRequest("Invalid refresh token or token expired.");
            }
            var newJwtToken = GenerateJwtToken(user);
            return Ok(new { token = newJwtToken });
        }
        }
}