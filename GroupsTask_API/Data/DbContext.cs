using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace GroupsTask_API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Group> Groups { get; set; } 
    }
}
