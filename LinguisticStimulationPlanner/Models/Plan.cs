using System;
using System.Collections.Generic;

namespace LinguisticStimulationPlanner.Models
{
    public class Plan
    {
        public int Id { get; set; }
        public int? PatientId { get; set; }
        public Patient Patient { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Notes { get; set; }

        public ICollection<PlanGoal> PlanGoals { get; set; } = new List<PlanGoal>();

        public bool IsValidPlan()
        {
			return (PatientId != null && StartDate != null && EndDate != null);
        }
    }
}
