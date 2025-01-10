﻿// <auto-generated />
using LinguisticStimulationPlanner.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace LinguisticStimulationPlanner.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.0");

            modelBuilder.Entity("LinguisticStimulationPlanner.Models.Goal", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Goals");
                });

            modelBuilder.Entity("LinguisticStimulationPlanner.Models.GoalToy", b =>
                {
                    b.Property<int>("GoalId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ToyId")
                        .HasColumnType("INTEGER");

                    b.HasKey("GoalId", "ToyId");

                    b.HasIndex("ToyId");

                    b.ToTable("GoalToys");
                });

            modelBuilder.Entity("LinguisticStimulationPlanner.Models.Location", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Address")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Locations");
                });

            modelBuilder.Entity("LinguisticStimulationPlanner.Models.Patient", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Email")
                        .HasColumnType("TEXT");

                    b.Property<string>("Language")
                        .HasColumnType("TEXT");

                    b.Property<int>("LocationId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("LocationId");

                    b.ToTable("Patients");
                });

            modelBuilder.Entity("LinguisticStimulationPlanner.Models.PatientGoal", b =>
                {
                    b.Property<int>("PatientId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("GoalId")
                        .HasColumnType("INTEGER");

                    b.HasKey("PatientId", "GoalId");

                    b.HasIndex("GoalId");

                    b.ToTable("PatientGoals");
                });

            modelBuilder.Entity("LinguisticStimulationPlanner.Models.Toy", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("InInventory")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Toys");
                });

            modelBuilder.Entity("LinguisticStimulationPlanner.Models.GoalToy", b =>
                {
                    b.HasOne("LinguisticStimulationPlanner.Models.Goal", "Goal")
                        .WithMany("GoalToys")
                        .HasForeignKey("GoalId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("LinguisticStimulationPlanner.Models.Toy", "Toy")
                        .WithMany("GoalToys")
                        .HasForeignKey("ToyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Goal");

                    b.Navigation("Toy");
                });

            modelBuilder.Entity("LinguisticStimulationPlanner.Models.Patient", b =>
                {
                    b.HasOne("LinguisticStimulationPlanner.Models.Location", "Location")
                        .WithMany("Patients")
                        .HasForeignKey("LocationId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Location");
                });

            modelBuilder.Entity("LinguisticStimulationPlanner.Models.PatientGoal", b =>
                {
                    b.HasOne("LinguisticStimulationPlanner.Models.Goal", "Goal")
                        .WithMany("PatientGoals")
                        .HasForeignKey("GoalId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("LinguisticStimulationPlanner.Models.Patient", "Patient")
                        .WithMany("PatientGoals")
                        .HasForeignKey("PatientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Goal");

                    b.Navigation("Patient");
                });

            modelBuilder.Entity("LinguisticStimulationPlanner.Models.Goal", b =>
                {
                    b.Navigation("GoalToys");

                    b.Navigation("PatientGoals");
                });

            modelBuilder.Entity("LinguisticStimulationPlanner.Models.Location", b =>
                {
                    b.Navigation("Patients");
                });

            modelBuilder.Entity("LinguisticStimulationPlanner.Models.Patient", b =>
                {
                    b.Navigation("PatientGoals");
                });

            modelBuilder.Entity("LinguisticStimulationPlanner.Models.Toy", b =>
                {
                    b.Navigation("GoalToys");
                });
#pragma warning restore 612, 618
        }
    }
}
