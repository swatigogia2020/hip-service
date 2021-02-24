using Microsoft.EntityFrameworkCore.Migrations;

namespace In.ProjectEKA.HipService.Link.Database.Migrations
{
    public partial class CareContextMap : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CareContextMap",
                columns: table => new
                {
                    CareContextName = table.Column<string>(nullable: false),
                    CareContextType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CareContextMap", x => x.CareContextName);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CareContextMap");
        }
    }
}
