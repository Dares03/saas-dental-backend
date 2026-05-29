using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SaasDental.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddClinicalModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClinicalHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Occupation = table.Column<string>(type: "text", nullable: true),
                    Religion = table.Column<string>(type: "text", nullable: true),
                    MaritalStatus = table.Column<string>(type: "text", nullable: true),
                    PlaceOfOrigin = table.Column<string>(type: "text", nullable: true),
                    CompanionName = table.Column<string>(type: "text", nullable: true),
                    CurrentIllnessReason = table.Column<string>(type: "text", nullable: true),
                    CurrentIllnessStory = table.Column<string>(type: "text", nullable: true),
                    FamilyHistory = table.Column<string>(type: "text", nullable: true),
                    PersonalHistory = table.Column<string>(type: "text", nullable: true),
                    BloodPressure = table.Column<string>(type: "text", nullable: true),
                    HeartRate = table.Column<string>(type: "text", nullable: true),
                    Temperature = table.Column<string>(type: "text", nullable: true),
                    RespiratoryRate = table.Column<string>(type: "text", nullable: true),
                    GeneralClinicalExam = table.Column<string>(type: "text", nullable: true),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClinicalHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClinicalHistories_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Odontograms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VersionType = table.Column<int>(type: "integer", nullable: false),
                    Specifications = table.Column<string>(type: "text", nullable: true),
                    Observations = table.Column<string>(type: "text", nullable: true),
                    ClinicalHistoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Odontograms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Odontograms_ClinicalHistories_ClinicalHistoryId",
                        column: x => x.ClinicalHistoryId,
                        principalTable: "ClinicalHistories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Teeth",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ToothNumber = table.Column<int>(type: "integer", nullable: false),
                    OdontogramId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teeth", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Teeth_Odontograms_OdontogramId",
                        column: x => x.OdontogramId,
                        principalTable: "Odontograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ToothSurfaces",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SurfaceType = table.Column<int>(type: "integer", nullable: false),
                    ToothId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToothSurfaces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ToothSurfaces_Teeth_ToothId",
                        column: x => x.ToothId,
                        principalTable: "Teeth",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClinicalFindings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FindingType = table.Column<string>(type: "text", nullable: false),
                    Color = table.Column<int>(type: "integer", nullable: false),
                    Nomenclature = table.Column<string>(type: "text", nullable: false),
                    ToothId = table.Column<Guid>(type: "uuid", nullable: true),
                    ToothSurfaceId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClinicalFindings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClinicalFindings_Teeth_ToothId",
                        column: x => x.ToothId,
                        principalTable: "Teeth",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClinicalFindings_ToothSurfaces_ToothSurfaceId",
                        column: x => x.ToothSurfaceId,
                        principalTable: "ToothSurfaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClinicalFindings_ToothId",
                table: "ClinicalFindings",
                column: "ToothId");

            migrationBuilder.CreateIndex(
                name: "IX_ClinicalFindings_ToothSurfaceId",
                table: "ClinicalFindings",
                column: "ToothSurfaceId");

            migrationBuilder.CreateIndex(
                name: "IX_ClinicalHistories_PatientId",
                table: "ClinicalHistories",
                column: "PatientId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Odontograms_ClinicalHistoryId",
                table: "Odontograms",
                column: "ClinicalHistoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Teeth_OdontogramId",
                table: "Teeth",
                column: "OdontogramId");

            migrationBuilder.CreateIndex(
                name: "IX_ToothSurfaces_ToothId",
                table: "ToothSurfaces",
                column: "ToothId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClinicalFindings");

            migrationBuilder.DropTable(
                name: "ToothSurfaces");

            migrationBuilder.DropTable(
                name: "Teeth");

            migrationBuilder.DropTable(
                name: "Odontograms");

            migrationBuilder.DropTable(
                name: "ClinicalHistories");
        }
    }
}
