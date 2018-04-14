using System.Collections.Generic;
using System.Linq;
using DigitalInspection.Models.Orders;

namespace DigitalInspection.Models.Inspections.Reports
{
	public class InspectionReportGroup
	{
		public string Name { get; set; }

		public RecommendedServiceSeverity Condition { get; set; }

		public IEnumerable<InspectionReportItem> Items { get; set; }

		public InspectionReportGroup(IGrouping<string, InspectionItem> ig, string baseUrl)
		{
			var items = ig.OrderBy(ii => ii.Condition).ToList();

			Name = ig.Key;
			Condition = items.First().Condition;
			Items = items.Select(ii => new InspectionReportItem(ii, baseUrl));
		}
	}
}
