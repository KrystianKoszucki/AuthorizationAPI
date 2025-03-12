using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Authorization.Migrations
{
    public partial class ReplaceBanDurationWithBanEndDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BanDuration",
                table: "Users");

            migrationBuilder.AddColumn<DateTime>(
                name: "BanEndDate",
                table: "Users",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BanEndDate",
                table: "Users");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "BanDuration",
                table: "Users",
                type: "time",
                nullable: true);
        }
    }
}
