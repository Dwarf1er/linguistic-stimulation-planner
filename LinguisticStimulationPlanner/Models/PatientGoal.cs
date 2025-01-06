namespace LinguisticStimulationPlanner.Models
{
	public class PatientGoal
	{
		public int PatientId { get; set; }
		public Patient Patient { get; set; }

		public int GoalId { get; set; }
		public Goal Goal { get; set; }
	}
}
