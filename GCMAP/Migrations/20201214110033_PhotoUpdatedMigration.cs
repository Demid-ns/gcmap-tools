using Microsoft.EntityFrameworkCore.Migrations;

namespace GCMAP.Migrations
{
    public partial class PhotoUpdatedMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Photos_News_AttachedToNewsId",
                table: "Photos");

            migrationBuilder.DropIndex(
                name: "IX_Photos_AttachedToNewsId",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "AttachedToNewsId",
                table: "Photos");

            migrationBuilder.AddColumn<int>(
                name: "PhotoId",
                table: "News",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_News_PhotoId",
                table: "News",
                column: "PhotoId");

            migrationBuilder.AddForeignKey(
                name: "FK_News_Photos_PhotoId",
                table: "News",
                column: "PhotoId",
                principalTable: "Photos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_News_Photos_PhotoId",
                table: "News");

            migrationBuilder.DropIndex(
                name: "IX_News_PhotoId",
                table: "News");

            migrationBuilder.DropColumn(
                name: "PhotoId",
                table: "News");

            migrationBuilder.AddColumn<int>(
                name: "AttachedToNewsId",
                table: "Photos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Photos_AttachedToNewsId",
                table: "Photos",
                column: "AttachedToNewsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Photos_News_AttachedToNewsId",
                table: "Photos",
                column: "AttachedToNewsId",
                principalTable: "News",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
