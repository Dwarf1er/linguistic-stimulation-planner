using LinguisticStimulationPlanner.Models;
using LinguisticStimulationPlanner.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinguisticStimulationPlanner.Components.Dialog
{
    public partial class PlanGoalSelectDialog : ComponentBase
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Inject] ToyService ToyService { get; set; }
        [Parameter] public string Message { get; set; }
        [Parameter] public List<PlanGoal> AssignedGoals { get; set; }
        [Parameter] public Patient Patient { get; set; }

        private List<PlanGoalDialogItem> _planGoals;

        protected override async Task OnInitializedAsync()
        {
            if (Patient == null)
            {
                Console.Error.WriteLine("Patient is null, unable to fetch goals.");
                return;
            }

            var patientGoals = Patient.PatientGoals.Select(pg => pg.Goal).ToList();

            _planGoals = new List<PlanGoalDialogItem>();

            foreach (var goal in patientGoals)
            {
                var relatedToys = await ToyService.GetToysByGoalAsync(goal.Id);

                _planGoals.Add(new PlanGoalDialogItem
                {
                    Goal = goal,
                    IsSelected = AssignedGoals.Any(pg => pg.GoalId == goal.Id),
                    AvailableToys = relatedToys,
                    SelectedToyId = AssignedGoals.FirstOrDefault(pg => pg.GoalId == goal.Id)?.ToyId
                });
            }
        }

        private void Cancel()
        {
            MudDialog.Cancel();
        }

        private void Save()
        {
            var selectedPlanGoals = _planGoals
                .Where(pg => pg.IsSelected)
                .Select(pg => new PlanGoal
                {
                    GoalId = pg.Goal.Id,
                    ToyId = pg.SelectedToyId
                }).ToList();

            MudDialog.Close(DialogResult.Ok(selectedPlanGoals));
        }

        private class PlanGoalDialogItem
        {
            public Goal Goal { get; set; }
            public bool IsSelected { get; set; }
            public int? SelectedToyId { get; set; }
            public List<Toy> AvailableToys { get; set; }
        }
    }
}
