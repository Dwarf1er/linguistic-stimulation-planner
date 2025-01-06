using LinguisticStimulationPlanner.Components.Dialog;
using LinguisticStimulationPlanner.Models;
using MudBlazor;

namespace LinguisticStimulationPlanner.Components.Layout
{
    public partial class GoalComponent
    {
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

        private async Task SaveGoal(Goal goal)
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
        }

        private async Task SaveAllGoals()
        {
            _isEditMode = false;

            foreach (var goal in _goals)
            {
                await SaveGoal(goal);
            }
        }

        private async Task DeleteSelectedGoals()
        {
            _isEditMode = false;

            foreach (var goal in _selectedGoals)
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
        }

        private void ToggleEditMode()
        {
            if (_isEditMode)
            {
                SaveAllGoals();
            }
            else
            {
                _isEditMode = true;
            }
        }

        private void DiscardChanges()
        {
            _goals = _originalGoals.Select(goal => new Goal { Id = goal.Id, Name = goal.Name, Description = goal.Description }).ToList();
            _isEditMode = false;
        }

        private async Task ShowDeleteConfirmationDialog()
        {
            var parameters = new DialogParameters
        {
            { "Message", "Are you sure you want to delete the selected goals?" },
            { "ConfirmButton", "Delete" },
            { "CancelButton", "Cancel" }
        };

            var dialog = DialogService.Show<ConfirmationDialog>("Delete Goals", parameters);
            var result = await dialog.Result;

            if (!result.Canceled)
            {
                await DeleteSelectedGoals();
            }
        }
    }
}
