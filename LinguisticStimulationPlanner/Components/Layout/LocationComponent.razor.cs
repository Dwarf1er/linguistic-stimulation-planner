using LinguisticStimulationPlanner.Models;
using LinguisticStimulationPlanner.Services;
using LinguisticStimulationPlanner.Components.Dialog;
using MudBlazor;
using Microsoft.AspNetCore.Components;

namespace LinguisticStimulationPlanner.Components.Layout
{
    public partial class LocationComponent : ComponentBase
    {
        [Inject] public LocationService LocationService { get; set; }
        [Inject] public IDialogService DialogService { get; set; }

        private List<Location> _locations = new List<Location>();
        private HashSet<Location> _selectedLocations = new HashSet<Location>();
        private Location _newLocation = new Location();
        private List<Location> _originalLocations = new List<Location>();
        private bool _isEditMode = false;

        protected override async Task OnInitializedAsync()
        {
            _locations = await LocationService.GetLocationsAsync();
            _originalLocations = _locations.Select(location => new Location { Id = location.Id, Name = location.Name, Address = location.Address }).ToList();
        }

        private async Task SaveLocationAsync(Location location)
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
            _selectedLocations.Clear();
        }

        private async Task SaveAllLocationsAsync()
        {
            _isEditMode = false;

            List<Location> validLocations = _locations.Where(location => location.IsValidLocation()).ToList();
            foreach (Location validLocation in validLocations)
            {
                await SaveLocationAsync(validLocation);
            }

            _selectedLocations.Clear();
        }

        private async Task DeleteSelectedLocationsAsync()
        {
            _isEditMode = false;

            List<Location> locationsToDelete = _selectedLocations.ToList();
            foreach (Location location in locationsToDelete)
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
            _selectedLocations.Clear();
        }

        private void ToggleEditMode()
        {
            if (_isEditMode)
            {
                SaveAllLocationsAsync();
            }
            else
            {
                _isEditMode = true;
            }

            _selectedLocations.Clear();
        }

        private void DiscardChanges()
        {
            _locations = _originalLocations.Select(location => new Location { Id = location.Id, Name = location.Name, Address = location.Address }).ToList();
            _isEditMode = false;
            _selectedLocations.Clear();
        }

        private async Task ShowDeleteConfirmationDialogAsync()
        {
            DialogParameters parameters = new DialogParameters
            {
                { "Message", "Are you sure you want to delete the selected locations?" },
                { "ConfirmButton", "Delete" },
                { "CancelButton", "Cancel" }
            };

            IDialogReference dialog = DialogService.Show<ConfirmationDialog>("Delete Locations", parameters);
            DialogResult result = await dialog.Result;

            if (!result.Canceled)
            {
                await DeleteSelectedLocationsAsync();
            }
        }

    }
}
