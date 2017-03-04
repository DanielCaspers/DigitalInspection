using DigitalInspection.Models;
using System;
using System.Web;
using System.IO;

namespace DigitalInspection.Services
{
	public static class ImageService
	{
		private static readonly string UPLOAD_DIR = "~/Uploads/";

		public static Image SaveImage(HttpPostedFileBase picture, string uploadSubdirectory, string fileNamePrefix)
		{
			// TODO Improve error handling and prevent NPEs
			if (picture != null && picture.ContentLength > 0)
			{
				var imageFileName = fileNamePrefix + "_" + picture.FileName;
				var imagePath = Path.Combine(HttpContext.Current.Server.MapPath(UPLOAD_DIR), uploadSubdirectory, imageFileName);
				picture.SaveAs(imagePath);

				return new Image
				{
					Title = imageFileName,
					ImageUrl = imagePath
				};
			}

			return null;
		}

		public static void DeleteImage(Image picture, string subdirectory)
		{
			if (picture != null && File.Exists(picture.ImageUrl))
			{
				try
				{
					File.Delete(picture.ImageUrl);
				}
				catch (IOException e)
				{
					Console.WriteLine(e.Message);
				}
			}
		}


	}
}