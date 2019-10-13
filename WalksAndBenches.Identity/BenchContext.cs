using WalksAndBenches.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace WalksAndBenches.Identity
{
    public class BenchContext : IdentityDbContext<BenchUser>
    {
        public BenchContext(DbContextOptions<BenchContext> options) : base(options)
        {
        }

        public DbSet<Walks> Walks { get; set; }
    }
}
