using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StargateAPI.Migrations
{
    /// <inheritdoc />
    public partial class AdditionalData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AstronautDetail",
                keyColumn: "Id",
                keyValue: 1,
                column: "CareerStartDate",
                value: new DateTime(2025, 4, 29, 19, 23, 13, 889, DateTimeKind.Local).AddTicks(8357));

            migrationBuilder.UpdateData(
                table: "AstronautDuty",
                keyColumn: "Id",
                keyValue: 1,
                column: "DutyStartDate",
                value: new DateTime(2025, 4, 29, 19, 23, 13, 889, DateTimeKind.Local).AddTicks(8405));

            migrationBuilder.InsertData(
                table: "AstronautDuty",
                columns: new[] { "Id", "DutyEndDate", "DutyStartDate", "DutyTitle", "PersonId", "Rank" },
                values: new object[] { 2, new DateTime(2025, 3, 30, 19, 23, 13, 889, DateTimeKind.Local).AddTicks(8412), new DateTime(2020, 4, 29, 19, 23, 13, 889, DateTimeKind.Local).AddTicks(8408), "Junior Commander", 1, "0LT" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AstronautDuty",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.UpdateData(
                table: "AstronautDetail",
                keyColumn: "Id",
                keyValue: 1,
                column: "CareerStartDate",
                value: new DateTime(2025, 4, 28, 22, 44, 49, 710, DateTimeKind.Local).AddTicks(6407));

            migrationBuilder.UpdateData(
                table: "AstronautDuty",
                keyColumn: "Id",
                keyValue: 1,
                column: "DutyStartDate",
                value: new DateTime(2025, 4, 28, 22, 44, 49, 710, DateTimeKind.Local).AddTicks(6456));
        }
    }
}
