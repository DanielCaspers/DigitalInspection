using DigitalInspection.Utils;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace DigitalInspection.Models
{

	public class Vehicle
	{
		[RegularExpression("[A-HJ-NPR-Z0-9]{13}[0-9]{4}", ErrorMessage = "Invalid VIN. Please check the format.")]
		[Required(ErrorMessage = "VIN is required")]
		[DisplayName("VIN *")]
		public string VIN { get; set; }

		[Required(ErrorMessage = "Year is required")]
		[DisplayName("Year *")]
		public int? Year { get; set; }

		[Required(ErrorMessage = "Make is required")]
		[DisplayName("Make *")]
		public string Make { get; set; }

		[Required(ErrorMessage = "Model is required")]
		[DisplayName("Model *")]
		public string Model { get; set; }

		[Required(ErrorMessage = "Color is required")]
		[DisplayName("Color *")]
		public string Color { get; set; }

		[Required(ErrorMessage = "License plate is required")]
		[DisplayName("License plate *")]
		public string License { get; set; }

		[Required(ErrorMessage = "Transmission is required")]
		[DisplayName("Transmission *")]
		public string Transmission { get; set; }

		[Required(ErrorMessage = "Engine is required")]
		[DisplayName("Engine *")]
		public string Engine { get; set; }

		[Required(ErrorMessage = "Mileage is required")]
		[DisplayName("Mileage *")]
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