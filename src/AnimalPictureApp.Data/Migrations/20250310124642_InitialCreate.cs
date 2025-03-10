using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnimalPictureApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnimalPictures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AnimalType = table.Column<string>(type: "TEXT", nullable: false),
                    ImageData = table.Column<byte[]>(type: "BLOB", nullable: false),
                    StoredAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ContentType = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimalPictures", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnimalPictures_AnimalType",
                table: "AnimalPictures",
                column: "AnimalType");

            migrationBuilder.CreateIndex(
                name: "IX_AnimalPictures_StoredAt",
                table: "AnimalPictures",
                column: "StoredAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnimalPictures");
        }
    }
}
