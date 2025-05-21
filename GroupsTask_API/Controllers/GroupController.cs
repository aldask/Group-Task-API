using Microsoft.AspNetCore.Mvc;
using GroupsTask_API.Data;
using GroupsTask_API.Models;

namespace GroupsTask_API.Controllers
{
    [ApiController]
    [Route("api/groups")]
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
        public async Task<IActionResult> GetGroup(int id)
        {
            var group = await _context.Groups.FindAsync(id);

            if (group == null)
                return NotFound("Group not found.");

            return Ok(group);
        }

        [HttpPost]
        public IActionResult CreateGroup([FromBody] Group group)
        {
            if (string.IsNullOrWhiteSpace(group.Title))
            {
                return BadRequest("Group title is required.");
            }

            _context.Groups.Add(group);
            _context.SaveChanges();

            return Ok(group);
        }
    }
}