using System.Data.Entity.Validation;
using System.Diagnostics;

namespace DigitalInspection.Services
{
	// TODO Review with C# Devs at work to find a better solution. Perhaps a global event observer?
	public static class ExceptionHandlerService
	{
		public static void HandleException(DbEntityValidationException dbEx)
		{
			foreach (var validationErrors in dbEx.EntityValidationErrors)
			{
				foreach (var validationError in validationErrors.ValidationErrors)
				{
					Trace.TraceError("Property: {0} Error: {1}",
											validationError.PropertyName,
											validationError.ErrorMessage);
				}
			}
		}
	}
}