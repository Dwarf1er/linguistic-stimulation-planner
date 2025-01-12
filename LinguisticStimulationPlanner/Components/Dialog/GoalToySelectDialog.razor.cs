using LinguisticStimulationPlanner.Models;
using MudBlazor;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinguisticStimulationPlanner.Components.Dialog
{
    public partial class GoalToySelectDialog : ComponentBase
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }

        [Parameter] public Plan Plan { get; set; }

        private Dictionary<Goal, bool> _selectedGoals = new();
        private Dictionary<Goal, Toy> _selectedGoalToys = new();

        protected override void OnInitialized()
        {
            if (Plan?.Patient?.PatientGoals != null)
            {
                foreach (var goal in Plan.Patient.PatientGoals.Select(pg => pg.Goal))
                {
                    _selectedGoals[goal] = Plan.PlanGoals.Any(pg => pg.GoalId == goal.Id);
                    var toy = Plan.PlanGoals.FirstOrDefault(pg => pg.GoalId == goal.Id)?.Toy;
                    _selectedGoalToys[goal] = toy ?? null;
                }
            }
        }

        private void Save()
        {
            var selectedGoalsWithToys = _selectedGoals
                .Where(kvp => kvp.Value)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => _selectedGoalToys.ContainsKey(kvp.Key) ? _selectedGoalToys[kvp.Key] : null
                );

            MudDialog.Close(DialogResult.Ok(selectedGoalsWithToys));
        }

        private void Cancel()
        {
            MudDialog.Cancel();
        }
    }
}
