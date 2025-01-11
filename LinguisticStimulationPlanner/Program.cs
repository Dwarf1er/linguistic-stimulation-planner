using LinguisticStimulationPlanner.Data;
using LinguisticStimulationPlanner.Services;
using LinguisticStimulationPlanner.Utilities;
using LinguisticStimulationPlanner.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using Photino.Blazor;
using Microsoft.Extensions.FileProviders;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace LinguisticStimulationPlanner
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            PhotinoBlazorAppBuilder appBuilder = PhotinoBlazorAppBuilder.CreateDefault(args);

            ConfigureServices(appBuilder.Services);
            DatabaseSetup.SetupDatabase();

            appBuilder.RootComponents.Add<App>("app");

            PhotinoBlazorApp app = appBuilder.Build();

            app.MainWindow
                .SetSize(1400, 800)
                .SetLogVerbosity(0)
                .SetDevToolsEnabled(EnvironmentInfo.IsDevelopment)
                .SetIconFile("wwwroot/favicon.ico")
                .SetTitle("Linguistic Stimulation Planner");

            app.Services.GetRequiredService<TempFileService>().CleanTemporaryFiles();

            AppDomain.CurrentDomain.UnhandledException += (sender, error) =>
            {
                app.MainWindow.ShowMessage("Fatal exception", error.ExceptionObject.ToString());
            };

            app.Run();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            string databasePath = DatabaseSetup.GetDatabasePath();

            services.AddLogging();
            services.AddSingleton<IFileProvider>(_ => new ManifestEmbeddedFileProvider(typeof(Program).Assembly, "wwwroot"));
            services.AddSingleton<TempFileService>();
            services.AddMudServices();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite($"Data Source={databasePath}"));

            services.AddScoped<GoalService>();
            services.AddScoped<LocationService>();
            services.AddScoped<PatientService>();
            services.AddScoped<ToyService>();
        }
    }
}
