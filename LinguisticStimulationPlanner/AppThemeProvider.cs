using MudBlazor;

namespace LinguisticStimulationPlanner
{

    public static class AppThemeProvider
    {
        public static MudTheme GetTheme()
        {
            var theme = new MudTheme();

			theme.PaletteLight.Primary = "#00EC91";
			theme.PaletteLight.Secondary = "#00b747";
            theme.PaletteLight.AppbarBackground = theme.PaletteDark.Background;

			theme.PaletteDark.Primary = "#00EC91";
			theme.PaletteDark.Secondary = "#00b747";

			return theme;
        }
    }
}
