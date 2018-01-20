namespace DigitalInspection.Models.Store
{
	public class StoreFeatures
	{
		public string WeekdayHours { get; set; }
		public string SaturdayHours { get; set; }
		public string NumberOfBays { get; set; }
		public string NumberOfTechs { get; set; }
		public WebPhoneNumber TowCompanyInfo { get; set; }

		public StoreFeatures() { }

		public StoreFeatures(
			string weekdayHours,
			string saturdayHours,
			string numberOfBays,
			string numberOfTechs,
			string towCompanyName,
			string towCompanyPhoneForDisplay,
			string towCompanyPhoneForTelLink) {

			WeekdayHours = weekdayHours;
			SaturdayHours = saturdayHours;
			NumberOfBays = numberOfBays;
			NumberOfTechs = numberOfTechs;
			TowCompanyInfo = new WebPhoneNumber(
				towCompanyName,
				towCompanyPhoneForDisplay,
				towCompanyPhoneForTelLink);
		}
	}
}
