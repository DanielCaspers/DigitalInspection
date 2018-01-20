using System.Collections.Generic;

namespace DigitalInspection.Models.Store
{
	public class StoreInfo
	{
		public StoreInfo() { }

		public string Name { get; set; }

		public string NameShort { get; set; }

		public string ManagerFirstName { get; set; }

		public string ManagerLastName { get; set; }

		public string CodeName { get; set; }

		public WebPhoneNumber PhoneNumberToCall { get; set; }

		public WebPhoneNumber PhoneNumberToSMS { get; set; }

		public StoreAddress StoreAddress { get; set; }

		public IList<string> CommunitiesServed;

		public StoreFeatures StoreFeatures { get; set; }

		public StoreIntegrations StoreIntegrations { get; set; }

		public StoreWebAssets StoreWebAssets { get; set; }
	}
}
