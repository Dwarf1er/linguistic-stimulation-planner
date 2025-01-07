using LinguisticStimulationPlanner.Models;
using LinguisticStimulationPlanner.Data;
using Microsoft.EntityFrameworkCore;

namespace LinguisticStimulationPlanner.Services
{
    public class PatientService
    {
        private readonly ApplicationDbContext _context;

        public PatientService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Patient>> GetPatientsAsync()
        {
            return await _context.Patients.Include(p => p.PatientGoals).ThenInclude(pg => pg.Goal)
                                          .Include(p => p.Location)
                                          .ToListAsync();
        }

        public async Task<Patient> CreatePatientAsync(Patient patient)
        {
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
            return patient;
        }

        public async Task<Patient> UpdatePatientAsync(Patient patient)
        {
            _context.Patients.Update(patient);
            await _context.SaveChangesAsync();
            return patient;
        }

        public async Task DeletePatientAsync(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient != null)
            {
                _context.Patients.Remove(patient);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AssignGoalsToPatient(Patient patient, List<Goal> goals)
        {
            foreach (var goal in goals)
            {
                if (!patient.PatientGoals.Any(pg => pg.GoalId == goal.Id))
                {
                    patient.PatientGoals.Add(new PatientGoal
                    {
                        GoalId = goal.Id,
                        PatientId = patient.Id
                    });
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task DeassignGoalsFromPatient(Patient patient, List<Goal> goals)
        {
            foreach (var goal in goals)
            {
                var patientGoal = patient.PatientGoals.FirstOrDefault(pg => pg.GoalId == goal.Id);
                if (patientGoal != null)
                {
                    patient.PatientGoals.Remove(patientGoal);
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task AssignLocationToPatient(Patient patient, Location location)
        {
            patient.Location = location;
            await _context.SaveChangesAsync();
        }
    }
}
