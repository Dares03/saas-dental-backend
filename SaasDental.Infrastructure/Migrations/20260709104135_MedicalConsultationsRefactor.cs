using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SaasDental.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MedicalConsultationsRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BloodPressure",
                table: "ClinicalHistories");

            migrationBuilder.DropColumn(
                name: "CurrentIllnessReason",
                table: "ClinicalHistories");

            migrationBuilder.DropColumn(
                name: "CurrentIllnessStory",
                table: "ClinicalHistories");

            migrationBuilder.DropColumn(
                name: "GeneralClinicalExam",
                table: "ClinicalHistories");

            migrationBuilder.DropColumn(
                name: "HeartRate",
                table: "ClinicalHistories");

            migrationBuilder.DropColumn(
                name: "RespiratoryRate",
                table: "ClinicalHistories");

            migrationBuilder.DropColumn(
                name: "Temperature",
                table: "ClinicalHistories");

            migrationBuilder.AddColumn<string>(
                name: "BloodPressure",
                table: "ClinicalEvolutions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentIllnessReason",
                table: "ClinicalEvolutions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentIllnessStory",
                table: "ClinicalEvolutions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GeneralClinicalExam",
                table: "ClinicalEvolutions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HeartRate",
                table: "ClinicalEvolutions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RespiratoryRate",
                table: "ClinicalEvolutions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Temperature",
                table: "ClinicalEvolutions",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BloodPressure",
                table: "ClinicalEvolutions");

            migrationBuilder.DropColumn(
                name: "CurrentIllnessReason",
                table: "ClinicalEvolutions");

            migrationBuilder.DropColumn(
                name: "CurrentIllnessStory",
                table: "ClinicalEvolutions");

            migrationBuilder.DropColumn(
                name: "GeneralClinicalExam",
                table: "ClinicalEvolutions");

            migrationBuilder.DropColumn(
                name: "HeartRate",
                table: "ClinicalEvolutions");

            migrationBuilder.DropColumn(
                name: "RespiratoryRate",
                table: "ClinicalEvolutions");

            migrationBuilder.DropColumn(
                name: "Temperature",
                table: "ClinicalEvolutions");

            migrationBuilder.AddColumn<string>(
                name: "BloodPressure",
                table: "ClinicalHistories",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentIllnessReason",
                table: "ClinicalHistories",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentIllnessStory",
                table: "ClinicalHistories",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GeneralClinicalExam",
                table: "ClinicalHistories",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HeartRate",
                table: "ClinicalHistories",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RespiratoryRate",
                table: "ClinicalHistories",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Temperature",
                table: "ClinicalHistories",
                type: "text",
                nullable: true);
        }
    }
}
