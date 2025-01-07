namespace LinguisticStimulationPlanner.Models
{
	public class Location
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Address { get; set; }


		public ICollection<Patient> Patients { get; set; } = new List<Patient>();

		public bool IsValidLocation()
		{
			return !(string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Address));
		}
	}
}
