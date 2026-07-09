using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SaasDental.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddClinicalEvolutions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClinicalEvolutions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    ClinicalHistoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    ToothId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClinicalEvolutions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClinicalEvolutions_ClinicalHistories_ClinicalHistoryId",
                        column: x => x.ClinicalHistoryId,
                        principalTable: "ClinicalHistories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClinicalEvolutions_Teeth_ToothId",
                        column: x => x.ToothId,
                        principalTable: "Teeth",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClinicalEvolutions_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClinicalEvolutions_ClinicalHistoryId",
                table: "ClinicalEvolutions",
                column: "ClinicalHistoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ClinicalEvolutions_CreatedByUserId",
                table: "ClinicalEvolutions",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ClinicalEvolutions_ToothId",
                table: "ClinicalEvolutions",
                column: "ToothId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClinicalEvolutions");
        }
    }
}
