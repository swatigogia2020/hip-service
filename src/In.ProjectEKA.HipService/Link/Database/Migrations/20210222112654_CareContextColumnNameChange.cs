using Microsoft.EntityFrameworkCore.Migrations;

namespace In.ProjectEKA.HipService.Link.Database.Migrations
{
    public partial class CareContextColumnNameChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn("CareContextNumber", "CareContext", "CareContextName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn("CareContextName", "CareContext", "CareContextNumber");
        }
    }
}
