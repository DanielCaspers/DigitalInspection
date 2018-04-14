using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Web;

namespace DigitalInspection.Services.Core
{
	public static class ImageService
	{
		private static readonly string UPLOAD_DIR = "~/Uploads/";

		public static Models.Image SaveImage(HttpPostedFileBase picture, string uploadSubdirectory, string fileNamePrefix, bool compress = true)
		{
			// TODO Improve error handling and prevent NPEs
			if (picture != null && picture.ContentLength > 0)
			{
				var imageDirectoryPath = CreateFolderTree(UPLOAD_DIR, new string[] { uploadSubdirectory });
				return SaveImageInternal(picture, imageDirectoryPath, fileNamePrefix, compress);
			}

			return null;
		}

		// uploadDirectoryTree represents a route tree, like the Angular router. 
		public static Models.Image SaveImage(HttpPostedFileBase picture, string[] uploadDirectoryTree, string fileNamePrefix, bool compress = true)
		{
			// TODO Improve error handling and prevent NPEs
			if (picture != null && picture.ContentLength > 0)
			{
				var imageDirectoryPath = CreateFolderTree(UPLOAD_DIR, uploadDirectoryTree);
				return SaveImageInternal(picture, imageDirectoryPath, fileNamePrefix, compress);
			}

			return null;
		}

		public static void DeleteImage(Models.Image picture, string subdirectory)
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

		private static Models.Image SaveImageInternal(HttpPostedFileBase picture, string imageDirectoryPath, string fileNamePrefix, bool compress)
		{
			var imageFileName = fileNamePrefix + "_" + picture.FileName;
			var imagePath = Path.Combine(imageDirectoryPath, imageFileName);
			System.Drawing.Image image = System.Drawing.Image.FromStream(picture.InputStream, true, true);
			// TODO: Allow users of function to pass in image quality, or decide whether or not to save at some setting. Maybe custom enum model for image quality?

			if (compress)
			{
				image = ReduceImageSize(image, 240, 320);
			}

			image.Save(imagePath);

			return new Models.Image
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

		// http://stackoverflow.com/a/21394605/2831961
		private static Bitmap ReduceImageSize(System.Drawing.Image image, int height, int width)
		{
			Bitmap newImg = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
			Graphics newGraphic = Graphics.FromImage(newImg);

			newGraphic.InterpolationMode = InterpolationMode.Bicubic;
			newGraphic.DrawImage(image, 0, 0, width, height);
			newGraphic.Dispose();

			return newImg;
		}


	}
}