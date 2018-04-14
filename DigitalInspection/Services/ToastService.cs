using System;
using DigitalInspection.ViewModels;
using System.Net;
using System.Web.Mvc;
using DigitalInspection.Models.Orders;
using DigitalInspection.Models.Web;

namespace DigitalInspection.Services
{
	public static class ToastService
	{

		public static ToastViewModel FileLockRequired()
		{
			return Error("Work order lock expired - Please request edit rights again.");
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
			return Error("This feature isn't ready yet. Please navigate back and refresh the page.", ToastActionType.NavigateBack);
		}

		public static ToastViewModel ResourceNotFound(string resource, ToastActionType action = ToastActionType.Refresh)
		{
			return Error(resource + " could not be found.", action);
		}

		public static ToastViewModel UnknownErrorOccurred()
		{
			return Error("An unknown error occurred.");
		}

		public static ToastViewModel UnknownErrorOccurred(Exception e)
		{
			return Error("An unknown error occurred." + Environment.NewLine + Environment.NewLine + GetInnermostException(e)?.Message);
		}

		public static ToastViewModel UnknownErrorOccurred(Exception e, HandleErrorInfo info)
		{
			Exception inner = GetInnermostException(e);
			return Error(
				"An unknown error occurred at " + info.ControllerName + "/" + info.ActionName + "." + 
				Environment.NewLine +
				Environment.NewLine +
				"Innermost exception: (" + inner?.GetType() +  ") " + inner?.Message
			);
		}

		public static ToastViewModel UnknownErrorOccurred(HttpStatusCode httpCode, string errorMessage)
		{
			return Error("An unknown error occurred. Write down these details! " + httpCode + ":" + errorMessage);
		}

		public static ToastViewModel DatabaseException(Exception e)
		{
			return Error("Could not perform operation on the database: " + Environment.NewLine + Environment.NewLine + GetInnermostException(e)?.Message);
		}

		public static ToastViewModel Error(string message, ToastActionType action = ToastActionType.Refresh)
		{
			return new ToastViewModel
			{
				Icon = "error",
				Message = message,
				Type = ToastType.Error,
				Action = action
			};
		}

		public static ToastViewModel WorkOrderError(HttpResponse<WorkOrder> response)
		{
			switch (response.HTTPCode)
			{
				case HttpStatusCode.NotFound:
					return ResourceNotFound("Work order", ToastActionType.NavigateBack);
				case (HttpStatusCode)423:
					return FileLockedByAnotherClient(response.ErrorMessage, ToastActionType.Refresh);
				case (HttpStatusCode)428:
					return FileLockRequired();
				default:
					return UnknownErrorOccurred(response.HTTPCode, response.ErrorMessage);
			}
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