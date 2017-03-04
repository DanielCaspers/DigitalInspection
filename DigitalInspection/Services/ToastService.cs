using DigitalInspection.ViewModels;

namespace DigitalInspection.Services
{
	public static class ToastService
	{
		public static ToastViewModel ResourceNotFound(string resource)
		{
			return new ToastViewModel
			{
				Icon = "error",
				Message = resource + " could not be found.",
				Type = ToastType.Error,
				Action = ToastActionType.Refresh
			};
		}

		public static ToastViewModel UnknownErrorOccurred()
		{
			return new ToastViewModel
			{
				Icon = "error",
				Message = "An unknown error occurred.",
				Type = ToastType.Error,
				Action = ToastActionType.Refresh
			};
		}
	}
}