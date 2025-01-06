using Microsoft.EntityFrameworkCore;

namespace LinguisticStimulationPlanner.Data
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{

		}
	}
}
