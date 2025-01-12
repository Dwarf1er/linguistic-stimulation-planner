using LinguisticStimulationPlanner.Data;
using LinguisticStimulationPlanner.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinguisticStimulationPlanner.Services
{
    public class PlanService
    {
        private readonly ApplicationDbContext _context;

        public PlanService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Plan>> GetPlansAsync()
        {
            return await _context.Plans.Include(p => p.PlanGoals)
                .ThenInclude(pg => pg.Goal)
                .ThenInclude(g => g.GoalToys)
                .ThenInclude(gt => gt.Toy)
                .ToListAsync();
        }

        public async Task UpdatePlanAsync(Plan plan)
        {
            var existingPlan = await _context.Plans.Include(p => p.PlanGoals)
                                                    .ThenInclude(pg => pg.Goal)
                                                    .FirstOrDefaultAsync(p => p.Id == plan.Id);

            if (existingPlan != null)
            {
                existingPlan.StartDate = plan.StartDate;
                existingPlan.EndDate = plan.EndDate;
                existingPlan.Notes = plan.Notes;
                existingPlan.PatientId = plan.PatientId;

                foreach (var updatedPlanGoal in plan.PlanGoals)
                {
                    var existingPlanGoal = existingPlan.PlanGoals.FirstOrDefault(pg => pg.GoalId == updatedPlanGoal.GoalId);
                    if (existingPlanGoal != null)
                    {
                        existingPlanGoal.ToyId = updatedPlanGoal.ToyId;
                    }
                    else
                    {
                        existingPlan.PlanGoals.Add(updatedPlanGoal);
                    }
                }

                var deletedPlanGoals = existingPlan.PlanGoals.Where(pg => !plan.PlanGoals.Any(p => p.GoalId == pg.GoalId)).ToList();
                foreach (var deletedGoal in deletedPlanGoals)
                {
                    existingPlan.PlanGoals.Remove(deletedGoal);
                }

                _context.Plans.Update(existingPlan);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeletePlanAsync(int planId)
        {
            var planToDelete = await _context.Plans.Include(p => p.PlanGoals).FirstOrDefaultAsync(p => p.Id == planId);
            
            if (planToDelete != null)
            {
                _context.PlanGoals.RemoveRange(planToDelete.PlanGoals);
                
                _context.Plans.Remove(planToDelete);
                
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Plan> CreatePlanAsync(Plan plan)
        {
            _context.Plans.Add(plan);
            await _context.SaveChangesAsync();
            return plan;
        }

        public async Task AssignPlanGoalToPlan(Plan plan, List<PlanGoal> planGoalsToAdd)
        {
            foreach (var planGoal in planGoalsToAdd)
            {
                if (!plan.PlanGoals.Any(pg => pg.GoalId == planGoal.GoalId))
                {
                    plan.PlanGoals.Add(planGoal);
                }
            }
            _context.Plans.Update(plan);
            await _context.SaveChangesAsync();
        }

        public async Task DeassignPlanGoalFromPlan(Plan plan, List<PlanGoal> planGoalsToRemove)
        {
            foreach (var planGoal in planGoalsToRemove)
            {
                var existingGoal = plan.PlanGoals.FirstOrDefault(pg => pg.GoalId == planGoal.GoalId);
                if (existingGoal != null)
                {
                    plan.PlanGoals.Remove(existingGoal);
                }
            }
            _context.Plans.Update(plan);
            await _context.SaveChangesAsync();
        }
    }
}
