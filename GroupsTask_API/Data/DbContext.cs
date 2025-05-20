using Microsoft.EntityFrameworkCore;
using GroupsTask_API.Models;

namespace GroupsTask_API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Group> Groups { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TransactionSplit> Splits { get; set; }
    }
}
