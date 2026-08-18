using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeReviewer.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddAIGradingColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AiFeedback",
                table: "Submissions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Grade",
                table: "Submissions",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AiFeedback",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "Grade",
                table: "Submissions");
        }
    }
}
