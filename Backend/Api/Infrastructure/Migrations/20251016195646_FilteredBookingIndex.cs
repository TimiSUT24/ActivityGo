using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FilteredBookingIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Bookings_ActivityOccurrenceId_UserId",
                table: "Bookings");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_ActivityOccurrenceId_UserId",
                table: "Bookings",
                columns: new[] { "ActivityOccurrenceId", "UserId" },
                unique: true,
                filter: "[Status] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Bookings_ActivityOccurrenceId_UserId",
                table: "Bookings");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_ActivityOccurrenceId_UserId",
                table: "Bookings",
                columns: new[] { "ActivityOccurrenceId", "UserId" },
                unique: true);
        }
    }
}
