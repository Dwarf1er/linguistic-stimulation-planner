using Microsoft.EntityFrameworkCore;
using LinguisticStimulationPlanner.Models;

namespace LinguisticStimulationPlanner.Data
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
        {
        }

		public DbSet<Goal> Goals { get; set; }
		public DbSet<GoalToy> GoalToys { get; set; }
		public DbSet<Location> Locations { get; set; }
		public DbSet<Patient> Patients { get; set; }
		public DbSet<PatientGoal> PatientGoals { get; set; }
		public DbSet<Plan> Plans { get; set; }
		public DbSet<PlanGoal> PlanGoals { get; set; }
		public DbSet<Toy> Toys { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Patient>()
				.HasOne(p => p.Location)
				.WithMany(l => l.Patients)
				.HasForeignKey(p => p.LocationId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<PatientGoal>()
				.HasKey(pg => new { pg.PatientId, pg.GoalId });

			modelBuilder.Entity<PatientGoal>()
				.HasOne(pg => pg.Patient)
				.WithMany(p => p.PatientGoals)
				.HasForeignKey(pg => pg.PatientId);

			modelBuilder.Entity<PatientGoal>()
				.HasOne(pg => pg.Goal)
				.WithMany(p => p.PatientGoals)
				.HasForeignKey(pg => pg.GoalId);

			modelBuilder.Entity<GoalToy>()
				.HasKey(gt => new { gt.GoalId, gt.ToyId });

			modelBuilder.Entity<GoalToy>()
				.HasOne(gt => gt.Goal)
				.WithMany(g => g.GoalToys)
				.HasForeignKey(gt => gt.GoalId);

			modelBuilder.Entity<GoalToy>()
				.HasOne(gt => gt.Toy)
				.WithMany(t => t.GoalToys)
				.HasForeignKey(gt => gt.ToyId);

            modelBuilder.Entity<Plan>()
                .HasOne(p => p.Patient)
                .WithMany(pat => pat.Plans)
                .HasForeignKey(p => p.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PlanGoal>()
                .HasOne(pg => pg.Plan)
                .WithMany(p => p.PlanGoals)
                .HasForeignKey(pg => pg.PlanId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PlanGoal>()
                .HasOne(pg => pg.Goal)
                .WithMany()
                .HasForeignKey(pg => pg.GoalId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PlanGoal>()
                .HasOne(pg => pg.Toy)
                .WithMany()
                .HasForeignKey(pg => pg.ToyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
	}
}
