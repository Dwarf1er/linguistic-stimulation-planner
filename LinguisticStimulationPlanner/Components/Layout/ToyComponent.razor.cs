using LinguisticStimulationPlanner.Components.Dialog;
using LinguisticStimulationPlanner.Models;
using LinguisticStimulationPlanner.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinguisticStimulationPlanner.Components.Layout
{
    public partial class ToyComponent : ComponentBase
    {
        [Inject] public ToyService ToyService { get; set; }
        [Inject] public IDialogService DialogService { get; set; }

        private List<Toy> _toys = new List<Toy>();
        private HashSet<Toy> _selectedToys = new HashSet<Toy>();
        private Toy _newToy = new Toy();
        private List<Goal> _newToyGoals = new List<Goal>();
        private bool _isEditMode = false;
        private List<Toy> _originalToys = new List<Toy>();
        private bool _showInInventoryOnly = true;

        protected override async Task OnInitializedAsync()
        {
            _toys = await ToyService.GetToysAsync();
            _originalToys = _toys.Select(toy => new Toy { Id = toy.Id, Name = toy.Name, InInventory = toy.InInventory }).ToList();
        }

        private async Task SaveToyAsync(Toy toy)
        {
            if (toy.Id == 0)
            {
                await ToyService.CreateToyAsync(toy);

                if(_newToyGoals.Any())
                {
                    await ToyService.AssignGoalsToToyAsync(toy, _newToyGoals);
                }
            }

            else
            {
                await ToyService.UpdateToyAsync(toy);
            }

            _toys = await ToyService.GetToysAsync();
            _originalToys = _toys.Select(toy => new Toy { Id = toy.Id, Name = toy.Name, InInventory = toy.InInventory }).ToList();
            _selectedToys.Clear();
        }

        private async Task SaveAllToysAsync()
        {
            _isEditMode = false;

            List<Toy> validToys = _toys.Where(toy => toy.IsValidToy()).ToList();
            foreach (Toy validToy in validToys)
            {
                await SaveToyAsync(validToy);
            }
        }

        private async Task DeleteSelectedToysAsync()
        {
            _isEditMode = false;

            List<Toy> toysToDelete = _selectedToys.ToList();
            foreach (Toy toy in toysToDelete)
            {
                await ToyService.DeleteToyAsync(toy.Id);
            }

            _toys = await ToyService.GetToysAsync();
            _selectedToys.Clear();
            _originalToys = _toys.Select(toy => new Toy { Id = toy.Id, Name = toy.Name, InInventory = toy.InInventory }).ToList();
        }

        private async void AddNewToy()
        {
            _toys.Insert(0, _newToy);
            _isEditMode = true;
            _newToyGoals = new List<Goal>();
            _newToy = new Toy();
            _selectedToys.Clear();
        }

        private void ToggleEditMode()
        {
            if (_isEditMode)
            {
                SaveAllToysAsync();
            }
            else
            {
                _isEditMode = true;
            }

            _selectedToys.Clear();
        }

        private void DiscardChanges()
        {
            _toys = _originalToys.Select(toy => new Toy { Id = toy.Id, Name = toy.Name, InInventory = toy.InInventory }).ToList();
            _isEditMode = false;
            _selectedToys.Clear();
        }

        private async Task ShowDeleteConfirmationDialogAsync()
        {
            DialogParameters parameters = new DialogParameters
            {
                { "Message", "Are you sure you want to delete the selected toys?" },
                { "ConfirmButton", "Delete" },
                { "CancelButton", "Cancel" }
            };

            IDialogReference dialog = DialogService.Show<ConfirmationDialog>("Delete Toys", parameters);
            DialogResult result = await dialog.Result;

            if (!result.Canceled)
            {
                await DeleteSelectedToysAsync();
            }
        }

        private async Task ShowGoalSelectDialogAsync(Toy currentToy)
        {
            if (currentToy.Id == 0)
            {
                currentToy = _newToy;
            }

            List<Goal> assignedGoals = currentToy.GoalToys.Select(gt => gt.Goal).ToList();

            DialogParameters parameters = new DialogParameters
            {
                { "Message", "Select goals to assign to the toy" },
                { "AssignedGoals", assignedGoals }
            };

            IDialogReference dialog = DialogService.Show<GoalSelectDialog>("Manage Goals", parameters);
            DialogResult result = await dialog.Result;

            if (!result.Canceled)
            {
                List<Goal> selectedGoals = result.Data as List<Goal>;

                if (selectedGoals != null)
                {
                    if ( currentToy.Id == 0)
                    {
                        _newToyGoals = selectedGoals;
                    }

                    else
                    {
                        List<Goal> goalsToRemove = assignedGoals.Where(g => !selectedGoals.Contains(g)).ToList();
                        List<Goal> goalsToAdd = selectedGoals.Where(g => !assignedGoals.Contains(g)).ToList();

                        if (goalsToRemove.Any())
                        {
                            await ToyService.DeassignGoalsFromToy(currentToy, goalsToRemove);
                        }

                        if (goalsToAdd.Any())
                        {
                            await ToyService.AssignGoalsToToyAsync(currentToy, goalsToAdd);
                        }
                    }
                }
            }
        }
    }
}
