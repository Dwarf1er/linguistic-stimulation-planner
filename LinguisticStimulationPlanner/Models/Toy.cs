namespace LinguisticStimulationPlanner.Models
{
	public class Toy
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public bool InInventory { get; set; }

		public ICollection<GoalToy> GoalToys { get; set; } = new List<GoalToy>();
		public ICollection<PlanGoalToy> PlanGoalToys { get; set; } = new List<PlanGoalToy>();

		public bool IsValidToy()
		{
			return !(string.IsNullOrEmpty(Name));
		}
	}
}
