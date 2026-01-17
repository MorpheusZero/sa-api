using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoulArenasAPI.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddRolesRaw : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RolesRaw",
                table: "users",
                type: "varchar(1024)",
                maxLength: 1024,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RolesRaw",
                table: "users");
        }
    }
}
