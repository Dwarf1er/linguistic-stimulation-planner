﻿using MudBlazor;

namespace LinguisticStimulationPlanner.Components.Layout
{
	public partial class MainLayout
	{
		public required MudThemeProvider _mudThemeProvider;
		private MudTheme _mudTheme = AppThemeProvider.GetTheme();
		private bool _drawerOpen = true;
		private bool _isDarkMode;

		protected void DrawerToggle()
		{
			_drawerOpen = !_drawerOpen;
		}

		protected override async Task OnAfterRenderAsync(bool firstRender)
		{
			if(firstRender)
			{
				_isDarkMode = await _mudThemeProvider.GetSystemPreference();
				await _mudThemeProvider.WatchSystemPreference(OnSystemPreferenceChanged);
				StateHasChanged();
			}
		}

		private Task OnSystemPreferenceChanged(bool newValue)
		{
			_isDarkMode = newValue;
			StateHasChanged();
			return Task.CompletedTask;
		}
	}
}
