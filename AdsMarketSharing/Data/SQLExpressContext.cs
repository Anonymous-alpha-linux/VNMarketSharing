using AdsMarketSharing.Entities;
using AdsMarketSharing.Entities.Functions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1,Name = Static.AccountRole.Administrator },
                new Role { Id = 2,Name = Static.AccountRole.Marketter },
                new Role { Id = 3,Name = Static.AccountRole.ServiceStaff },
                new Role { Id = 4,Name = Static.AccountRole.Collaborator }
            );

            modelBuilder.Entity<AccountRole>().HasKey(accountRole => new { accountRole.AccountId ,accountRole.RoleId });
            
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            //Scalars.RegisterFunction(modelBuilder);   
        }


        public DbSet<Account> Accounts { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<AccountRole> AccountRoles { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<ReceiverAddress> ReceiverAddresses { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<UserPage> UserPages { get; set; }
        public DbSet<ProductClassify> ProductClassifies { get; set; }
        public DbSet<ProductClassifyType> ProductClassifyTypes { get; set; }
        public DbSet<ProductClassfiyDetail> ProductClassfiyDetails { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Reply> Replies { get; set; }
    }
}
