using LinguisticStimulationPlanner.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace LinguisticStimulationPlanner.Components.Dialog
{
    public partial class GoalSelectDialog
    {
        private List<GoalWithCheck> _allGoalWithChecks = new List<GoalWithCheck>();

        [Parameter] public string Message { get; set; }
        [Parameter] public string ConfirmButton { get; set; } = "Confirm";
        [Parameter] public string CancelButton { get; set; } = "Cancel";
        [Parameter] public List<Goal> AssignedGoals { get; set; }
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var allGoals = await GoalService.GetGoalsAsync();

            _allGoalWithChecks = allGoals.Select(goal => new GoalWithCheck
            {
                Goal = goal,
                IsChecked = false
            }).ToList();

            if (AssignedGoals != null)
            {
                foreach (var assignedGoal in AssignedGoals)
                {
                    var goalWithCheck = _allGoalWithChecks.FirstOrDefault(gwc => gwc.Goal.Id == assignedGoal.Id);
                    if (goalWithCheck != null)
                    {
                        goalWithCheck.IsChecked = true;
                    }
                }
            }
        }

        private void Cancel()
        {
            MudDialog.Cancel();
        }

        private void Confirm()
        {
            var selectedGoals = _allGoalWithChecks.Where(g => g.IsChecked).Select(g => g.Goal).ToList();

            MudDialog.Close(DialogResult.Ok(selectedGoals));
        }
    }

    public class GoalWithCheck
    {
        public Goal Goal { get; set; }
        public bool IsChecked { get; set; }
    }
}
