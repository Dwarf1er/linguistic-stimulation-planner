namespace LinguisticStimulationPlanner.Models
{
	public class GoalToy
	{
		public int GoalId { get; set; }
		public Goal Goal { get; set; }

		public int ToyId { get; set; }
		public Toy Toy { get; set; }
	}
}
