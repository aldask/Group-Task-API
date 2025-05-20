using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GroupsTask_API.Data;
using GroupsTask_API.Models;

namespace GroupsTask_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroupsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GroupsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAllGroups()
        {
            var groups = _context.Groups.ToList();
            return Ok(groups);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGroupAsync(int id)
        {
            var group = await _context.Groups
                .Include(g => g.Members)
                        .Include(g => g.Transactions)
                        .ThenInclude(t => t.Splits)
                        .FirstOrDefaultAsync(g => g.Id == id);
            if (group == null)
                return NotFound("Group not found.");

            return Ok(group);
        }

        [HttpPost]
        public IActionResult CreateGroup([FromBody] Group group)
        {
            if (string.IsNullOrEmpty(group.Title))
            {
                return BadRequest("Group title is required.");
            }
            else
            {
                _context.Groups.Add(group);
                _context.SaveChanges();
                return CreatedAtAction(nameof(GetGroupAsync), new { id = group.Id }, group);
            }
        }
    }
}