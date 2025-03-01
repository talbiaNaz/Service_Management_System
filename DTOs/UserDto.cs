using System.ComponentModel.DataAnnotations;

namespace SMS_Project.DTOs
{
    public class UserDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; }

        [Required, EmailAddress, MaxLength(150)]
        public string Email { get; set; }

        [Required, MinLength(6), MaxLength(255)]
        public string Password { get; set; }

        [Required, MaxLength(10)]
        public string Role { get; set; }
    }
}
