using DigitalInspection.ViewModels;

namespace DigitalInspection.Services
{
	public static class ToastService
	{
		public static ToastViewModel NotYetImplemented()
		{
			return new ToastViewModel
			{
				Icon = "error",
				Message = "This feature isn't ready yet. Please navigate back and refresh the page.",
				Type = ToastType.Error,
				Action = ToastActionType.NavigateBack
			};
		}

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