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
            return await _context.Toys.ToListAsync();
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
    }
}
