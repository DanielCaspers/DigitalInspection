﻿using System;
using System.ComponentModel.DataAnnotations;

namespace DigitalInspection.Models
{
	//http://cpratt.co/file-uploads-in-asp-net-mvc-with-view-models/
	public class Image
	{
		//public Guid Id { get; set; }

		//[Required]
		public string Title { get; set; }

		public string Caption { get; set; }

		//[Required]
		[DataType(DataType.ImageUrl)]
		public string ImageUrl { get; set; }

		//[Required]
		[DataType(DataType.DateTime)]
		public DateTime CreatedDate
		{
			get { return createdDate ?? DateTime.UtcNow; }
			set { createdDate = value; }
		}

		private DateTime? createdDate;
	}
}