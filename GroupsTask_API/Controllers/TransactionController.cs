using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GroupsTask_API.Data;
using GroupsTask_API.Models;

namespace GroupsTask_API.Controllers
{
    [ApiController]
    [Route("api/groups/{groupId}/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TransactionsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetTransactions(int groupId)
        {
            var groupExists = await _context.Groups.AnyAsync(g => g.Id == groupId);
            if (!groupExists) return NotFound("Group not found.");

            var transactions = await _context.Transactions
                .Where(t => t.GroupId == groupId)
                .Include(t => t.Payer)
                .Include(t => t.Splits)
                .ToListAsync();

            return Ok(transactions);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTransaction(int groupId, [FromBody] Transaction transaction)
        {
            var group = await _context.Groups.FindAsync(groupId);
            if (group == null)
                return NotFound("Group not found.");

            if (transaction.Amount <= 0)
                return BadRequest("Amount must be greater than zero.");

            var payer = await _context.Members.FindAsync(transaction.PayerId);
            if (payer == null || payer.GroupId != groupId)
                return BadRequest("Payer must be a member of the group.");

            transaction.GroupId = groupId;

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            // Still need to work on logic!!!!!!!!

            return CreatedAtAction(nameof(GetTransactions), new { groupId }, transaction);
        }
    }
}
