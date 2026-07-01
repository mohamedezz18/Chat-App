using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatingApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Likes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Likes",
                columns: table => new
                {
                    SourceMemberID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetMemberID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Likes", x => new { x.SourceMemberID, x.TargetMemberID });
                    table.ForeignKey(
                        name: "FK_Likes_Members_SourceMemberID",
                        column: x => x.SourceMemberID,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Likes_Members_TargetMemberID",
                        column: x => x.TargetMemberID,
                        principalTable: "Members",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Likes_TargetMemberID",
                table: "Likes",
                column: "TargetMemberID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Likes");
        }
    }
}
