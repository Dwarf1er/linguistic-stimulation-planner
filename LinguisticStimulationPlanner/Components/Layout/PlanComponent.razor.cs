using LinguisticStimulationPlanner.Models;
using LinguisticStimulationPlanner.Services;
using MudBlazor;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinguisticStimulationPlanner.Components.Dialog;
using System;

namespace LinguisticStimulationPlanner.Components.Layout
{
    public partial class PlanComponent :ComponentBase
    {
        [Inject] PlanService PlanService { get; set; }
        [Inject] PatientService PatientService { get; set; }
        [Inject] IDialogService DialogService { get; set; }

        private List<Plan> _plans = new List<Plan>();
        private HashSet<Plan> _selectedPlans = new HashSet<Plan>();
        private Plan _newPlan = new Plan();
        private List<PlanGoal> _newPlanGoals = new List<PlanGoal>();
        private List<Patient> _patients = new List<Patient>();
        private List<Plan> _originalPlans = new List<Plan>();
        private string _searchText = string.Empty;
        private bool _showActivePlansOnly = true;
        private bool _isEditMode = false;

        private IEnumerable<Plan> FilteredPlans => _plans
            .Where(p => !_showActivePlansOnly || p.EndDate == null || p.EndDate >= DateTime.Today)
            .Where(p => string.IsNullOrWhiteSpace(_searchText) || PlanMatchesSearch(p));

        protected override async Task OnInitializedAsync()
        {
            _plans = await PlanService.GetPlansAsync();
            _patients = await PatientService.GetPatientsAsync();
            _originalPlans = _plans.Select(plan => new Plan { Id = plan.Id, PatientId = plan.PatientId, Patient = plan.Patient, StartDate = plan.StartDate, EndDate = plan.EndDate, Notes = plan.Notes, PlanGoals = plan.PlanGoals}).ToList();
        }

        private async Task SavePlanAsync(Plan plan)
        {
            if (plan.Id == 0)
            {
                await PlanService.CreatePlanAsync(plan);

                if(_newPlanGoals.Any())
                {
                    await PlanService.AssignPlanGoalToPlan(plan, _newPlanGoals);
                }
            }
            else
            {
                await PlanService.UpdatePlanAsync(plan);
            }

            _plans = await PlanService.GetPlansAsync();
            _originalPlans = _plans.Select(plan => new Plan { Id = plan.Id, PatientId = plan.PatientId, Patient = plan.Patient, StartDate = plan.StartDate, EndDate = plan.EndDate, Notes = plan.Notes, PlanGoals = plan.PlanGoals}).ToList();
            _selectedPlans.Clear();
        }

        private async Task SaveAllPlansAsync()
        {
            _isEditMode = false;
            List<Plan> validPlans = _plans.Where(plan => plan.IsValidPlan()).ToList();
            foreach (Plan validPlan in validPlans)
            {
                await SavePlanAsync(validPlan);
            }
        }

        private async Task DeleteSelectedPlansAsync()
        {
            _isEditMode = false;

            List<Plan> plansToDelete = _selectedPlans.ToList();
            foreach (Plan plan in plansToDelete)
            {
                await PlanService.DeletePlanAsync(plan.Id);
            }

            _plans = await PlanService.GetPlansAsync();
            _selectedPlans.Clear();
            _originalPlans = _plans.Select(plan => new Plan { Id = plan.Id, PatientId = plan.PatientId, Patient = plan.Patient, StartDate = plan.StartDate, EndDate = plan.EndDate, Notes = plan.Notes, PlanGoals = plan.PlanGoals}).ToList();
        }

        private void AddNewPlan()
        {
            _plans.Insert(0, _newPlan);
            _isEditMode = true;
            _newPlanGoals = new List<PlanGoal>();
            _newPlan = new Plan();
            _selectedPlans.Clear();
        }

        private async Task ToggleEditModeAsync()
        {
            if (_isEditMode)
            {
                await SaveAllPlansAsync();
            }
            else
            {
                _isEditMode = true;
            }

            _selectedPlans.Clear();
        }

        private void DiscardChanges()
        {
            _plans = _originalPlans.Select(plan => new Plan { Id = plan.Id, PatientId = plan.PatientId, Patient = plan.Patient, StartDate = plan.StartDate, EndDate = plan.EndDate, Notes = plan.Notes, PlanGoals = plan.PlanGoals}).ToList();
            _isEditMode = false;
            _selectedPlans.Clear();
        }

        private bool PlanMatchesSearch(Plan plan)
        {
            string search = _searchText.ToLower();

            return (plan.Patient?.Name?.ToLower().Contains(search) ?? false)
                || (plan.Notes?.ToLower().Contains(search) ?? false)
                || plan.PlanGoals.Any(g => g.Goal.Name?.ToLower().Contains(search) ?? false)
                || (plan.StartDate?.ToString("d").Contains(search) ?? false)
                || (plan.EndDate?.ToString("d").Contains(search) ?? false);
        }

        private DateTime GetStartOfWeek()
        {
            DateTime currentDate = DateTime.Today;
            int daysToSubstract = (int)currentDate.DayOfWeek;
            if (daysToSubstract == 0)
            {
                return currentDate;
            }

            DateTime sunday = currentDate.AddDays(-daysToSubstract);
            return sunday;
        }

        private async Task ShowDeleteConfirmationDialogAsync()
        {
            DialogParameters parameters = new DialogParameters
            {
                { "Message", "Are you sure you want to delete the selected plans?" },
                { "ConfirmButton", "Delete" },
                { "CancelButton", "Cancel" }
            };

            IDialogReference dialog = DialogService.Show<ConfirmationDialog>("Delete Plans", parameters);
            DialogResult result = await dialog.Result;

            if (!result.Canceled)
            {
                await DeleteSelectedPlansAsync();
            }
        }

        private async Task ShowPlanGoalSelectDialogAsync(Plan currentPlan)
        {
            if (currentPlan.Patient == null && currentPlan.PatientId != null)
            {
                currentPlan.Patient = _patients.FirstOrDefault(p => p.Id == currentPlan.PatientId);
            }

            List<PlanGoal> assignedPlanGoals = currentPlan.PlanGoals.ToList();

            DialogParameters parameters = new DialogParameters
            {
                { "Message", "Select goals to assign to the plan" },
                { "Patient", currentPlan.Patient },
                { "AssignedGoals", assignedPlanGoals }
            };

            IDialogReference dialog = DialogService.Show<PlanGoalSelectDialog>("Select Goals and Toys", parameters);
            DialogResult result = await dialog.Result;

            if (!result.Canceled)
            {
                List<PlanGoal> selectedPlanGoals = result.Data as List<PlanGoal>;

                if(selectedPlanGoals != null)
                {
                    if(currentPlan.Id == 0)
                    {
                        _newPlanGoals = selectedPlanGoals;
                    }

                    else
                    {
                        List<PlanGoal> planGoalsToRemove = assignedPlanGoals.Where(pg => !selectedPlanGoals.Contains(pg)).ToList();
                        List<PlanGoal> planGoalsToAdd = selectedPlanGoals.Where(pg => !assignedPlanGoals.Contains(pg)).ToList();

                        if(planGoalsToRemove.Any())
                        {
                            await PlanService.DeassignPlanGoalFromPlan(currentPlan, planGoalsToRemove);
                        }

                        if(planGoalsToAdd.Any())
                        {
                            await PlanService.AssignPlanGoalToPlan(currentPlan, planGoalsToAdd);
                        }
                    }
                }
            }
        }
    }
}
