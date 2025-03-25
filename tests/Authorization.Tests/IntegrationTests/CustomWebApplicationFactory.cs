using Authorization.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    private static readonly object _lock = new();
    private static bool _databaseInitialized;
    private DbConnection _connection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApiDatabaseContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            _connection = new SqliteConnection("DataSource=:memory:;Mode=Memory;Cache=Shared");
            _connection.Open();

            services.AddDbContext<ApiDatabaseContext>(options =>
            {
                options.UseSqlite(_connection);
            });

            lock (_lock)
            {
                using var scope = services.BuildServiceProvider().CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApiDatabaseContext>();
                if (!_databaseInitialized)
                {
                    db.Database.EnsureCreated();
                    _databaseInitialized = true;
                    db.Database.EnsureDeleted();
                    db.Database.EnsureCreated();

                    SeedDatabase(db);
                }
            }
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _connection?.Dispose();
    }

    private void SeedDatabase(ApiDatabaseContext db)
    {
        var user1 = new User
        {
            Name = "Test",
            Surname = "Test",
            DateOfBirth = new DateTime(1999, 3, 13),
            Email = "test@test.com",
            Password = "password123",
            RoleId = UserRoles.User
        };

        var user2 = new User
        {
            Name = "TestBanned",
            Surname = "TestBanned",
            DateOfBirth = new DateTime(1999, 3, 13),
            Email = "banned@banned.com",
            Password = "banned123",
            RoleId = UserRoles.Banned,
        };
        user2.HandleBan();

        var user3 = new User
        {
            Name = "admin",
            Surname = "admin",
            DateOfBirth = new DateTime(1999, 3, 13),
            Email = "admin@admin.com",
            Password = "admin123",
            RoleId = UserRoles.Admin,
        };

        var user4 = new User
        {
            Name = "TestBanned2",
            Surname = "TestBanned2",
            DateOfBirth = new DateTime(1999, 3, 13),
            Email = "banned2@banned2.com",
            Password = "banned2$123",
            RoleId = UserRoles.Banned,
        };
        user4.HandleBan();

        var user5 = new User
        {
            Name = "PermamentlyBanned",
            Surname = "PermamentlyBanned",
            DateOfBirth = new DateTime(1998, 2, 11),
            Email = "permabanned@permabanned.com",
            Password = "permabanned123",
            RoleId = UserRoles.Banned,
        };
        user5.Permaban();

        var user6 = new User
        {
            Name = "PermamentlyBanned1",
            Surname = "PermamentlyBanned1",
            DateOfBirth = new DateTime(1997, 1, 10),
            Email = "permabanned1@permabanned1.com",
            Password = "permabanned1233",
            RoleId = UserRoles.Banned,
        };
        user6.Permaban();

        var user7 = new User
        {
            Name = "PermamentlyBanned2",
            Surname = "PermamentlyBanned2",
            DateOfBirth = new DateTime(1990, 11, 11),
            Email = "permabanned2@permabanned3.com",
            Password = "permabanned123334",
            RoleId = UserRoles.Banned,
        };
        user7.Permaban();

        db.Users.AddRange(user1, user2, user3, user4, user5, user6, user7);
        db.SaveChanges();
    }
}