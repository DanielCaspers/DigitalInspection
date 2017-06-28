using DigitalInspection.ViewModels;
using System.Net;

namespace DigitalInspection.Services
{
	public static class ToastService
	{

		public static ToastViewModel FileLockRequired()
		{
			return new ToastViewModel
			{
				Icon = "error",
				Message = "Please request edit rights again.",
				Type = ToastType.Error,
				Action = ToastActionType.Refresh
			};
		}

		public static ToastViewModel FileLockedByAnotherClient(string message, ToastActionType action = ToastActionType.NavigateBack)
		{
			return new ToastViewModel
			{
				Icon = "lock",
				Message = message,
				Type = ToastType.Warn,
				Action = action
			};
		}

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

		public static ToastViewModel ResourceNotFound(string resource, ToastActionType action = ToastActionType.Refresh)
		{
			return new ToastViewModel
			{
				Icon = "error",
				Message = resource + " could not be found.",
				Type = ToastType.Error,
				Action = action
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

		public static ToastViewModel UnknownErrorOccurred(HttpStatusCode httpCode, string errorMessage)
		{
			return new ToastViewModel
			{
				Icon = "error",
				Message = "An unknown error occurred. Write down these details! " + httpCode + ":" + errorMessage,
				Type = ToastType.Error,
				Action = ToastActionType.Refresh
			};
		}
	}
}