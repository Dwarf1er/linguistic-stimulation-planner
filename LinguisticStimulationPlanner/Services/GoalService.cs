using LinguisticStimulationPlanner.Data;
using LinguisticStimulationPlanner.Models;
using Microsoft.EntityFrameworkCore;

namespace LinguisticStimulationPlanner.Services
{
    public class GoalService
    {
        private readonly ApplicationDbContext _context;

        public GoalService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Goal>> GetGoalsAsync()
        {
            return await _context.Goals.ToListAsync();
        }

        public async Task<Goal> CreateGoalAsync(Goal goal)
        {
            _context.Goals.Add(goal);
            await _context.SaveChangesAsync();
            return goal;
        }

        public async Task<Goal> UpdateGoalAsync(Goal goal)
        {
            _context.Goals.Update(goal);
            await _context.SaveChangesAsync();
            return goal;
        }

        public async Task DeleteGoalAsync(int id)
        {
            var goal = await _context.Goals.FindAsync(id);
            if (goal != null)
            {
                _context.Goals.Remove(goal);
                await _context.SaveChangesAsync();
            }
        }
    }
}
