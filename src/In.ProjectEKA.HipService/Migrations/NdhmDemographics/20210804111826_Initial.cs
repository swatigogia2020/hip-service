using Microsoft.EntityFrameworkCore.Migrations;

namespace In.ProjectEKA.HipService.Migrations.NdhmDemographics
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NdhmDemographics",
                columns: table => new
                {
                    HealthId = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Gender = table.Column<string>(nullable: true),
                    DateOfBirth = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NdhmDemographics", x => x.HealthId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NdhmDemographics");
        }
    }
}
