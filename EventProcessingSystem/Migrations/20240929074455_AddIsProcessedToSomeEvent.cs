using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventGenerationAndProcessingSystem.Migrations
{
    public partial class AddIsProcessedToSomeEvent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsProcessed",
                table: "Events",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsProcessed",
                table: "Events");
        }
    }
}
