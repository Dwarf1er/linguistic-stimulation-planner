using LinguisticStimulationPlanner.Components.Dialog;
using LinguisticStimulationPlanner.Models;
using LinguisticStimulationPlanner.Services;
using MudBlazor;

namespace LinguisticStimulationPlanner.Components.Layout
{
    public partial class ToyComponent
    {
        private List<Toy> _toys = new List<Toy>();
        private HashSet<Toy> _selectedToys = new HashSet<Toy>();
        private Toy _newToy = new Toy();
        private bool _isEditMode = false;
        private List<Toy> _originalToys = new List<Toy>();
        private bool _showInInventoryOnly = false;

        protected override async Task OnInitializedAsync()
        {
            _toys = await ToyService.GetToysAsync();
            _originalToys = _toys.Select(toy => new Toy { Id = toy.Id, Name = toy.Name, InInventory = toy.InInventory }).ToList();
        }

        private async Task SaveToy(Toy toy)
        {
            if (toy.Id == 0)
            {
                await ToyService.CreateToyAsync(toy);
            }
            else
            {
                await ToyService.UpdateToyAsync(toy);
            }

            _toys = await ToyService.GetToysAsync();
            _originalToys = _toys.Select(toy => new Toy { Id = toy.Id, Name = toy.Name, InInventory = toy.InInventory }).ToList();
        }

        private async Task SaveAllToys()
        {
            _isEditMode = false;

            foreach (var toy in _toys)
            {
                await SaveToy(toy);
            }
        }

        private async Task DeleteSelectedToys()
        {
            _isEditMode = false;

            foreach (var toy in _selectedToys)
            {
                await ToyService.DeleteToyAsync(toy.Id);
            }

            _toys = await ToyService.GetToysAsync();
            _selectedToys.Clear();
            _originalToys = _toys.Select(toy => new Toy { Id = toy.Id, Name = toy.Name, InInventory = toy.InInventory }).ToList();
        }

        private void AddNewToy()
        {
            _toys.Insert(0, _newToy);
            _isEditMode = true;
            _newToy = new Toy();
        }

        private void ToggleEditMode()
        {
            if (_isEditMode)
            {
                SaveAllToys();
            }
            else
            {
                _isEditMode = true;
            }
        }

        private void DiscardChanges()
        {
            _toys = _originalToys.Select(toy => new Toy { Id = toy.Id, Name = toy.Name, InInventory = toy.InInventory }).ToList();
            _isEditMode = false;
        }

        private async Task ShowDeleteConfirmationDialog()
        {
            var parameters = new DialogParameters
            {
                { "Message", "Are you sure you want to delete the selected toys?" },
                { "ConfirmButton", "Delete" },
                { "CancelButton", "Cancel" }
            };

            var dialog = DialogService.Show<ConfirmationDialog>("Delete Toys", parameters);
            var result = await dialog.Result;

            if (!result.Canceled)
            {
                await DeleteSelectedToys();
            }
        }

        private async Task ShowGoalSelectDialog(Toy currentToy)
        {
            var assignedGoals = currentToy.GoalToys.Select(gt => gt.Goal).ToList();

            var parameters = new DialogParameters
            {
                { "Message", "Select goals to assign to the toy" },
                { "AssignedGoals", assignedGoals }
            };

            var dialog = DialogService.Show<GoalSelectDialog>("Select goals to assign to the toy", parameters);
            var result = await dialog.Result;

            if (!result.Canceled)
            {
                var selectedGoals = result.Data as List<Goal>;

                var goalsToRemove = assignedGoals.Where(g => !selectedGoals.Contains(g)).ToList();
                await ToyService.DeassignGoalsFromToy(currentToy, goalsToRemove);
                await ToyService.AssignGoalsToToy(currentToy, selectedGoals);
            }
        }
    }
}
