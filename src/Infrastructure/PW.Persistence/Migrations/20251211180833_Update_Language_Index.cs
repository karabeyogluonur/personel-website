using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PW.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Update_Language_Index : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Language_Code_Unique",
                table: "Languages");

            migrationBuilder.CreateIndex(
                name: "IX_Language_Code_Unique_Active",
                table: "Languages",
                column: "Code",
                unique: true,
                filter: "\"IsDeleted\" = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Language_Code_Unique_Active",
                table: "Languages");

            migrationBuilder.CreateIndex(
                name: "IX_Language_Code_Unique",
                table: "Languages",
                column: "Code",
                unique: true);
        }
    }
}
