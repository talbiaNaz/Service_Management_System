using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SMS_Project.Models
{
    public class Service
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; }

        [Required, MaxLength(500)]
        public string Description { get; set; }

        [Required, Range(1, 10000)]
        public decimal Price { get; set; } // Price must be between 1 and 10,000

        [Required, Range(10, 480)]
        public int Duration { get; set; } // Min 10 mins, Max 8 hours

        public bool IsApproved { get; set; } = false;
    }
}
