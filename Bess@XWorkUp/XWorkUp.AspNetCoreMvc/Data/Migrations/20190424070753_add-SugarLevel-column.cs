using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace XWorkUp.AspNetCoreMvc.Data.Migrations
{
    public partial class addSugarLevelcolumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ShortDescription",
                table: "Pies",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SugarLevel",
                table: "Pies",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "RecipeInformation",
                columns: table => new
                {
                    RecipeInformationId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PreparationDirections = table.Column<string>(nullable: true),
                    Duration = table.Column<int>(nullable: false),
                    PieId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeInformation", x => x.RecipeInformationId);
                    table.ForeignKey(
                        name: "FK_RecipeInformation_Pies_PieId",
                        column: x => x.PieId,
                        principalTable: "Pies",
                        principalColumn: "PieId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecipeInformation_PieId",
                table: "RecipeInformation",
                column: "PieId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecipeInformation");

            migrationBuilder.DropColumn(
                name: "SugarLevel",
                table: "Pies");

            migrationBuilder.AlterColumn<string>(
                name: "ShortDescription",
                table: "Pies",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100,
                oldNullable: true);
        }
    }
}
