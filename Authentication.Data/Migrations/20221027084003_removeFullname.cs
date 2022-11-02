using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Authentication.Data.Migrations
{
    /// <inheritdoc />
    public partial class removeFullname : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Fullname",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Fullname",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
