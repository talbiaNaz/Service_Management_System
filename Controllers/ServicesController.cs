using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMS_Project.Context;
using SMS_Project.Models;

namespace SMS_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ServicesController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddService([FromBody] Service service)
        {
            _context.Services.Add(service);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetService), new { id = service.Id }, service);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetService(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null)
            {
                return NotFound();
            }
            return Ok(service);
        }

        [HttpGet]
        public async Task<IActionResult> GetApprovedServices()
        {
            // Filter services where 'isApproved' is true (or whatever your field is named)
            var approvedServices = await _context.Services
                                                 .Where(service => service.IsApproved == true) // Assuming 'IsApproved' is a boolean
                                                 .ToListAsync();

            // Return the approved services as the response
            return Ok(approvedServices);
        }


        [Authorize(Roles = "Admin")]
        [HttpPut("ApproveOrRejectService:{id}")]
        public async Task<IActionResult> ApproveOrRejectService(int id, [FromBody] bool isApproved)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null) return NotFound();

            service.IsApproved = isApproved;
            _context.Services.Update(service);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
