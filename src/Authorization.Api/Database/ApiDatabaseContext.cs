using Authorization.Models;
using Microsoft.EntityFrameworkCore;

namespace Authorization.Database
{
    public class ApiDatabaseContext : DbContext
    {
        public ApiDatabaseContext(DbContextOptions<ApiDatabaseContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (Database.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite")
            {
                modelBuilder.Entity<User>()
                    .Property(u => u.Id)
                    .HasAnnotation("Sqlite:Autoincrement", true);
            }
            modelBuilder.Entity<User>().HasData(
                new User() { Id = 1, Name = "User1", Surname = "Surname1", DateOfBirth = new DateTime(1998, 5, 5), Email = "User1Surname1@email.com", Password = "testUser1Surname1", RoleId = UserRoles.SuperAdmin },
                new User() { Id = 2, Name = "User2", Surname = "Surname2", DateOfBirth = new DateTime(2006, 10, 8), Email = "User2Surname2@email.com", Password = "testUser2Surname2", RoleId = UserRoles.User },
                new User() { Id = 3, Name = "User3", Surname = "Surname3", DateOfBirth = new DateTime(2006, 7, 9), Email = "User3Surname3@email.com", Password = "testUser3Surname3", RoleId = UserRoles.Banned },
                new User() { Id = 4, Name = "User4", Surname = "Surname4", DateOfBirth = new DateTime(1980, 10, 10), Email = "User4Surname4@email.com", Password = "testUser4Surname4", RoleId = UserRoles.Admin },
                new User() { Id = 5, Name = "User5", Surname = "Surname5", DateOfBirth = new DateTime(1981, 11, 11), Email = "User5Surname5@email.com", Password = "testUser5Surname5", RoleId = UserRoles.User });
        }
    }
}
