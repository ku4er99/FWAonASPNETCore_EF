using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyCompany.Domain.Entities;

namespace MyCompany.Domain
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<TextField> TextFields { get; set; }
        public DbSet<ServiceItem> ServiceItems { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityRole>().HasData(new IdentityRole // Создаем роль admin
            {
                Id = "766c8fe1-3762-42b3-bd96-2c876a9628ec",
                Name = "admin",
                NormalizedName = "ADMIN"
            });

            modelBuilder.Entity<IdentityUser>().HasData(new IdentityUser // Создаем нового пользователя
            {
                Id = "d85aa196-fa1b-488b-89ce-af8aecbac1c7",
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = "my@email.com",
                NormalizedEmail = "MY@EMAIL.COM",
                EmailConfirmed = true,
                PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(null, "superpassword"),
                SecurityStamp = string.Empty
            });

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>  //связываем пользователя с ролью admin
            {
                RoleId = "766c8fe1-3762-42b3-bd96-2c876a9628ec",
                UserId = "d85aa196-fa1b-488b-89ce-af8aecbac1c7"
            });

            modelBuilder.Entity<TextField>().HasData(new TextField { 
                Id = new Guid("6c6a7a28-1b0a-4513-b951-109bb5fbaee0"), 
                CodeWord = "PageIndex", 
                Title = "Главная"
            });
            modelBuilder.Entity<TextField>().HasData(new TextField
            {
                Id = new Guid("fcaa51fb-0684-450d-ac78-a6f8606bfee2"), 
                CodeWord = "PageServices", 
                Title = "Наши услуги"
            });
            modelBuilder.Entity<TextField>().HasData(new TextField
            {
                Id = new Guid("fec701df-524f-4889-ae4f-78a31e6211d6"), 
                CodeWord = "PageContacts", 
                Title = "Контакты"
            });
        }
    }
}
