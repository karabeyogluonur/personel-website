using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PW.Persistence.Migrations;

/// <inheritdoc />
public partial class Initial : Migration
{
   /// <inheritdoc />
   protected override void Up(MigrationBuilder migrationBuilder)
   {
      migrationBuilder.CreateTable(
          name: "Assets",
          columns: table => new
          {
             Id = table.Column<int>(type: "integer", nullable: false)
                  .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
             FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
             Folder = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
             Extension = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
             ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
             AltText = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
          },
          constraints: table =>
          {
             table.PrimaryKey("PK_Assets", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "Categories",
          columns: table => new
          {
             Id = table.Column<int>(type: "integer", nullable: false)
                  .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
             Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
             Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
             IsActive = table.Column<bool>(type: "boolean", nullable: false),
             CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
             UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
             IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
             DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
          },
          constraints: table =>
          {
             table.PrimaryKey("PK_Categories", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "Languages",
          columns: table => new
          {
             Id = table.Column<int>(type: "integer", nullable: false)
                  .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
             Code = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
             Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
             FlagImageFileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
             IsPublished = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
             IsDefault = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
             DisplayOrder = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
             IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
             DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
             CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
             UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
          },
          constraints: table =>
          {
             table.PrimaryKey("PK_Languages", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "Settings",
          columns: table => new
          {
             Id = table.Column<int>(type: "integer", nullable: false)
                  .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
             Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
             Value = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false)
          },
          constraints: table =>
          {
             table.PrimaryKey("PK_Settings", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "Technologies",
          columns: table => new
          {
             Id = table.Column<int>(type: "integer", nullable: false)
                  .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
             Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
             IconImageFileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
             Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
             IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
             CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
             UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
          },
          constraints: table =>
          {
             table.PrimaryKey("PK_Technologies", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "AssetTranslations",
          columns: table => new
          {
             Id = table.Column<int>(type: "integer", nullable: false)
                  .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
             LanguageId = table.Column<int>(type: "integer", nullable: false),
             EntityId = table.Column<int>(type: "integer", nullable: false),
             AltText = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
          },
          constraints: table =>
          {
             table.PrimaryKey("PK_AssetTranslations", x => x.Id);
             table.ForeignKey(
                    name: "FK_AssetTranslations_Assets_EntityId",
                    column: x => x.EntityId,
                    principalTable: "Assets",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
             table.ForeignKey(
                    name: "FK_AssetTranslations_Languages_LanguageId",
                    column: x => x.LanguageId,
                    principalTable: "Languages",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
          });

      migrationBuilder.CreateTable(
          name: "CategoryTranslations",
          columns: table => new
          {
             Id = table.Column<int>(type: "integer", nullable: false)
                  .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
             LanguageId = table.Column<int>(type: "integer", nullable: false),
             EntityId = table.Column<int>(type: "integer", nullable: false),
             Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
             Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
          },
          constraints: table =>
          {
             table.PrimaryKey("PK_CategoryTranslations", x => x.Id);
             table.ForeignKey(
                    name: "FK_CategoryTranslations_Categories_EntityId",
                    column: x => x.EntityId,
                    principalTable: "Categories",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
             table.ForeignKey(
                    name: "FK_CategoryTranslations_Languages_LanguageId",
                    column: x => x.LanguageId,
                    principalTable: "Languages",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
          });

      migrationBuilder.CreateTable(
          name: "SettingTranslations",
          columns: table => new
          {
             Id = table.Column<int>(type: "integer", nullable: false)
                  .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
             LanguageId = table.Column<int>(type: "integer", nullable: false),
             EntityId = table.Column<int>(type: "integer", nullable: false),
             Value = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false)
          },
          constraints: table =>
          {
             table.PrimaryKey("PK_SettingTranslations", x => x.Id);
             table.ForeignKey(
                    name: "FK_SettingTranslations_Languages_LanguageId",
                    column: x => x.LanguageId,
                    principalTable: "Languages",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
             table.ForeignKey(
                    name: "FK_SettingTranslations_Settings_EntityId",
                    column: x => x.EntityId,
                    principalTable: "Settings",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "TechnologyTranslations",
          columns: table => new
          {
             Id = table.Column<int>(type: "integer", nullable: false)
                  .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
             LanguageId = table.Column<int>(type: "integer", nullable: false),
             EntityId = table.Column<int>(type: "integer", nullable: false),
             Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
             Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
          },
          constraints: table =>
          {
             table.PrimaryKey("PK_TechnologyTranslations", x => x.Id);
             table.ForeignKey(
                    name: "FK_TechnologyTranslations_Languages_LanguageId",
                    column: x => x.LanguageId,
                    principalTable: "Languages",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
             table.ForeignKey(
                    name: "FK_TechnologyTranslations_Technologies_EntityId",
                    column: x => x.EntityId,
                    principalTable: "Technologies",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateIndex(
          name: "IX_Assets_FileName",
          table: "Assets",
          column: "FileName");

      migrationBuilder.CreateIndex(
          name: "IX_AssetTranslations_EntityId_LanguageId",
          table: "AssetTranslations",
          columns: new[] { "EntityId", "LanguageId" },
          unique: true);

      migrationBuilder.CreateIndex(
          name: "IX_AssetTranslations_LanguageId",
          table: "AssetTranslations",
          column: "LanguageId");

      migrationBuilder.CreateIndex(
          name: "IX_Categories_Name",
          table: "Categories",
          column: "Name",
          unique: true,
          filter: "\"IsDeleted\" = false");

      migrationBuilder.CreateIndex(
          name: "IX_CategoryTranslations_EntityId_LanguageId",
          table: "CategoryTranslations",
          columns: new[] { "EntityId", "LanguageId" },
          unique: true);

      migrationBuilder.CreateIndex(
          name: "IX_CategoryTranslations_LanguageId",
          table: "CategoryTranslations",
          column: "LanguageId");

      migrationBuilder.CreateIndex(
          name: "IX_Language_Code_Unique_Active",
          table: "Languages",
          column: "Code",
          unique: true,
          filter: "\"IsDeleted\" = false");

      migrationBuilder.CreateIndex(
          name: "IX_Settings_Name",
          table: "Settings",
          column: "Name",
          unique: true);

      migrationBuilder.CreateIndex(
          name: "IX_SettingTranslations_EntityId_LanguageId",
          table: "SettingTranslations",
          columns: new[] { "EntityId", "LanguageId" },
          unique: true);

      migrationBuilder.CreateIndex(
          name: "IX_SettingTranslations_LanguageId",
          table: "SettingTranslations",
          column: "LanguageId");

      migrationBuilder.CreateIndex(
          name: "IX_Technologies_Name",
          table: "Technologies",
          column: "Name");

      migrationBuilder.CreateIndex(
          name: "IX_TechnologyTranslations_EntityId_LanguageId",
          table: "TechnologyTranslations",
          columns: new[] { "EntityId", "LanguageId" },
          unique: true);

      migrationBuilder.CreateIndex(
          name: "IX_TechnologyTranslations_LanguageId",
          table: "TechnologyTranslations",
          column: "LanguageId");
   }

   /// <inheritdoc />
   protected override void Down(MigrationBuilder migrationBuilder)
   {
      migrationBuilder.DropTable(
          name: "AssetTranslations");

      migrationBuilder.DropTable(
          name: "CategoryTranslations");

      migrationBuilder.DropTable(
          name: "SettingTranslations");

      migrationBuilder.DropTable(
          name: "TechnologyTranslations");

      migrationBuilder.DropTable(
          name: "Assets");

      migrationBuilder.DropTable(
          name: "Categories");

      migrationBuilder.DropTable(
          name: "Settings");

      migrationBuilder.DropTable(
          name: "Languages");

      migrationBuilder.DropTable(
          name: "Technologies");
   }
}
