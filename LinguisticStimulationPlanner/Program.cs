using LinguisticStimulationPlanner.Components;
using LinguisticStimulationPlanner.Data;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using Photino.Blazor;

namespace LinguisticStimulationPlanner;

public class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var appBuilder = PhotinoBlazorAppBuilder.CreateDefault(args);

        appBuilder.Services.AddLogging();
        appBuilder.Services.AddDbContext<ApplicationDbContext>
        (
            options => options.UseSqlite(configuration.GetConnectionString("DefaultConnection"))
        );
        appBuilder.Services.AddMudServices();

        appBuilder.RootComponents.Add<App>("app");

        var app = appBuilder.Build();

        app.MainWindow
            .SetSize(1400, 800)
            .SetDevToolsEnabled(true)
            .SetLogVerbosity(0)
            .SetIconFile("favicon.ico")
            .SetTitle("Linguistic Stimulation Planner");

        AppDomain.CurrentDomain.UnhandledException += (sender, error) =>
        {
            app.MainWindow.ShowMessage("Fatal exception", error.ExceptionObject.ToString());
        };

        app.Run();
    }
}
