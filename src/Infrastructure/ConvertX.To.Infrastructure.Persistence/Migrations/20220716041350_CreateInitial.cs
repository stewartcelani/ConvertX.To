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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileNameWithoutExtension = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SourceFormat = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TargetFormat = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConvertedFileExtension = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    RequestCompleteDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Downloads = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    DateDeleted = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "getdate()"),
                    DateUpdated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
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
