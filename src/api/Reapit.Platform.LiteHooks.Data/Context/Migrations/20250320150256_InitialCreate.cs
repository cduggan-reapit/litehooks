using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reapit.Platform.LiteHooks.Data.Context.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "examples",
                columns: table => new
                {
                    id = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    cursor = table.Column<long>(type: "INTEGER", nullable: false),
                    created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    modified = table.Column<DateTime>(type: "TEXT", nullable: false),
                    deleted = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_examples", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_examples_created",
                table: "examples",
                column: "created");

            migrationBuilder.CreateIndex(
                name: "IX_examples_deleted",
                table: "examples",
                column: "deleted");

            migrationBuilder.CreateIndex(
                name: "IX_examples_modified",
                table: "examples",
                column: "modified");

            migrationBuilder.CreateIndex(
                name: "IX_examples_name_deleted",
                table: "examples",
                columns: new[] { "name", "deleted" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "examples");
        }
    }
}
