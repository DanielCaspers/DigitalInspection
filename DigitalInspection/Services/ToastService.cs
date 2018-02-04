using System;
using DigitalInspection.ViewModels;
using System.Net;

namespace DigitalInspection.Services
{
	public static class ToastService
	{

		public static ToastViewModel FileLockRequired()
		{
			return ErrorInternal("Please request edit rights again.");
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
			return ErrorInternal("An unknown error occurred.");
		}

		public static ToastViewModel UnknownErrorOccurred(Exception e)
		{
			return ErrorInternal("An unknown error occurred." + Environment.NewLine + Environment.NewLine + GetInnermostException(e)?.Message);
		}

		public static ToastViewModel UnknownErrorOccurred(HttpStatusCode httpCode, string errorMessage)
		{
			return ErrorInternal("An unknown error occurred. Write down these details! " + httpCode + ":" + errorMessage);
		}

		public static ToastViewModel DatabaseException(Exception e)
		{
			return ErrorInternal("Could not perform operation on the database: " + Environment.NewLine + Environment.NewLine + GetInnermostException(e)?.Message);
		}

		private static ToastViewModel ErrorInternal(string message)
		{
			return new ToastViewModel
			{
				Icon = "error",
				Message = message,
				Type = ToastType.Error,
				Action = ToastActionType.Refresh
			};
		}

		private static Exception GetInnermostException(Exception e)
		{
			while (e.InnerException != null)
			{
				e = e.InnerException;
			}

			return e;
		}
	}
}