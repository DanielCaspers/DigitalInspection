﻿using DigitalInspection.Utils;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace DigitalInspection.Models
{

	public class Vehicle
	{
		[RegularExpression("[A-HJ-NPR-Z0-9]{13}[0-9]{4}", ErrorMessage = "Invalid VIN. Please check the format.")]
		public string VIN { get; set; }

		public int? Year { get; set; }

		public string Make { get; set; }

		public string Model { get; set; }

		public string Color { get; set; }

		[DisplayName("License plate")]
		public string License { get; set; }

		public string Transmission { get; set; }

		public string Engine { get; set; }

		[DisplayName("Mileage")]
		public int? Odometer { get; set; }

		public Vehicle() { }

		public Vehicle(string vin, int? year, string make, string model, string license, string color, string engine, string transmission, int? odometer)
		{
			VIN = vin;
			Year = year;
			Make = make?.ToTitleCase();
			Model = model?.ToTitleCase();
			License = license;

			Color = color?.ToTitleCase();
			if (System.Drawing.Color.FromName(Color).IsKnownColor == false)
			{
				Color = null;
			}

			Engine = engine;
			Transmission = transmission?.ToTitleCase();
			Odometer = odometer;
		}
	}
}