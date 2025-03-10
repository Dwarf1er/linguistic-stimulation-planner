﻿using LinguisticStimulationPlanner.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace LinguisticStimulationPlanner.Utilities
{
    public static class DatabaseSetup
    {
        public static string DevelopmentDatabasePath = "./Data/Database.db";

        public static string GetDatabasePath()
        {
            if (EnvironmentInfo.IsDevelopment)
            {
                return DevelopmentDatabasePath;
            }

            string test = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LinguisticStimulationPlanner", "Database.db");
            return test;
        }

        public static void SetupDatabase()
        {
            string databasePath = GetDatabasePath();

            if (EnvironmentInfo.IsDevelopment)
            {
                return;
            }

            string directoryPath = Path.GetDirectoryName(databasePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            ApplyMigrations(databasePath);
        }

        private static DbContextOptions<ApplicationDbContext> GetDbContextOptions(string databasePath)
        {
            return new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite($"Data Source={databasePath}")
                .Options;
        }

        public static void ApplyMigrations(string databasePath)
        {
            var options = GetDbContextOptions(databasePath);
            using var context = new ApplicationDbContext(options);

            context.Database.Migrate();
        }
    }
}
