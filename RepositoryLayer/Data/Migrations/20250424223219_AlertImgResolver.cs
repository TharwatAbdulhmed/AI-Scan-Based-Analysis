using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RepositoryLayer.Data.Migrations
{
    /// <inheritdoc />
    public partial class AlertImgResolver : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResolvedImageUrl",
                table: "Studies");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResolvedImageUrl",
                table: "Studies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
