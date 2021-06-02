using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SOCMonitorV2.Migrations.Shift
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbl_Officials",
                columns: table => new
                {
                    ECNo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_Officials", x => x.ECNo);
                });

            migrationBuilder.CreateTable(
                name: "tbl_Shift",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ECNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DutyDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ShiftTimeBase = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DutyStartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DutyEndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_Shift", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_Officials");

            migrationBuilder.DropTable(
                name: "tbl_Shift");
        }
    }
}
