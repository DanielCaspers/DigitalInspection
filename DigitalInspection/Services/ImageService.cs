using DigitalInspection.Models;
using System;
using System.Web;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace DigitalInspection.Services
{
	public static class ImageService
	{
		private static readonly string UPLOAD_DIR = "~/Uploads/";

		public static Models.Image SaveImage(HttpPostedFileBase picture, string uploadSubdirectory, string fileNamePrefix)
		{
			// TODO Improve error handling and prevent NPEs
			if (picture != null && picture.ContentLength > 0)
			{
				var imageDirectoryPath = CreateStorageFolders(UPLOAD_DIR, uploadSubdirectory);
				var imageFileName = fileNamePrefix + "_" + picture.FileName;
				var imagePath = Path.Combine(imageDirectoryPath, imageFileName);
				// TODO: Allow users of function to pass in image quality, or decide whether or not to save at some setting. Maybe custom enum model for image quality?
				Bitmap processedImage = ReduceImageSize(picture, 240, 320);
				processedImage.Save(imagePath);

				return new Models.Image
				{
					Title = imageFileName,
					ImageUrl = imagePath
				};
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

		// http://stackoverflow.com/a/21394605/2831961
		private static Bitmap ReduceImageSize(HttpPostedFileBase imageFile, int height, int width)
		{
			System.Drawing.Image image = System.Drawing.Image.FromStream(imageFile.InputStream, true, true);

			Bitmap newImg = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
			Graphics newGraphic = Graphics.FromImage(newImg);

			newGraphic.InterpolationMode = InterpolationMode.Bicubic;
			newGraphic.DrawImage(image, 0, 0, width, height);
			newGraphic.Dispose();

			return newImg;
		}


	}
}