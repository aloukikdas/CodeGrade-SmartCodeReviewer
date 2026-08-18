using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeReviewer.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddLanguageAndLocking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "Submissions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AllowedLanguage",
                table: "Assignments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Language",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "AllowedLanguage",
                table: "Assignments");
        }
    }
}
