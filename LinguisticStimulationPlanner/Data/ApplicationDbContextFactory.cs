using LinguisticStimulationPlanner.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LinguisticStimulationPlanner.Data
{
	public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
	{
		public ApplicationDbContext CreateDbContext(string[] args = null)
		{
			var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
			optionsBuilder.UseSqlite($"Data Source={DatabaseSetup.DevelopmentDatabasePath}");

			return new ApplicationDbContext(optionsBuilder.Options);
		}
	}
}
