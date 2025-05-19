using Microsoft.EntityFrameworkCore;
using GroupsTask_API.Models;

namespace GroupsTask_API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Group> Groups { get; set; } 
    }
}
