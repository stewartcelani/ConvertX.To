using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConvertX.To.Infrastructure.Persistence.Migrations
{
    public partial class CreateInitial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Conversions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceFormat = table.Column<string>(type: "text", nullable: false),
                    TargetFormat = table.Column<string>(type: "text", nullable: false),
                    ConvertedFormat = table.Column<string>(type: "text", nullable: false),
                    SourceMegabytes = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    ConvertedMegabytes = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    DateRequestReceived = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DateRequestCompleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    RequestSeconds = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Downloads = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    DateUpdated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversions", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Conversions");
        }
    }
}
