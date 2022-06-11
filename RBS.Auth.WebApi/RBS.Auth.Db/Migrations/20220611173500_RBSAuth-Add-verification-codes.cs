using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RBS.Auth.Db.Migrations
{
    public partial class RBSAuthAddverificationcodes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "UserCredentials",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "UserVerificationCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpireDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserVerificationCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserVerificationCodes_UserCredentials_UserId",
                        column: x => x.UserId,
                        principalTable: "UserCredentials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserVerificationCodes_UserId",
                table: "UserVerificationCodes",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserVerificationCodes");

            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "UserCredentials");
        }
    }
}
