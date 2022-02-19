using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Web;
using Image = DigitalInspection.Models.Inspections.Image;

namespace DigitalInspection.Services.Core
{
	public static class ImageService
	{
		private static readonly string UPLOAD_DIR = "~/Uploads/";

		public static Image SaveImage(HttpPostedFileBase file, string uploadSubdirectory, string fileNamePrefix)
		{
			if (!IsValid(file))
			{
				return null;
			}

			var imageDirectoryPath = CreateFolderTree(UPLOAD_DIR, new[] { uploadSubdirectory });
			return SaveImageInternal(file, imageDirectoryPath, fileNamePrefix);
		}

		// uploadDirectoryTree represents a route tree, like the Angular router. 
		public static Image SaveImage(HttpPostedFileBase file, string[] uploadDirectoryTree, string fileNamePrefix)
		{
			if (!IsValid(file))
			{
				return null;
			}

			var imageDirectoryPath = CreateFolderTree(UPLOAD_DIR, uploadDirectoryTree);
			return SaveImageInternal(file, imageDirectoryPath, fileNamePrefix);
		}

		public static void DeleteImage(Image file)
		{
			if (file != null && File.Exists(file.ImageUrl))
			{
				try
				{
					File.Delete(file.ImageUrl);
				}
				catch (IOException e)
				{
					Console.WriteLine(e.Message);
				}
			}
		}

		private static string CreateFolderTree(string baseDir, string[] dirTree)
		{
			// Starts with already created directory, and incrementally builds to solution
			string folderPath = HttpContext.Current.Server.MapPath(UPLOAD_DIR);
			foreach(string folderToCreate in dirTree)
			{
				folderPath = Path.Combine(folderPath, folderToCreate);
				DirectoryInfo di = Directory.CreateDirectory(folderPath); // No folders are created if they already exist
				// SetPathAccessControl(folderPath);
			}

			return folderPath;
		}

		private static bool IsValid(HttpPostedFileBase file)
		{
			if (file == null || file.ContentLength == 0)
			{
				return false;
			}

			// Only images and videos are valid for upload.
			return file.ContentType.StartsWith("image/") || file.ContentType.StartsWith("video/");
		}

		private static Image SaveImageInternal(HttpPostedFileBase file, string imageDirectoryPath, string fileNamePrefix)
		{
			var imageFileName = fileNamePrefix + "_" + file.FileName;
			var imagePath = Path.Combine(imageDirectoryPath, imageFileName);

			file.SaveAs(imagePath);

			return new Image
			{
				Title = imageFileName,
				ImageUrl = imagePath
			};
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