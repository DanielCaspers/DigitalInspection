using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DigitalInspection.Models
{

	public enum Transmission { Auto, Manual }

	public class Vehicle
	{
		[RegularExpression("[A-HJ-NPR-Z0-9]{13}[0-9]{4}", ErrorMessage = "Invalid VIN. Please check the format.")]
		[Required(ErrorMessage = "VIN is required")]
		[DisplayName("VIN *")]
		public string VIN { get; set; }

		[Required(ErrorMessage = "Year is required")]
		[DisplayName("Year *")]
		public int Year { get; set; }

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
		public string LicensePlate { get; set; }

		[Required(ErrorMessage = "License state is required")]
		[DisplayName("License state *")]
		public string LicenseState { get; set; }

		[Required(ErrorMessage = "Transmission is required")]
		[DisplayName("Transmission *")]
		public Transmission Transmission { get; set; }

		[Required(ErrorMessage = "Engine size is required")]
		[DisplayName("Engine size *")]
		public float EngineDisplacement { get; set; }

		[Required(ErrorMessage = "Mileage is required")]
		[DisplayName("Mileage *")]
		public int Mileage { get; set; }
	}
}