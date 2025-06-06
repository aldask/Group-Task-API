﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GroupsTask_API.Data;
using GroupsTask_API.Models;

namespace GroupsTask_API.Controllers
{
    [ApiController]
    [Route("api/groups/{groupId}/members")]
    public class MembersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MembersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetMembers(int groupId)
        {
            var groupExists = await _context.Groups.AnyAsync(g => g.Id == groupId);
            if (!groupExists)
                return NotFound("Group not found.");

            var members = await _context.Members
                .Where(m => m.GroupId == groupId)
                .ToListAsync();

            return Ok(members);
        }

        [HttpPost]
        public async Task<IActionResult> AddMember(int groupId, [FromBody] Member member)
        {
            if (string.IsNullOrWhiteSpace(member.Name))
                return BadRequest("Member name is required.");

            var groupExists = await _context.Groups.AnyAsync(g => g.Id == groupId);
            if (!groupExists)
                return NotFound("Group not found.");

            member.GroupId = groupId;
            _context.Members.Add(member);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMembers), new { groupId }, member);
        }

        [HttpDelete("{memberId}")]
        public async Task<IActionResult> RemoveMember(int groupId, int memberId)
        {
            // Find member by ID
            var member = await _context.Members.FindAsync(memberId);

            // Check if member exists and belongs to the group
            if (member == null || member.GroupId != groupId)
                return NotFound("Member not found in group.");

            // Prevent deletion if member balance isn't zero
            if (member.Balance != 0)
                return BadRequest("Cannot remove member with unsettled balance.");

            _context.Members.Remove(member);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}