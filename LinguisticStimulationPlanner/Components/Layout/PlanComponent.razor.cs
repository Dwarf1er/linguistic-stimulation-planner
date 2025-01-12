using LinguisticStimulationPlanner.Models;
using LinguisticStimulationPlanner.Services;
using MudBlazor;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinguisticStimulationPlanner.Components.Dialog;

namespace LinguisticStimulationPlanner.Components.Layout
{
    public partial class PlanComponent :ComponentBase
    {
        [Inject] PlanService PlanService { get; set; }
        [Inject] PatientService PatientService { get; set; }
        [Inject] IDialogService DialogService { get; set; }

        private List<Plan> _plans = new List<Plan>();
        private List<Patient> _patients = new List<Patient>();
        private HashSet<Plan> _selectedPlans = new HashSet<Plan>();
        private bool _isEditMode = false;
        private Plan _newPlan = new Plan();

        protected override async Task OnInitializedAsync()
        {
            _plans = await PlanService.GetPlansAsync();
            _patients = await PatientService.GetPatientsAsync();
        }

        private async Task AddNewPlan()
        {
            _plans.Insert(0, _newPlan);
            _newPlan = new Plan();
            _isEditMode = true;
        }

        private void ToggleEditMode()
        {
            if (_isEditMode)
            {
                SaveAllPlansAsync();
            }
            else
            {
                _isEditMode = true;
            }

            _selectedPlans.Clear();
        }

        private async Task SaveAllPlansAsync()
        {
            _isEditMode = false;
            foreach (var plan in _plans.Where(p => p.Id == 0 || PlanService.IsPlanModified(p)))
            {
                if (plan.Id == 0)
                {
                    await PlanService.CreatePlanAsync(plan);
                }
                else
                {
                    await PlanService.UpdatePlanAsync(plan);
                }
            }
            _plans = await PlanService.GetPlansAsync();
        }

        private void DiscardChanges()
        {
            _plans = _plans.Select(plan => PlanService.ClonePlan(plan)).ToList();
            _isEditMode = false;
        }

        private async Task DeleteSelectedPlansAsync()
        {
            _isEditMode = false;

            List<Plan> plansToDelete = _selectedPlans.ToList();

            foreach (var plan in plansToDelete)
            {
                await PlanService.DeletePlanAsync(plan.Id);
            }

            _plans = await PlanService.GetPlansAsync();
            _selectedPlans.Clear();
        }

        private async Task ShowDeleteConfirmationDialogAsync()
        {
            DialogParameters parameters = new DialogParameters
            {
                { "Message", "Are you sure you want to delete the selected patients?" },
                { "ConfirmButton", "Delete" },
                { "CancelButton", "Cancel" }
            };

            IDialogReference dialog = DialogService.Show<ConfirmationDialog>("Delete Patients", parameters);
            DialogResult result = await dialog.Result;

            if (!result.Canceled)
            {
                await DeleteSelectedPlansAsync();
            }
        }

        private async Task ShowGoalToyDialogAsync(Plan plan)
        {
            var parameters = new DialogParameters { { "Plan", plan } };
            var dialog = DialogService.Show<GoalToySelectDialog>("Select Goals and Toys", parameters);
            var result = await dialog.Result;

            if (!result.Canceled && result.Data is Dictionary<Goal, Toy> selectedGoalsWithToys)
            {
                plan.PlanGoals.Clear();
                foreach (var kvp in selectedGoalsWithToys)
                {
                    plan.PlanGoals.Add(new PlanGoal
                    {
                        GoalId = kvp.Key.Id,
                        Goal = kvp.Key,
                        ToyId = kvp.Value?.Id,
                        Toy = kvp.Value
                    });
                }
            }
        }
    }
}
