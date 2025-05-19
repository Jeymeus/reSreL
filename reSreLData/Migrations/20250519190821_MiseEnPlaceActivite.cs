using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace reSreLData.Migrations
{
    /// <inheritdoc />
    public partial class MiseEnPlaceActivite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RessourceId",
                table: "Games",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Games_RessourceId",
                table: "Games",
                column: "RessourceId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Ressources_RessourceId",
                table: "Games",
                column: "RessourceId",
                principalTable: "Ressources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_Ressources_RessourceId",
                table: "Games");

            migrationBuilder.DropIndex(
                name: "IX_Games_RessourceId",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "RessourceId",
                table: "Games");
        }
    }
}
