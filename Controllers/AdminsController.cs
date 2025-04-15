using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealtorConnect.Data;
using RealtorConnect.Models;

namespace RealtorConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Создание группы риэлторов
        [HttpPost("groups")]
        public async Task<IActionResult> CreateGroup([FromBody] CreateRealtorGroupDto dto)
        {
            var group = new RealtorGroup { Name = dto.Name };
            _context.RealtorGroups.Add(group);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Group created successfully", GroupId = group.Id });
        }

        // Получение всех групп
        [HttpGet("groups")]
        public async Task<ActionResult<IEnumerable<RealtorGroup>>> GetGroups()
        {
            return await _context.RealtorGroups
                .Include(g => g.Realtors)
                .ToListAsync();
        }

        // Добавление риэлтора в группу
        [HttpPost("groups/{groupId}/realtors/{realtorId}")]
        public async Task<IActionResult> AddRealtorToGroup(int groupId, int realtorId)
        {
            var realtor = await _context.Realtors.FindAsync(realtorId);
            if (realtor == null)
                return NotFound("Realtor not found");

            var group = await _context.RealtorGroups.FindAsync(groupId);
            if (group == null)
                return NotFound("Group not found");

            realtor.GroupId = groupId;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Realtor added to group successfully" });
        }

        // Получение всех риэлторов в группе
        [HttpGet("groups/{groupId}/realtors")]
        public async Task<ActionResult<IEnumerable<Realtor>>> GetRealtorsInGroup(int groupId)
        {
            var realtors = await _context.Realtors
                .Where(r => r.GroupId == groupId)
                .ToListAsync();

            return Ok(realtors);
        }

        // Удаление риэлтора из группы
        [HttpDelete("groups/{groupId}/realtors/{realtorId}")]
        public async Task<IActionResult> RemoveRealtorFromGroup(int groupId, int realtorId)
        {
            var realtor = await _context.Realtors.FindAsync(realtorId);
            if (realtor == null)
                return NotFound("Realtor not found");

            if (realtor.GroupId != groupId)
                return BadRequest("Realtor is not in this group");

            realtor.GroupId = null;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Realtor removed from group successfully" });
        }

        // Получение всех запросов на вступление в группы
        [HttpGet("join-requests")]
        public async Task<ActionResult<IEnumerable<JoinRequest>>> GetJoinRequests()
        {
            return await _context.JoinRequests
                .Include(jr => jr.Realtor)
                .Include(jr => jr.Group)
                .ToListAsync();
        }

        // Подтверждение запроса на вступление
        [HttpPost("join-requests/{requestId}/approve")]
        public async Task<IActionResult> ApproveJoinRequest(int requestId)
        {
            var request = await _context.JoinRequests
                .Include(jr => jr.Realtor)
                .FirstOrDefaultAsync(jr => jr.Id == requestId);
            if (request == null)
                return NotFound("Join request not found");

            if (request.Status != "Pending")
                return BadRequest("Request is already processed");

            request.Status = "Approved";
            request.Realtor.GroupId = request.GroupId;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Join request approved" });
        }

        // Отклонение запроса на вступление
        [HttpPost("join-requests/{requestId}/reject")]
        public async Task<IActionResult> RejectJoinRequest(int requestId)
        {
            var request = await _context.JoinRequests.FindAsync(requestId);
            if (request == null)
                return NotFound("Join request not found");

            if (request.Status != "Pending")
                return BadRequest("Request is already processed");

            request.Status = "Rejected";
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Join request rejected" });
        }

        // Получение всех пользователей (для админки)
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var clients = await _context.Clients.ToListAsync();
            var realtors = await _context.Realtors.ToListAsync();

            return Ok(new
            {
                Clients = clients,
                Realtors = realtors
            });
        }
    }

    public class CreateRealtorGroupDto
    {
        public string Name { get; set; }
    }
}