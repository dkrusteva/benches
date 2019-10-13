using WalksAndBenches.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace WalksAndBenches.Identity
{
    public class BenchContext : IdentityDbContext<BenchUser>
    {
        public BenchContext(DbContextOptions<BenchContext> options) : base(options)
        {
        }

        public DbSet<Walks> Walks { get; set; }

        /*protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Walks>()
                .HasData(new Walks()
                {
                    Id = 1,
                    Description = "The first walk",
                    Location = "Bristol harbourside",
                    SubmitterName = "Rose",
                    WalkName = "Around the docks",
                    Url = "haroubrside0"
                });
        }*/
    }
}
