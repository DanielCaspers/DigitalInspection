namespace DigitalInspection.Models.Store
{
	public class StoreIntegrations
	{
		public string MapQuestURL { get; set; }
		public string FaceBookURL { get; set; }
		public string FaceBookBadge { get; set; }
		public string GooglePlusID { get; set; }
		public string GoogleSiteURL { get; set; }
		public string GoogleReviewURL { get; set; }
		public string GooglePlacesRef { get; set; }
		public string YahooSiteURL { get; set; }
		public string YelpSiteURL { get; set; }
		public string InsiderPagesURL { get; set; }
		public string DexSiteURL { get; set; }

		public StoreIntegrations() { }

		public StoreIntegrations(
			string mapQuestURL,
			string faceBookURL,
			string faceBookBadge,
			string googlePlusID,
			string googleSiteURL,
			string googleReviewURL,
			string googlePlacesRef,
			string yahooSiteURL,
			string yelpSiteURL,
			string insiderPagesURL,
			string dexSiteURL) {

			MapQuestURL = mapQuestURL;
			FaceBookURL = faceBookURL;
			FaceBookBadge = faceBookBadge;
			GooglePlusID = googlePlusID;
			GoogleSiteURL = googleSiteURL;
			GoogleReviewURL = googleReviewURL;
			GooglePlacesRef = googlePlacesRef;
			YahooSiteURL = yahooSiteURL;
			YelpSiteURL = yelpSiteURL;
			InsiderPagesURL = insiderPagesURL;
			DexSiteURL = dexSiteURL;
		}
	}
}
