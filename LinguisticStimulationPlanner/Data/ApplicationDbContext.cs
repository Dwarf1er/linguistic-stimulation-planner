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
		public DbSet<PlanGoalToy> PlanGoalToys { get; set; }
		public DbSet<Toy> Toys { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Patient>()
				.HasOne(p => p.Location)
				.WithMany(l => l.Patients)
				.HasForeignKey(p => p.LocationId)
				.OnDelete(DeleteBehavior.Restrict);

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

			modelBuilder.Entity<PlanGoal>()
				.HasKey(pg => new { pg.PlanId, pg.GoalId });

			modelBuilder.Entity<PlanGoal>()
				.HasOne(pg => pg.Plan)
				.WithMany(p => p.PlanGoals)
				.HasForeignKey(pg => pg.PlanId);

			modelBuilder.Entity<PlanGoal>()
				.HasOne(pg => pg.Goal)
				.WithMany(pg => pg.PlanGoals)
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

            modelBuilder.Entity<PlanGoalToy>()
                .HasKey(pgt => new { pgt.PlanId, pgt.GoalId, pgt.ToyId });

            modelBuilder.Entity<PlanGoalToy>()
                .HasOne(pgt => pgt.Plan)
                .WithMany(p => p.PlanGoalToys)
                .HasForeignKey(pgt => pgt.PlanId);

            modelBuilder.Entity<PlanGoalToy>()
                .HasOne(pgt => pgt.Goal)
                .WithMany(g => g.PlanGoalToys)
                .HasForeignKey(pgt => pgt.GoalId);

            modelBuilder.Entity<PlanGoalToy>()
                .HasOne(pgt => pgt.Toy)
                .WithMany(t => t.PlanGoalToys)
                .HasForeignKey(pgt => pgt.ToyId);
        }
	}
}
