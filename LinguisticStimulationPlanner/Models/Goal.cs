namespace LinguisticStimulationPlanner.Models
{
	public class Goal
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }

		public ICollection<PatientGoal> PatientGoals { get; set; } = new List<PatientGoal>();
		public ICollection<GoalToy> GoalToys { get; set; } = new List<GoalToy>();

		public bool IsValidGoal()
		{
			return !(string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Description));
		}
	}
}
