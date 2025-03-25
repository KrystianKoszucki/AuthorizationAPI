using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Authorization.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    RoleIdBeforeBan = table.Column<int>(type: "int", nullable: false),
                    BanCounter = table.Column<int>(type: "int", nullable: false),
                    LastBanDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BanDuration = table.Column<TimeSpan>(type: "time", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "BanCounter", "BanDuration", "DateOfBirth", "Email", "LastBanDate", "Name", "Password", "RoleId", "RoleIdBeforeBan", "Surname" },
                values: new object[,]
                {
                    { 1, 0, null, new DateTime(1998, 5, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "User1Surname1@email.com", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "User1", "testUser1Surname1", 3, 0, "Surname1" },
                    { 2, 0, null, new DateTime(2006, 10, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "User2Surname2@email.com", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "User2", "testUser2Surname2", 1, 0, "Surname2" },
                    { 3, 0, null, new DateTime(2006, 7, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "User3Surname3@email.com", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "User3", "testUser3Surname3", 4, 0, "Surname3" },
                    { 4, 0, null, new DateTime(1980, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "User4Surname4@email.com", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "User4", "testUser4Surname4", 2, 0, "Surname4" },
                    { 5, 0, null, new DateTime(1981, 11, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "User5Surname5@email.com", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "User5", "testUser5Surname5", 1, 0, "Surname5" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
