using System.Diagnostics;
using System.Web.Mvc;

namespace DigitalInspection.ViewModels.Base
{
	public class BaseErrorModel : BaseViewModel
	{
		public HandleErrorInfo Error { get; set; }

		public StackTrace StackTrace { get; set; }
	}
}