using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReviewsAssistant.Infrastructure.Data.Migrations;

public partial class AddAiDraftResponseMetadata : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "AiDraftResponseModel",
            table: "Reviews",
            type: "character varying(100)",
            maxLength: 100,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "AiDraftResponseProvider",
            table: "Reviews",
            type: "character varying(100)",
            maxLength: 100,
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "AiDraftResponseModel",
            table: "Reviews");

        migrationBuilder.DropColumn(
            name: "AiDraftResponseProvider",
            table: "Reviews");
    }
}
