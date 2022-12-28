using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FPTBook.Models;
using FPTBook.Utils;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FPTBook.Data
{
    public class ApplicationDbContext:IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
          //..
        }
        public DbSet<Book> Books { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
             this.SeedRoles(modelBuilder);
            RoleAdmin(modelBuilder);
             Admin(modelBuilder);

        }
        private void Admin(ModelBuilder builder1){
            var hasher = new PasswordHasher<ApplicationUser>();
            builder1.Entity<ApplicationUser>().HasData(
                new ApplicationUser() {  
                                        Id = "AdminID123",
                                        FullName = "Nguyen Van Hoan"
                                        ,Address = "Da Nang"
                                        ,UserName = "admin@gmail.com"
                                        ,NormalizedUserName = "ADMIN@GMAIL.COM"
                                        ,Email = "admin@gmail.com"
                                        ,NormalizedEmail = "ADMIN@GMAIL.COM"
                                        ,EmailConfirmed = true
                                        ,PasswordHash = hasher.HashPassword(null,"Hoan12345@")
                                        // ,SecurityStamp = "d37dafcc-608e-4922-a535-b13bc790da06"
                                        // ,ConcurrencyStamp = "0ea5c5a8-cb0c-4e6b-9e9b-43c263407543"
                                        ,PhoneNumber = "0346724026"
                                        ,PhoneNumberConfirmed = false
                                        ,TwoFactorEnabled = false
                                        ,LockoutEnd = null
                                        ,LockoutEnabled = true
                                        ,AccessFailedCount =0
                                        }
            );
        }
        private void RoleAdmin(ModelBuilder builder){
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole(){Id = "AdminRole", Name = "admin", ConcurrencyStamp ="3", NormalizedName = "admin"}
            );

            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string> {RoleId = "AdminRole", UserId="AdminID123"}
            );
        }
        private void SeedRoles(ModelBuilder builder)
        {
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole() { Id = "b74ddd14-6340-4840-95c2-db12554843e5", Name = Role.CUSTOMER, ConcurrencyStamp = "1", NormalizedName = Role.CUSTOMER },
                new IdentityRole() { Id = "87az93ba-d201-2597-edc1-d211f91b7cb1", Name = Role.OWNER, ConcurrencyStamp = "2", NormalizedName = Role.OWNER }
                );
        }

    }
}