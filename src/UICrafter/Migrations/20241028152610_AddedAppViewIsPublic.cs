using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UICrafter.Migrations
{
    /// <inheritdoc />
    public partial class AddedAppViewIsPublic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "AppViews",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "AppViews");
        }
    }
}
