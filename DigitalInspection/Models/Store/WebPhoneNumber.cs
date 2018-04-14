using DigitalInspection.Models.Orders;

namespace DigitalInspection.Models.Store
{
	public class WebPhoneNumber : PhoneNumber
	{
		public string NumberForWebLink { get; set; }

		public WebPhoneNumber() { }

		public WebPhoneNumber(
			string contactName,
			string numberForDisplay,
			string numberForWebLink) {

			ContactName = contactName;
			Number = numberForDisplay;
			NumberForWebLink = numberForWebLink;
		}
	}
}
