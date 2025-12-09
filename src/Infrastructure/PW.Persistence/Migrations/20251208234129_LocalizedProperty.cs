using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PW.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class LocalizedProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LocalizedProperties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EntityId = table.Column<int>(type: "integer", nullable: false),
                    LanguageId = table.Column<int>(type: "integer", nullable: false),
                    LocaleKeyGroup = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                    LocaleKey = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                    LocaleValue = table.Column<string>(type: "text", maxLength: 2147483647, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalizedProperties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocalizedProperties_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LocalizedProperties_EntityId_LocaleKeyGroup",
                table: "LocalizedProperties",
                columns: new[] { "EntityId", "LocaleKeyGroup" });

            migrationBuilder.CreateIndex(
                name: "IX_LocalizedProperties_LanguageId",
                table: "LocalizedProperties",
                column: "LanguageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LocalizedProperties");
        }
    }
}
