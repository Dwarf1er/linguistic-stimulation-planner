using LinguisticStimulationPlanner.Data;
using LinguisticStimulationPlanner.Models;
using Microsoft.EntityFrameworkCore;

namespace LinguisticStimulationPlanner.Services
{
    public class ToyService
    {
        private readonly ApplicationDbContext _context;

        public ToyService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Toy>> GetToysAsync()
        {
            return await _context.Toys.Include(t => t.GoalToys).ThenInclude(gt => gt.Goal).ToListAsync();
        }

        public async Task<Toy> CreateToyAsync(Toy toy)
        {
            _context.Toys.Add(toy);
            await _context.SaveChangesAsync();
            return toy;
        }

        public async Task<Toy> UpdateToyAsync(Toy toy)
        {
            _context.Toys.Update(toy);
            await _context.SaveChangesAsync();
            return toy;
        }

        public async Task DeleteToyAsync(int id)
        {
            var toy = await _context.Toys.FindAsync(id);
            if (toy != null)
            {
                _context.Toys.Remove(toy);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AssignGoalsToToy(Toy toy, List<Goal> goals)
        {
            foreach (var goal in goals)
            {
                if (!toy.GoalToys.Any(gt => gt.GoalId == goal.Id))
                {
                    toy.GoalToys.Add(new GoalToy
                    {
                        GoalId = goal.Id,
                        ToyId = toy.Id
                    });
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task DeassignGoalsFromToy(Toy toy, List<Goal> goals)
        {
            var goalsToRemove = toy.GoalToys
                .Where(gt => !goals.Any(g => g.Id == gt.GoalId))
                .ToList();

            foreach (var goalToRemove in goalsToRemove)
            {
                _context.GoalToys.Remove(goalToRemove);
            }

            await _context.SaveChangesAsync();
        }
    }
}
