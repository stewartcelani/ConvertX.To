using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConvertX.To.Infrastructure.Persistence.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Conversions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FileNameWithoutExtension = table.Column<string>(type: "text", nullable: false),
                    SourceFormat = table.Column<string>(type: "text", nullable: false),
                    TargetFormat = table.Column<string>(type: "text", nullable: false),
                    ConvertedFileExtension = table.Column<string>(type: "text", nullable: false),
                    RequestDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    RequestCompleteDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
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
