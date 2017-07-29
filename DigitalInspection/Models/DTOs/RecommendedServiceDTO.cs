﻿using DigitalInspection.Models.Orders;
using System;

namespace DigitalInspection.Models.DTOs
{
	public class RecommendedServiceDTO
	{
		public string id { get; set; }
		public string desc { get; set; }
		public string orderID { get; set; }
		public long? date { get; set; }
		public string techNum { get; set; }
		public string DIlink { get; set; }
		public string estID { get; set; }
		public int? notificationCnt { get; set; }
		public string level { get; set; }

		public RecommendedServiceDTO() { }

		public RecommendedServiceDTO(
			string _id,
			string _desc,
			string _orderId,
			long? _lastModifiedDate,
			string _technicianId,
			string _appLink,
			string _estimateId,
			int? _notificationCount,
			RecommendedServiceSeverity _level,
			bool _isCustomerConcern)
		{
			id = _id;
			desc = _desc;
			orderID = _orderId;
			date = _lastModifiedDate;
			techNum = _technicianId;
			DIlink = _appLink;
			estID = _estimateId;
			notificationCnt = _notificationCount;

			if (_level == RecommendedServiceSeverity.UNKNOWN)
			{
				level = string.Empty;
			}
			else
			{
				if (_isCustomerConcern == true)
				{
					level = "0";
				}
				else
				{
					level = "";
				}

				level += (int) _level;
			}
		}
	}
}