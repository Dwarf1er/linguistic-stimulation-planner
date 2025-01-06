namespace LinguisticStimulationPlanner.Models
{
	public class PlanGoal
	{
		public int PlanId { get; set; }
		public Plan Plan { get; set; }

		public int GoalId { get; set; }
		public Goal Goal { get; set; }
	}
}
