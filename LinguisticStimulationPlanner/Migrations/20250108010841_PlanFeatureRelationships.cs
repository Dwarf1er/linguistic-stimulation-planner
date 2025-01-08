using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinguisticStimulationPlanner.Migrations
{
    /// <inheritdoc />
    public partial class PlanFeatureRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlanGoalToys",
                columns: table => new
                {
                    PlanId = table.Column<int>(type: "INTEGER", nullable: false),
                    GoalId = table.Column<int>(type: "INTEGER", nullable: false),
                    ToyId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanGoalToys", x => new { x.PlanId, x.GoalId, x.ToyId });
                    table.ForeignKey(
                        name: "FK_PlanGoalToys_Goals_GoalId",
                        column: x => x.GoalId,
                        principalTable: "Goals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlanGoalToys_PlanGoals_PlanId_GoalId",
                        columns: x => new { x.PlanId, x.GoalId },
                        principalTable: "PlanGoals",
                        principalColumns: new[] { "PlanId", "GoalId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlanGoalToys_Plans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "Plans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlanGoalToys_Toys_ToyId",
                        column: x => x.ToyId,
                        principalTable: "Toys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlanGoalToys_GoalId",
                table: "PlanGoalToys",
                column: "GoalId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanGoalToys_ToyId",
                table: "PlanGoalToys",
                column: "ToyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlanGoalToys");
        }
    }
}
