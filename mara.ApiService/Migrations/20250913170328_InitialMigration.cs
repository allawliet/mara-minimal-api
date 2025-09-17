using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace mara.ApiService.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Roles = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeatherForecasts",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    TemperatureC = table.Column<int>(type: "int", nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Location = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherForecasts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Todos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Todos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Todos_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "PasswordHash", "Roles", "UpdatedAt", "Username" },
                values: new object[] { 1, new DateTime(2025, 9, 13, 17, 3, 27, 781, DateTimeKind.Utc).AddTicks(6982), "admin@mara.com", "$2a$11$sf3pQ60j5DQv2MFRJJkgJuyC2IlkZk5uhRMNvEWOUZoxsfjI5jafS", "[]", new DateTime(2025, 9, 13, 17, 3, 27, 781, DateTimeKind.Utc).AddTicks(6989), "admin" });

            migrationBuilder.InsertData(
                table: "WeatherForecasts",
                columns: new[] { "Id", "Date", "Location", "Summary", "TemperatureC" },
                values: new object[,]
                {
                    { "seed_berlin_0", new DateOnly(2025, 9, 14), "Berlin", "Cool", 1 },
                    { "seed_berlin_1", new DateOnly(2025, 9, 15), "Berlin", "Sweltering", 18 },
                    { "seed_berlin_2", new DateOnly(2025, 9, 16), "Berlin", "Warm", 3 },
                    { "seed_berlin_3", new DateOnly(2025, 9, 17), "Berlin", "Chilly", 3 },
                    { "seed_berlin_4", new DateOnly(2025, 9, 18), "Berlin", "Balmy", 27 },
                    { "seed_berlin_5", new DateOnly(2025, 9, 19), "Berlin", "Cool", 15 },
                    { "seed_berlin_6", new DateOnly(2025, 9, 20), "Berlin", "Bracing", 20 },
                    { "seed_berlin_7", new DateOnly(2025, 9, 21), "Berlin", "Warm", -7 },
                    { "seed_berlin_8", new DateOnly(2025, 9, 22), "Berlin", "Chilly", 23 },
                    { "seed_berlin_9", new DateOnly(2025, 9, 23), "Berlin", "Hot", 25 },
                    { "seed_london_0", new DateOnly(2025, 9, 14), "London", "Mild", 14 },
                    { "seed_london_1", new DateOnly(2025, 9, 15), "London", "Freezing", 24 },
                    { "seed_london_2", new DateOnly(2025, 9, 16), "London", "Mild", 32 },
                    { "seed_london_3", new DateOnly(2025, 9, 17), "London", "Scorching", -4 },
                    { "seed_london_4", new DateOnly(2025, 9, 18), "London", "Warm", 7 },
                    { "seed_london_5", new DateOnly(2025, 9, 19), "London", "Hot", 24 },
                    { "seed_london_6", new DateOnly(2025, 9, 20), "London", "Bracing", 7 },
                    { "seed_london_7", new DateOnly(2025, 9, 21), "London", "Freezing", 7 },
                    { "seed_london_8", new DateOnly(2025, 9, 22), "London", "Freezing", 11 },
                    { "seed_london_9", new DateOnly(2025, 9, 23), "London", "Balmy", -9 },
                    { "seed_new york_0", new DateOnly(2025, 9, 14), "New York", "Balmy", -6 },
                    { "seed_new york_1", new DateOnly(2025, 9, 15), "New York", "Freezing", 2 },
                    { "seed_new york_2", new DateOnly(2025, 9, 16), "New York", "Bracing", 7 },
                    { "seed_new york_3", new DateOnly(2025, 9, 17), "New York", "Bracing", 1 },
                    { "seed_new york_4", new DateOnly(2025, 9, 18), "New York", "Freezing", 30 },
                    { "seed_new york_5", new DateOnly(2025, 9, 19), "New York", "Bracing", -4 },
                    { "seed_new york_6", new DateOnly(2025, 9, 20), "New York", "Warm", -8 },
                    { "seed_new york_7", new DateOnly(2025, 9, 21), "New York", "Warm", 25 },
                    { "seed_new york_8", new DateOnly(2025, 9, 22), "New York", "Cool", 9 },
                    { "seed_new york_9", new DateOnly(2025, 9, 23), "New York", "Chilly", 12 },
                    { "seed_sydney_0", new DateOnly(2025, 9, 14), "Sydney", "Mild", 10 },
                    { "seed_sydney_1", new DateOnly(2025, 9, 15), "Sydney", "Bracing", 30 },
                    { "seed_sydney_2", new DateOnly(2025, 9, 16), "Sydney", "Mild", 20 },
                    { "seed_sydney_3", new DateOnly(2025, 9, 17), "Sydney", "Cool", 10 },
                    { "seed_sydney_4", new DateOnly(2025, 9, 18), "Sydney", "Chilly", 3 },
                    { "seed_sydney_5", new DateOnly(2025, 9, 19), "Sydney", "Freezing", 19 },
                    { "seed_sydney_6", new DateOnly(2025, 9, 20), "Sydney", "Chilly", 21 },
                    { "seed_sydney_7", new DateOnly(2025, 9, 21), "Sydney", "Sweltering", -6 },
                    { "seed_sydney_8", new DateOnly(2025, 9, 22), "Sydney", "Cool", -5 },
                    { "seed_sydney_9", new DateOnly(2025, 9, 23), "Sydney", "Balmy", -7 },
                    { "seed_tokyo_0", new DateOnly(2025, 9, 14), "Tokyo", "Freezing", 14 },
                    { "seed_tokyo_1", new DateOnly(2025, 9, 15), "Tokyo", "Warm", -3 },
                    { "seed_tokyo_2", new DateOnly(2025, 9, 16), "Tokyo", "Warm", 26 },
                    { "seed_tokyo_3", new DateOnly(2025, 9, 17), "Tokyo", "Bracing", -1 },
                    { "seed_tokyo_4", new DateOnly(2025, 9, 18), "Tokyo", "Sweltering", 30 },
                    { "seed_tokyo_5", new DateOnly(2025, 9, 19), "Tokyo", "Warm", 25 },
                    { "seed_tokyo_6", new DateOnly(2025, 9, 20), "Tokyo", "Warm", 9 },
                    { "seed_tokyo_7", new DateOnly(2025, 9, 21), "Tokyo", "Chilly", 19 },
                    { "seed_tokyo_8", new DateOnly(2025, 9, 22), "Tokyo", "Cool", 18 },
                    { "seed_tokyo_9", new DateOnly(2025, 9, 23), "Tokyo", "Hot", -5 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Todos_UserId",
                table: "Todos",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Todos");

            migrationBuilder.DropTable(
                name: "WeatherForecasts");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
