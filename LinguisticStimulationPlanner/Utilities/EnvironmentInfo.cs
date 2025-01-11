using System;

namespace LinguisticStimulationPlanner.Utilities
{
    public static class EnvironmentInfo
    {
        public static readonly bool IsDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
    }
}
