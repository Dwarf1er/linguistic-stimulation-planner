namespace LinguisticStimulationPlanner.Models
{
    public class PlanGoalToy
    {
        public int PlanId { get; set; }
        public Plan Plan { get; set; }

        public int GoalId { get; set; }
        public Goal Goal { get; set; }

        public int ToyId { get; set; }
        public Toy Toy { get; set; }
    }
}
