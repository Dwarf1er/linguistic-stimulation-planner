using LinguisticStimulationPlanner.Components.Dialog;
using LinguisticStimulationPlanner.Models;
using LinguisticStimulationPlanner.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace LinguisticStimulationPlanner.Components.Layout
{
    public partial class GoalComponent : ComponentBase
    {
        [Inject] public GoalService GoalService { get; set; }
        [Inject] public IDialogService DialogService { get; set; }

        private List<Goal> _goals = new List<Goal>();
        private HashSet<Goal> _selectedGoals = new HashSet<Goal>();
        private Goal _newGoal = new Goal();
        private bool _isEditMode = false;
        private List<Goal> _originalGoals = new List<Goal>();

        protected override async Task OnInitializedAsync()
        {
            _goals = await GoalService.GetGoalsAsync();
            _originalGoals = _goals.Select(goal => new Goal { Id = goal.Id, Name = goal.Name, Description = goal.Description }).ToList();
        }

        private async Task SaveGoalAsync(Goal goal)
        {
            if (goal.Id == 0)
            {
                await GoalService.CreateGoalAsync(goal);
            }
            else
            {
                await GoalService.UpdateGoalAsync(goal);
            }

            _goals = await GoalService.GetGoalsAsync();
            _originalGoals = _goals.Select(goal => new Goal { Id = goal.Id, Name = goal.Name, Description = goal.Description }).ToList();
            _selectedGoals.Clear();
        }

        private async Task SaveAllGoalsAsync()
        {
            _isEditMode = false;

            List<Goal> validGoals = _goals.Where(goal => goal.IsValidGoal()).ToList();
            foreach (Goal validGoal in validGoals)
            {
                await SaveGoalAsync(validGoal);
            }

            _selectedGoals.Clear();
        }

        private async Task DeleteSelectedGoalsAsync()
        {
            _isEditMode = false;

            List<Goal> goalsToDelete = _selectedGoals.ToList();
            foreach (Goal goal in goalsToDelete)
            {
                await GoalService.DeleteGoalAsync(goal.Id);
            }

            _goals = await GoalService.GetGoalsAsync();
            _selectedGoals.Clear();
            _originalGoals = _goals.Select(goal => new Goal { Id = goal.Id, Name = goal.Name, Description = goal.Description }).ToList();
        }

        private void AddNewGoal()
        {
            _goals.Insert(0, _newGoal);
            _isEditMode = true;
            _newGoal = new Goal();
            _selectedGoals.Clear();
        }

        private void ToggleEditMode()
        {
            if (_isEditMode)
            {
                SaveAllGoalsAsync();
            }
            else
            {
                _isEditMode = true;
            }

            _selectedGoals.Clear();
        }

        private void DiscardChanges()
        {
            _goals = _originalGoals.Select(goal => new Goal { Id = goal.Id, Name = goal.Name, Description = goal.Description }).ToList();
            _isEditMode = false;
            _selectedGoals.Clear();
        }

        private async Task ShowDeleteConfirmationDialogAsync()
        {
            DialogParameters parameters = new DialogParameters
            {
                { "Message", "Are you sure you want to delete the selected goals?" },
                { "ConfirmButton", "Delete" },
                { "CancelButton", "Cancel" }
            };

            IDialogReference dialog = DialogService.Show<ConfirmationDialog>("Delete Goals", parameters);
            DialogResult result = await dialog.Result;

            if (!result.Canceled)
            {
                await DeleteSelectedGoalsAsync();
            }
        }
    }
}
