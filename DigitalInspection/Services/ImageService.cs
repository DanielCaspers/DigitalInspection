using DigitalInspection.Models;
using System;
using System.Web;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

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
				var imageDirectoryPath = CreateStorageFolders(UPLOAD_DIR, uploadSubdirectory);
				var imageFileName = fileNamePrefix + "_" + picture.FileName;
				var imagePath = Path.Combine(imageDirectoryPath, imageFileName);
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

		private static string CreateStorageFolders(string uploadDir, string subdir)
		{
			var imageDirectoryPath = Path.Combine(HttpContext.Current.Server.MapPath(UPLOAD_DIR), subdir);
			DirectoryInfo di = Directory.CreateDirectory(imageDirectoryPath); // No folders are created if they already exist
			// SetPathAccessControl(imageDirectoryPath);
			return imageDirectoryPath;
		}

		// http://stackoverflow.com/a/5398398/2831961
		private static void SetPathAccessControl(string path)
		{
			DirectorySecurity sec = Directory.GetAccessControl(path);
			// Using this instead of the "Everyone" string means we work on non-English systems.
			SecurityIdentifier everyone = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
			sec.AddAccessRule(new FileSystemAccessRule(everyone, FileSystemRights.Modify | FileSystemRights.Synchronize, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
			Directory.SetAccessControl(path, sec);
		}


	}
}