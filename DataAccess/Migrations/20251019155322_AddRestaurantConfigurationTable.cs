using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddRestaurantConfigurationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RestaurantConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestaurantConfigurations", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "RestaurantConfigurations",
                columns: new[] { "Id", "Description", "Key", "UpdatedAt", "Value" },
                values: new object[,]
                {
                    { 1, "Capacità massima per fascia oraria", "MaxCapacityPerSlot", null, "50" },
                    { 2, "Orari pranzo disponibili", "LunchTimes", null, "12:00,12:30,13:00,13:30,14:00" },
                    { 3, "Orari cena disponibili", "DinnerTimes", null, "19:00,19:30,20:00,20:30,21:00,21:30,22:00" },
                    { 4, "Nome del ristorante", "RestaurantName", null, "Roma Antica" },
                    { 5, "Telefono ristorante", "RestaurantPhone", null, "+39 02 1234567" },
                    { 6, "Email ristorante", "RestaurantEmail", null, "info@romaantica.it" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantConfigurations_Key",
                table: "RestaurantConfigurations",
                column: "Key",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RestaurantConfigurations");
        }
    }
}
