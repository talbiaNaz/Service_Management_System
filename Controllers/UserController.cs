using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SMS_Project.Context;
using SMS_Project.DTOs;
using SMS_Project.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SMS_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        public UserController(AppDbContext dbContext, IConfiguration configuration)
        {
            _context = dbContext;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDto userDto)
        {
            // Check if user already exists
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == userDto.Email);
            if (existingUser != null)
            {
                return BadRequest("User with this email already exists.");
            }

            // Map UserDto to User model
            var user = new User
            {
                Name = userDto.Name,
                Email = userDto.Email,
                Password = userDto.Password,  // We will hash the password after this
                Role = "Customer"
            };

            // Hash the password before storing it
            var hasher = new PasswordHasher<User>();
            user.Password = hasher.HashPassword(user, userDto.Password);

            // Save user to the database
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User registered successfully!" });
        }



        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            // Find user by email
            var storedUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
            if (storedUser == null)
            {
                return Unauthorized("Invalid credentials");
            }

            // Compare the password using the PasswordHasher
            var hasher = new PasswordHasher<User>();

            // Verify the entered password with the stored hashed password
            var result = hasher.VerifyHashedPassword(storedUser, storedUser.Password, loginDto.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                return Unauthorized("Invalid credentials");
            }

            // Generate JWT token
            var tokenHandler = new JwtSecurityTokenHandler();

            // Retrieve the secret key from appsettings.json
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]); // Fetch key from configuration

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
            new Claim(ClaimTypes.NameIdentifier, storedUser.Id.ToString()), // Storing user ID
            new Claim(ClaimTypes.Name, storedUser.Name), // Store username or email
            new Claim(ClaimTypes.Role, storedUser.Role) // Store user role
                }),
                Expires = DateTime.UtcNow.AddHours(1), // Token expiration
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new
            {
                Token = tokenHandler.WriteToken(token),
                Role = storedUser.Role,
                UserId = storedUser.Id
            });
        }




    }
}
