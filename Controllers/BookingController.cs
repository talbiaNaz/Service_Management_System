using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMS_Project.Context;
using SMS_Project.DTOs;
using SMS_Project.Models;
using System.Security.Claims;

namespace SMS_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly AppDbContext _context;
        public BookingController(AppDbContext context)
        {
            _context = context;
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> BookService([FromBody] Booking booking)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            booking.UserId = int.Parse(userId);
            booking.Status = "Pending";
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUserBookings), new { userId = booking.UserId }, booking);
        }

        [Authorize]
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserBookings(int userId)
        {
            var bookings = await _context.Bookings.Where(b => b.UserId == userId).ToListAsync();
            return Ok(bookings);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingBookings()
        {
            var bookings = await _context.Bookings
                .Include(b => b.User)       // Include User data
                .Include(b => b.Service)    // Include Service data
                .Where(b => b.Status == "Pending")
                .ToListAsync();

            // Mapping each booking to the BookingDTO
            var bookingDTOs = bookings.Select(b => new BookingDTO
            {
                Id = b.Id,
                UserName = b.User.Name,   // Assuming you have Name field in User model
                ServiceName = b.Service.Name,  // Assuming you have Name field in Service model
                BookingDate = b.BookingDate,
                Status = b.Status
            }).ToList();

            return Ok(bookingDTOs);  // Return the DTO list
        }


        [Authorize(Roles = "Admin")]
        [HttpPut("ApproveOrRejectBooking{id}")]
        public async Task<IActionResult> ApproveOrRejectBooking(int id, [FromBody] string status)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return NotFound();

            booking.Status = status;
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();
            return NoContent();
        }


    }
}
