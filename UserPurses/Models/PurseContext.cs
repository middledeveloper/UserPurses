using Microsoft.EntityFrameworkCore;
using UserPurses.Models;

namespace WebAPIApp.Models
{
    public class PurseContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Purse> Purses { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Configuration> Configuration { get; set; }

        public PurseContext(DbContextOptions<PurseContext> options) : base(options)
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }
    }
}