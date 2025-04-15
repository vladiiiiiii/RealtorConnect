using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealtorConnect.Data;
using RealtorConnect.Models;
using RealtorConnect.Services.Interfaces;
using System.Security.Claims;

namespace RealtorConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApartmentsController : ControllerBase
    {
        private readonly IApartmentService _apartmentService; // Убедимся, что поле определено только один раз
        private readonly ApplicationDbContext _context;

        public ApartmentsController(IApartmentService apartmentService, ApplicationDbContext context)
        {
            _apartmentService = apartmentService;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Apartment>>> GetApartments()
        {
            return await _apartmentService.GetAllApartmentsAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Apartment>> GetApartment(int id)
        {
            var apartment = await _apartmentService.GetApartmentByIdAsync(id);
            if (apartment == null) return NotFound();
            return apartment;
        }

        [HttpGet("group/{groupId}")]
        [Authorize(Roles = "Realtor")]
        public async Task<IActionResult> GetApartmentsForGroup(int groupId)
        {
            var realtorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var realtor = await _context.Realtors.FindAsync(realtorId);
            if (realtor.GroupId != groupId) return Forbid();

            var apartments = await _context.Apartments
                .Where(a => _context.Realtors.Any(r => r.GroupId == groupId && r.Id == a.RealtorId))
                .ToListAsync();
            return Ok(apartments);
        }

        [HttpPost]
        [Authorize(Roles = "Realtor, Client")]
        public async Task<ActionResult<Apartment>> PostApartment(Apartment apartment)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole == "Client")
            {
                apartment.ClientId = userId; // Клиент автоматически становится владельцем
                apartment.RealtorId = null; // Клиент не может указывать риэлтора
            }

            await _apartmentService.AddApartmentAsync(apartment, userId, userRole);
            return CreatedAtAction(nameof(GetApartment), new { id = apartment.Id }, apartment);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Realtor, Client")]
        public async Task<IActionResult> PutApartment(int id, Apartment apartment)
        {
            if (id != apartment.Id) return BadRequest();

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            await _apartmentService.UpdateApartmentAsync(apartment, userId, userRole);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Realtor, Client")]
        public async Task<IActionResult> DeleteApartment(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            await _apartmentService.DeleteApartmentAsync(id, userId, userRole);
            return NoContent();
        }

        [HttpPost("{id}/upload-photo")]
        [Authorize(Roles = "Realtor, Client")]
        public async Task<IActionResult> UploadPhoto(int id, IFormFile file)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                var photoUrl = await _apartmentService.UploadPhotoAsync(id, file, userId, userRole);
                return Ok(new { PhotoUrl = photoUrl });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}