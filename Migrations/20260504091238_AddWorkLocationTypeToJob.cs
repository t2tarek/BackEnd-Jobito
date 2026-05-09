using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompanyDashboardAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkLocationTypeToJob : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WorkLocationType",
                table: "Jobs",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WorkLocationType",
                table: "Jobs");
        }
    }
}
