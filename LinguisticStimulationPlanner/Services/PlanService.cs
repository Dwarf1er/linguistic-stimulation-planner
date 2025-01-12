using LinguisticStimulationPlanner.Data;
using LinguisticStimulationPlanner.Models;
using Microsoft.EntityFrameworkCore;
using System;
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
            return await _context.Plans.ToListAsync();
        }

        private async Task<bool> IsValidGoalForPatientAsync(int patientId, int goalId)
        {
            return await _context.PatientGoals
                .AnyAsync(pg => pg.PatientId == patientId && pg.GoalId == goalId);
        }

        private async Task<bool> IsValidToyForGoalAsync(int goalId, int toyId)
        {
            return await _context.GoalToys
                .AnyAsync(gt => gt.GoalId == goalId && gt.ToyId == toyId);
        }

        public bool IsPlanModified(Plan plan)
        {
            var originalPlan = _context.Plans.AsNoTracking().FirstOrDefault(p => p.Id == plan.Id);

            if (originalPlan == null) return false;

            return originalPlan.StartDate != plan.StartDate
                   || originalPlan.EndDate != plan.EndDate
                   || originalPlan.Notes != plan.Notes
                   || originalPlan.PatientId != plan.PatientId
                   || originalPlan.PlanGoals.Count != plan.PlanGoals.Count
                   || originalPlan.PlanGoals.Any(pg => !plan.PlanGoals.Any(p => p.GoalId == pg.GoalId && p.ToyId == pg.ToyId));
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

        public Plan ClonePlan(Plan plan)
        {
            return new Plan
            {
                Id = plan.Id,
                PatientId = plan.PatientId,
                StartDate = plan.StartDate,
                EndDate = plan.EndDate,
                Notes = plan.Notes,
                PlanGoals = plan.PlanGoals.Select(pg => new PlanGoal
                {
                    GoalId = pg.GoalId,
                    ToyId = pg.ToyId
                }).ToList()
            };
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
    }
}
