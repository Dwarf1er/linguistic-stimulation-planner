namespace LinguisticStimulationPlanner.Models
{
	public class Patient
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Language { get; set; }
		public string Email { get; set; }
		public int LocationId { get; set; }
		public Location Location { get; set; }

		public ICollection<PatientGoal> PatientGoals { get; set; } = new List<PatientGoal>();
		public ICollection<Plan> Plans { get; set; } = new List<Plan>();
	}
}
