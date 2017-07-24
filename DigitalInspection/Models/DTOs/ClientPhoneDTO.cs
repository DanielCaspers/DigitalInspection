
namespace DigitalInspection.Models.DTOs
{
	public class ClientPhoneDTO
	{
		public string name { get; set; }
		public string number { get; set; }
		public string type { get; set; }
		public string smsPrefs { get; set; }

		public ClientPhoneDTO() { }

		public ClientPhoneDTO(
			string phoneNumber,
			string contact,
			string phoneType,
			string smsPreferences) {
			number = phoneNumber;
			name = contact;
			type = phoneType;
			smsPrefs = smsPreferences;
		}
	}
}