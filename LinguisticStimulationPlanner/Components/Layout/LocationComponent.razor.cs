using LinguisticStimulationPlanner.Models;
using LinguisticStimulationPlanner.Services;
using LinguisticStimulationPlanner.Components.Dialog;
using MudBlazor;

namespace LinguisticStimulationPlanner.Components.Layout
{
    public partial class LocationComponent
    {
        private List<Location> _locations = new List<Location>();
        private HashSet<Location> _selectedLocations = new HashSet<Location>();
        private Location _newLocation = new Location();
        private bool _isEditMode = false;
        private List<Location> _originalLocations = new List<Location>();

        protected override async Task OnInitializedAsync()
        {
            _locations = await LocationService.GetLocationsAsync();
            _originalLocations = _locations.Select(location => new Location { Id = location.Id, Name = location.Name, Address = location.Address }).ToList();
        }

        private async Task SaveLocation(Location location)
        {
            if (location.Id == 0)
            {
                await LocationService.CreateLocationAsync(location);
            }
            else
            {
                await LocationService.UpdateLocationAsync(location);
            }

            _locations = await LocationService.GetLocationsAsync();
            _originalLocations = _locations.Select(location => new Location { Id = location.Id, Name = location.Name, Address = location.Address }).ToList();
        }

        private async Task SaveAllLocations()
        {
            _isEditMode = false;

            foreach (var location in _locations)
            {
                await SaveLocation(location);
            }
        }

        private async Task DeleteSelectedLocations()
        {
            _isEditMode = false;

            foreach (var location in _selectedLocations)
            {
                await LocationService.DeleteLocationAsync(location.Id);
            }

            _locations = await LocationService.GetLocationsAsync();
            _selectedLocations.Clear();
            _originalLocations = _locations.Select(location => new Location { Id = location.Id, Name = location.Name, Address = location.Address }).ToList();
        }

        private void AddNewLocation()
        {
            _locations.Insert(0, _newLocation);
            _isEditMode = true;
            _newLocation = new Location();
        }

        private void ToggleEditMode()
        {
            if (_isEditMode)
            {
                SaveAllLocations();
            }
            else
            {
                _isEditMode = true;
            }
        }

        private void DiscardChanges()
        {
            _locations = _originalLocations.Select(location => new Location { Id = location.Id, Name = location.Name, Address = location.Address }).ToList();
            _isEditMode = false;
        }

        private async Task ShowDeleteConfirmationDialog()
        {
            var parameters = new DialogParameters
            {
                { "Message", "Are you sure you want to delete the selected locations?" },
                { "ConfirmButton", "Delete" },
                { "CancelButton", "Cancel" }
            };

            var dialog = DialogService.Show<ConfirmationDialog>("Delete Locations", parameters);
            var result = await dialog.Result;

            if (!result.Canceled)
            {
                await DeleteSelectedLocations();
            }
        }

    }
}
