﻿using AdsMarketSharing.Models.Auth;
using AdsMarketSharing.Models.Token;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AdsMarketSharing.Data
{
    /*public class SQLExpressContext: IdentityDbContext<ApplicationUser>
    {
        public SQLExpressContext(DbContextOptions<SQLExpressContext> options) : base(options)
        {

        }
        public DbSet<Account> Accounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

    }*/

    public class SQLExpressContext : DbContext
    {
        public SQLExpressContext(DbContextOptions<SQLExpressContext> options) : base(options)
        {

        }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<AccountRole> AccountRoles { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Role>().HasData(
                new Role { Id=1,Name = Static.AccountRole.Administrator },
                new Role { Id=2,Name = Static.AccountRole.Marketter },
                new Role { Id=3,Name = Static.AccountRole.ServiceStaff },
                new Role { Id=4,Name = Static.AccountRole.Collaborator }
            );
            modelBuilder.Entity<AccountRole>().HasKey(accountRole => new { accountRole.AccountId ,accountRole.RoleId });
        }

    }
}