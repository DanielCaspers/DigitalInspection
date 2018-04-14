namespace DigitalInspection.Models.Inspections.Reports
{
	public class CannedResponseReportItem
	{
		public string Response { get; set; }

		public string Description { get; set; }

		public string Url { get; set; }

		public CannedResponseReportItem(string response, string description, string url)
		{
			Response = response;
			Description = description;
			Url = url;
		}
	}
}
