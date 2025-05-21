using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GroupsTask_API.Data;
using GroupsTask_API.Models;

namespace GroupsTask_API.Controllers
{
    [ApiController]
    [Route("api/groups/{groupId}/transactions")]
    public class TransactionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TransactionsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTransaction(int groupId, [FromQuery] string splitMode, [FromBody] Transaction transaction)
        {
            if (transaction == null)
                return BadRequest("Transaction data is required.");

            var group = await _context.Groups.FindAsync(groupId);
            if (group == null)
                return NotFound("Group not found.");

            if (transaction.Amount <= 0)
                return BadRequest("Amount must be greater than zero.");

            var payer = await _context.Members.FindAsync(transaction.PayerId);
            if (payer == null || payer.GroupId != groupId)
                return BadRequest("Payer must be a member of the group.");

            var members = await _context.Members.Where(m => m.GroupId == groupId).ToListAsync();
            if (!members.Any())
                return BadRequest("Group has no members.");

            var finalSplits = new List<TransactionSplit>();

            switch (splitMode?.ToLower())
            {
                case "equal":
                    {
                        int count = members.Count;
                        decimal equalAmount = Math.Round(transaction.Amount / count, 2);
                        decimal totalSplit = equalAmount * count;
                        decimal remainder = Math.Round(transaction.Amount - totalSplit, 2);

                        for (int i = 0; i < count; i++)
                        {
                            var amount = i == 0 ? equalAmount + remainder : equalAmount;
                            finalSplits.Add(new TransactionSplit { MemberId = members[i].Id, Amount = amount });
                        }
                        break;
                    }
                case "percentage":
                    {
                        if (transaction.Splits == null || transaction.Splits.Count != members.Count)
                            return BadRequest("Provide splits with percentages for all members.");

                        var totalPercent = transaction.Splits.Sum(s => s.Amount);
                        if (Math.Abs(totalPercent - 100m) > 0.01m)
                            return BadRequest("Percentages must add up to 100.");

                        var splitsList = transaction.Splits.ToList();

                        for (int i = 0; i < members.Count; i++)
                        {
                            var percent = splitsList[i].Amount;
                            var splitAmount = Math.Round((percent / 100m) * transaction.Amount, 2);
                            finalSplits.Add(new TransactionSplit
                            {
                                MemberId = members[i].Id,
                                Amount = splitAmount
                            });
                        }
                        break;
                    }
                case "dynamic":
                    {
                        if (transaction.Splits == null || !transaction.Splits.Any())
                            return BadRequest("Splits are required for dynamic mode.");

                        var sumSplits = transaction.Splits.Sum(s => s.Amount);
                        if (Math.Abs(sumSplits - transaction.Amount) > 0.01m)
                            return BadRequest("Splits must sum to the total amount.");

                        var uniqueMemberIds = transaction.Splits.Select(s => s.MemberId).Distinct().ToList();
                        if (uniqueMemberIds.Count != transaction.Splits.Count)
                            return BadRequest("No duplicate members allowed in splits.");

                        foreach (var memberId in uniqueMemberIds)
                        {
                            if (!members.Any(m => m.Id == memberId))
                                return BadRequest($"Member ID {memberId} is not in the group.");
                        }

                        finalSplits = transaction.Splits.Select(s => new TransactionSplit
                        {
                            MemberId = s.MemberId,
                            Amount = s.Amount
                        }).ToList();
                        break;
                    }
                default:
                    return BadRequest("splitMode must be 'equal', 'percentage', or 'dynamic'.");
            }

            transaction.GroupId = groupId;
            transaction.CreatedAt = DateTime.UtcNow;
            transaction.Splits = finalSplits;

            payer.Balance += transaction.Amount;
            foreach (var split in finalSplits)
            {
                var member = members.First(m => m.Id == split.MemberId);
                member.Balance -= split.Amount;
            }

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTransactions), new { groupId }, transaction);
        }

        [HttpGet]
        public async Task<IActionResult> GetTransactions(int groupId)
        {
            var transactions = await _context.Transactions
                .Where(t => t.GroupId == groupId)
                .Include(t => t.Splits)
                .ToListAsync();

            return Ok(transactions);
        }
    }
}