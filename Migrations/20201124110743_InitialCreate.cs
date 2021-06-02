using Microsoft.EntityFrameworkCore.Migrations;

namespace SOCMonitorV2.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbl_Employee",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ECNo = table.Column<string>(type: "varchar(50)", nullable: false),
                    Name = table.Column<string>(type: "varchar(100)", nullable: false),
                    Designation = table.Column<string>(type: "varchar(50)", nullable: false),
                    Role = table.Column<string>(type: "varchar(100)", nullable: true),
                    IsExecutive = table.Column<bool>(type: "bit", nullable: false),
                    Location = table.Column<string>(type: "varchar(100)", nullable: false),
                    Organisation = table.Column<string>(type: "varchar(50)", nullable: false),
                    ReportingAuth = table.Column<string>(type: "varchar(100)", nullable: true),
                    Mobile = table.Column<string>(type: "varchar(10)", nullable: false),
                    Email = table.Column<string>(type: "varchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_Employee", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_Employee");
        }
    }
}
