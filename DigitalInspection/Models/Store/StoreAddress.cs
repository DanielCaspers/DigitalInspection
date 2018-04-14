using DigitalInspection.Models.Orders;

namespace DigitalInspection.Models.Store
{
	public class StoreAddress : Address
	{
		public string StateShort { get; set; }
		public string Latitude { get; set; }
		public string Longitude { get; set; }
		public string NearbyLandmark { get; set; }

		public StoreAddress() { }

		public StoreAddress(
			string line1,
			string city,
			string state,
			string stateShort,
			string zip,
			string latitude,
			string longitude,
			string nearbyLandmark) {

			Line1 = line1;
			City = city;
			State = state;
			StateShort = stateShort;
			ZIP = zip;
			Latitude = latitude;
			Longitude = longitude;
			NearbyLandmark = nearbyLandmark;
		}
	}
}
