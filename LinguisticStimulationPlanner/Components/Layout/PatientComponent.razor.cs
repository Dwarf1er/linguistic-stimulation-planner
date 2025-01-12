using LinguisticStimulationPlanner.Models;
using LinguisticStimulationPlanner.Services;
using LinguisticStimulationPlanner.Components.Dialog;
using MudBlazor;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace LinguisticStimulationPlanner.Components.Layout
{
    public partial class PatientComponent : ComponentBase
    {
        [Inject] PatientService PatientService { get; set; }
        [Inject] LocationService LocationService { get; set; }
        [Inject] IDialogService DialogService { get; set; }

        private List<Location> _locations = new List<Location>();
        private List<Patient> _patients = new List<Patient>();
        private HashSet<Patient> _selectedPatients = new HashSet<Patient>();
        private Patient _newPatient = new Patient();
        private List<Goal> _newPatientGoals = new List<Goal>();
        private bool _isEditMode = false;
        private List<Patient> _originalPatients = new List<Patient>();

        protected override async Task OnInitializedAsync()
        {
            _patients = await PatientService.GetPatientsAsync();
            _locations = await LocationService.GetLocationsAsync();
            _originalPatients = _patients.Select(patient => new Patient { Id = patient.Id, Name = patient.Name, Language = patient.Language, Email = patient.Email, LocationId = patient.LocationId, Location = patient.Location }).ToList();
        }

        private async Task SavePatientAsync(Patient patient)
        {
            if (patient.Id == 0)
            {
                await PatientService.CreatePatientAsync(patient);

                if(_newPatientGoals.Any())
                {
                    await PatientService.AssignGoalsToPatient(patient, _newPatientGoals);
                }
            }
            else
            {
                await PatientService.UpdatePatientAsync(patient);
            }

            _patients = await PatientService.GetPatientsAsync();
            _originalPatients = _patients.Select(patient => new Patient { Id = patient.Id, Name = patient.Name, Language = patient.Language, Email = patient.Email, LocationId = patient.LocationId, Location = patient.Location }).ToList();
            _selectedPatients.Clear();
        }

        private async Task SaveAllPatientsAsync()
        {
            _isEditMode = false;

            List<Patient> validPatients = _patients.Where(patient => patient.IsValidPatient()).ToList();
            foreach (Patient validPatient in validPatients)
            {
                await SavePatientAsync(validPatient);
            }
        }

        private async Task DeleteSelectedPatientsAsync()
        {
            _isEditMode = false;

            List<Patient> patientsToDelete = _selectedPatients.ToList();
            foreach (Patient patientToDelete in patientsToDelete)
            {
                await PatientService.DeletePatientAsync(patientToDelete.Id);
            }

            _patients = await PatientService.GetPatientsAsync();
            _selectedPatients.Clear();
            _originalPatients = _patients.Select(patient => new Patient { Id = patient.Id, Name = patient.Name, Language = patient.Language, Email = patient.Email, LocationId = patient.LocationId, Location = patient.Location }).ToList();
        }

        private async Task AddNewPatient()
        {
            _patients.Insert(0, _newPatient);
            _isEditMode = true;
            _newPatientGoals = new List<Goal>();
            _newPatient = new Patient();
            _selectedPatients.Clear();
        }

        private async Task ToggleEditMode()
        {
            if (_isEditMode)
            {
                await SaveAllPatientsAsync();
            }
            else
            {
                _isEditMode = true;
            }

            _selectedPatients.Clear();
        }

        private void DiscardChanges()
        {
            _patients = _originalPatients.Select(patient => new Patient { Id = patient.Id, Name = patient.Name, Language = patient.Language, Email = patient.Email, LocationId = patient.LocationId, Location = patient.Location }).ToList();
            _isEditMode = false;
            _selectedPatients.Clear();
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
                await DeleteSelectedPatientsAsync();
            }
        }

        private async Task ShowGoalSelectDialogAsync(Patient currentPatient)
        {
            if (currentPatient.Id == 0)
            {
                currentPatient = _newPatient;
            }

            List<Goal> assignedGoals = currentPatient.PatientGoals.Select(gt => gt.Goal).ToList();

            DialogParameters parameters = new DialogParameters
            {
                { "Message", "Select goals to assign to the patient" },
                { "AssignedGoals", assignedGoals }
            };

            IDialogReference dialog = DialogService.Show<GoalSelectDialog>("Manage Goals", parameters);
            DialogResult result = await dialog.Result;

            if (!result.Canceled)
            {
                List<Goal> selectedGoals = result.Data as List<Goal>;

                if (selectedGoals != null)
                {
                    if (currentPatient.Id == 0)
                    {
                        _newPatientGoals = selectedGoals;
                    }

                    else
                    {
                        List<Goal> goalsToRemove = assignedGoals.Where(g => !selectedGoals.Contains(g)).ToList();
                        List<Goal> goalsToAdd = selectedGoals.Where(g => !assignedGoals.Contains(g)).ToList();

                        if (goalsToRemove.Any())
                        {
                            await PatientService.DeassignGoalsFromPatient(currentPatient, goalsToRemove);
                        }

                        if (goalsToAdd.Any())
                        {
                            await PatientService.AssignGoalsToPatient(currentPatient, goalsToAdd);
                        }
                    }
                }
            }
        }
    }
}
