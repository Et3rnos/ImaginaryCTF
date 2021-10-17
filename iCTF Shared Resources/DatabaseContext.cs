using iCTF_Shared_Resources.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;

namespace iCTF_Shared_Resources
{
    public class DatabaseContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Config> Configuration { get; set; }
        public DbSet<Challenge> Challenges { get; set; }

        #pragma warning disable CS0114
        public DbSet<User> Users { get; set; }
        #pragma warning restore CS0114

        public DbSet<Team> Teams { get; set; }
        public DbSet<Solve> Solves { get; set; }
        public DbSet<Redirect> Redirects { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            /*optionsBuilder.ConfigureWarnings(w => {
                w.Throw(RelationalEventId.MultipleCollectionIncludeWarning);
            });*/
        }
    }
}
