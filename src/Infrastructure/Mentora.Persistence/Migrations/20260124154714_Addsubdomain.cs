using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mentora.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Addsubdomain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MenteeSubDomains",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubDomainId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenteeSubDomains", x => new { x.UserId, x.SubDomainId });
                    table.ForeignKey(
                        name: "FK_MenteeSubDomains_mentee_profile_UserId",
                        column: x => x.UserId,
                        principalTable: "mentee_profile",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MenteeSubDomains_subdomain_SubDomainId",
                        column: x => x.SubDomainId,
                        principalTable: "subdomain",
                        principalColumn: "subdomain_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MentorSubDomains",
                columns: table => new
                {
                    MentorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubDomainId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MentorSubDomains", x => new { x.MentorId, x.SubDomainId });
                    table.ForeignKey(
                        name: "FK_MentorSubDomains_mentor_profile_MentorId",
                        column: x => x.MentorId,
                        principalTable: "mentor_profile",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MentorSubDomains_subdomain_SubDomainId",
                        column: x => x.SubDomainId,
                        principalTable: "subdomain",
                        principalColumn: "subdomain_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MenteeSubDomains_SubDomainId",
                table: "MenteeSubDomains",
                column: "SubDomainId");

            migrationBuilder.CreateIndex(
                name: "IX_MentorSubDomains_SubDomainId",
                table: "MentorSubDomains",
                column: "SubDomainId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MenteeSubDomains");

            migrationBuilder.DropTable(
                name: "MentorSubDomains");
        }
    }
}
