using System.ComponentModel.DataAnnotations;
using System.Web;

namespace DigitalInspection.Models.Validators
{
	public class MaxFileSizeAttribute : ValidationAttribute
	{
		private readonly int _maxFileSize;
		public MaxFileSizeAttribute(int maxFileSize)
		{
			_maxFileSize = maxFileSize;
		}

		public override bool IsValid(object value)
		{
			if (value is HttpPostedFileBase file)
			{
				return file.ContentLength <= _maxFileSize;
			}
			return false;
		}

		public override string FormatErrorMessage(string name)
		{
			return base.FormatErrorMessage(_maxFileSize.ToString());
		}
	}
}
