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
    [Authorize(Roles = "Realtor")]
    public class RealtorGroupsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RealtorGroupsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Получение списка всех групп (для выбора)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RealtorGroup>>> GetGroups()
        {
            return await _context.RealtorGroups.ToListAsync();
        }

        // Запрос на вступление в группу
        [HttpPost("{groupId}/request-join")]
        public async Task<IActionResult> RequestJoinGroup(int groupId)
        {
            var realtorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var group = await _context.RealtorGroups.FindAsync(groupId);
            if (group == null)
                return NotFound("Group not found");

            var existingRequest = await _context.JoinRequests
                .FirstOrDefaultAsync(jr => jr.RealtorId == realtorId && jr.GroupId == groupId && jr.Status == "Pending");
            if (existingRequest != null)
                return BadRequest("You already have a pending request for this group");

            var request = new JoinRequest
            {
                RealtorId = realtorId,
                GroupId = groupId,
                Status = "Pending"
            };

            _context.JoinRequests.Add(request);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Join request sent. Waiting for admin approval" });
        }
    }
}