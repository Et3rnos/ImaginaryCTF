﻿using iCTF_Shared_Resources.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace iCTF_Shared_Resources
{
    public class DatabaseContext : IdentityDbContext<ApplicationUser>
    {
        private const string adminRoleId = "365af12d-bbee-42de-a35b-63a5a6fdb69e";

        public DbSet<Config> Configuration { get; set; }
        public DbSet<Challenge> Challenges { get; set; }

        #pragma warning disable CS0114
        public DbSet<User> Users { get; set; }
        #pragma warning restore CS0114

        public DbSet<Solve> Solves { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityRole>().HasData(new IdentityRole { Id = adminRoleId, Name = "Administrator", NormalizedName = "Administrator".ToUpper() });
        }
    }
}