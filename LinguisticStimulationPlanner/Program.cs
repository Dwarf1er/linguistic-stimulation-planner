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
using System.Reflection;
using System.IO;

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

            string iconPath = ExtractEmbeddedResourceToTempFile("favicon.ico");

            app.MainWindow
                .SetSize(1400, 800)
                .SetLogVerbosity(0)
                .SetDevToolsEnabled(EnvironmentInfo.IsDevelopment)
                .SetIconFile(iconPath)
                .SetTitle("Linguistic Stimulation Planner");

            AppDomain.CurrentDomain.ProcessExit += (sender, eventArgs) =>
            {
                using var serviceProvider = app.Services.CreateScope();
                var tempFileService = serviceProvider.ServiceProvider.GetRequiredService<TempFileService>();
                tempFileService.CleanTemporaryFiles();
            };

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
            services.AddScoped<PlanService>();
            services.AddScoped<ToyService>();
        }

        private static string ExtractEmbeddedResourceToTempFile(string fileName)
        {
            string resourceNamespace = typeof(Program).Namespace;
            string resourceName = $"{resourceNamespace}.wwwroot.{fileName}";

            Assembly assembly = Assembly.GetExecutingAssembly();

            using (Stream resourceStream = assembly.GetManifestResourceStream(resourceName))
            {
                if (resourceStream == null)
                {
                    Console.WriteLine($"Resource {resourceName} not found.");
                    return null;
                }

                string tempFile = Path.Combine(Path.GetTempPath(), fileName);

                using (FileStream fileStream = new FileStream(tempFile, FileMode.Create, FileAccess.Write))
                {
                    resourceStream.CopyTo(fileStream);
                }

                return tempFile;
            }
        }
    }
}
