using LinguisticStimulationPlanner.Data;
using LinguisticStimulationPlanner.Models;
using Microsoft.EntityFrameworkCore;

namespace LinguisticStimulationPlanner.Services
{
	public class LocationService
	{
		private readonly ApplicationDbContext _context;

		public LocationService(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<List<Location>> GetLocationsAsync()
		{
			return await _context.Locations.ToListAsync();
		}

		public async Task<Location> CreateLocationAsync(Location location)
		{
			_context.Locations.Add(location);
			await _context.SaveChangesAsync();
			return location;
		}

		public async Task<Location> UpdateLocationAsync(Location location)
		{
			_context.Locations.Update(location);
			await _context.SaveChangesAsync();
			return location;
		}

		public async Task DeleteLocationAsync(int id)
		{
			var location = await _context.Locations.FindAsync(id);
			if (location != null)
			{
				_context.Locations.Remove(location);
				await _context.SaveChangesAsync();
			}
		}
	}
}
