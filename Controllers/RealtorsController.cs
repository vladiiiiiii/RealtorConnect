using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealtorConnect.Data;
using RealtorConnect.Models;
using System.Security.Claims;

namespace RealtorConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RealtorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RealtorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("apartments")]
        [Authorize(Roles = "Realtor")]
        public async Task<ActionResult<IEnumerable<Apartment>>> GetRealtorApartments([FromQuery] ApartmentFilter filter)
        {
            var realtorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var groupId = await _context.GroupRealtors
                .Where(gr => gr.RealtorId == realtorId)
                .Select(gr => gr.GroupId)
                .FirstOrDefaultAsync();

            var query = _context.Apartments
                .Include(a => a.ApartmentStatus)
                .Where(a => a.RealtorId == realtorId || (a.Realtor != null && a.Realtor.GroupRealtor.GroupId == groupId));

            if (filter.PriceMax.HasValue) query = query.Where(a => a.Price <= (decimal)filter.PriceMax.Value);
            if (filter.Rooms.HasValue) query = query.Where(a => a.Rooms == filter.Rooms);
            if (!string.IsNullOrEmpty(filter.Address)) query = query.Where(a => a.Address.Contains(filter.Address));
            if (filter.FloorMin.HasValue) query = query.Where(a => a.Floor >= filter.FloorMin);
            if (filter.SquareAreaMin.HasValue) query = query.Where(a => a.SquareArea >= (decimal)filter.SquareAreaMin.Value);
            if (!string.IsNullOrEmpty(filter.Area)) query = query.Where(a => a.Area.Contains(filter.Area));

            return await query.ToListAsync();
        }

        [HttpPost]
        [Authorize(Roles = "Realtor")]
        public async Task<ActionResult<Apartment>> AddApartment(Apartment apartment)
        {
            var realtorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (string.IsNullOrEmpty(apartment.Address) || apartment.Price <= 0)
                return BadRequest("Address and price are required.");

            apartment.RealtorId = realtorId;
            apartment.ClientId = null;
            _context.Apartments.Add(apartment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetApartment), new { id = apartment.Id }, apartment);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Realtor")]
        public async Task<IActionResult> UpdateApartment(int id, Apartment apartment)
        {
            if (id != apartment.Id) return BadRequest();

            var existing = await _context.Apartments.FindAsync(id);
            if (existing == null || existing.RealtorId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value))
                return Forbid();

            _context.Entry(existing).CurrentValues.SetValues(apartment);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Realtor")]
        public async Task<IActionResult> DeleteApartment(int id)
        {
            var apartment = await _context.Apartments.FindAsync(id);
            if (apartment == null || apartment.RealtorId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value))
                return Forbid();

            _context.Apartments.Remove(apartment);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("favorite-clients")]
        [Authorize(Roles = "Realtor")]
        public async Task<ActionResult<IEnumerable<Client>>> GetClientsWithFavorites()
        {
            var realtorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var groupId = await _context.GroupRealtors
                .Where(gr => gr.RealtorId == realtorId)
                .Select(gr => gr.GroupId)
                .FirstOrDefaultAsync();

            var apartmentIds = await _context.Apartments
                .Where(a => a.RealtorId == realtorId || (a.Realtor != null && a.Realtor.GroupRealtor.GroupId == groupId))
                .Select(a => a.Id)
                .ToListAsync();

            var clientIds = await _context.Favorites
                .Where(f => apartmentIds.Contains(f.ApartmentId))
                .Select(f => f.ClientId)
                .Distinct()
                .ToListAsync();

            return await _context.Clients
                .Where(c => clientIds.Contains(c.Id))
                .ToListAsync();
        }

        [HttpPost("assign-client")]
        [Authorize(Roles = "Realtor")]
        public async Task<IActionResult> AssignClientToGroup([FromBody] AssignClientRequest request)
        {
            var realtorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var groupId = await _context.GroupRealtors
                .Where(gr => gr.RealtorId == realtorId)
                .Select(gr => gr.GroupId)
                .FirstOrDefaultAsync();

            if (groupId != request.GroupId)
                return Forbid("You can only assign clients to your own group.");

            var existingGroupClient = await _context.GroupClients
                .FirstOrDefaultAsync(gc => gc.ClientId == request.ClientId);
            if (existingGroupClient != null)
                return BadRequest("Client is already assigned to a group.");

            var groupClient = new GroupClient
            {
                GroupId = groupId,
                ClientId = request.ClientId
            };
            _context.GroupClients.Add(groupClient);
            await _context.SaveChangesAsync();

            var client = await _context.Clients.FindAsync(request.ClientId);
            client.GroupClientId = groupClient.Id;
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("add-client")]
        [Authorize(Roles = "Realtor")]
        public async Task<IActionResult> AddClient([FromBody] RegisterRequest model)
        {
            var realtorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var groupId = await _context.GroupRealtors
                .Where(gr => gr.RealtorId == realtorId)
                .Select(gr => gr.GroupId)
                .FirstOrDefaultAsync();

            if (await _context.Clients.AnyAsync(c => c.Email == model.Email))
            {
                return BadRequest("Email already exists.");
            }

            var client = new Client
            {
                Name = model.Name,
                Email = model.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
            };
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            var groupClient = new GroupClient
            {
                GroupId = groupId,
                ClientId = client.Id
            };
            _context.GroupClients.Add(groupClient);
            await _context.SaveChangesAsync();

            client.GroupClientId = groupClient.Id;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Client added and assigned to your group successfully" });
        }

        [HttpGet("group-clients")]
        [Authorize(Roles = "Realtor")]
        public async Task<ActionResult<IEnumerable<Client>>> GetGroupClients()
        {
            var realtorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var groupId = await _context.GroupRealtors
                .Where(gr => gr.RealtorId == realtorId)
                .Select(gr => gr.GroupId)
                .FirstOrDefaultAsync();

            return await _context.GroupClients
                .Where(gc => gc.GroupId == groupId)
                .Include(gc => gc.Client)
                .Select(gc => gc.Client)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Apartment>> GetApartment(int id)
        {
            var apartment = await _context.Apartments
                .Include(a => a.Realtor)
                .Include(a => a.ApartmentStatus)
                .FirstOrDefaultAsync(a => a.Id == id);
            if (apartment == null) return NotFound();
            return apartment;
        }
    }

    public class AssignClientRequest
    {
        public int ClientId { get; set; }
        public int GroupId { get; set; }
    }

    public class ApartmentFilter
    {
        public double? PriceMax { get; set; }
        public int? Rooms { get; set; }
        public int? FloorMin { get; set; }
        public double? SquareAreaMin { get; set; }
        public string Address { get; set; }
        public string Area { get; set; }
    }
}