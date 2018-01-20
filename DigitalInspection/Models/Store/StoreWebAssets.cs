namespace DigitalInspection.Models.Store
{
	public class StoreWebAssets
	{
		public string WebAddress { get; set; }
		public string SiteMapURL { get; set; }
		public string LogoSmall { get; set; }

		public StoreWebAssets() { }

		public StoreWebAssets(
			string webAddress,
			string siteMapURL,
			string logoSmall) {

			WebAddress = webAddress;
			SiteMapURL = siteMapURL;
			LogoSmall = logoSmall;
		}
	}
}
