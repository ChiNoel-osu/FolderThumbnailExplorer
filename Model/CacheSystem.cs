using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Media.Imaging;

namespace FolderThumbnailExplorer.Model
{
	public class CacheSystem
	{
		static readonly string cacheFolder = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "Cache";

		public static string SHA1Hash(string str)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (byte h in SHA1.HashData(Encoding.ASCII.GetBytes(str)))
				stringBuilder.Append(h.ToString("X2"));
			return stringBuilder.ToString();
		}

		public static bool IsImgCached(string imgPath)
		{
			string hash = SHA1Hash(imgPath);
			return File.Exists(Path.Combine(Path.Combine(cacheFolder, hash[..2]), hash));
		}
		public static void CacheImage(string imgPath, BitmapImage image)
		{
			string hash = SHA1Hash(imgPath);
			string cacheImgPath = Path.Combine(Path.Combine(cacheFolder, hash[..2]), hash);
			Directory.CreateDirectory(cacheImgPath.Remove(cacheImgPath.LastIndexOf(Path.DirectorySeparatorChar)));
			using FileStream fs = File.Create(cacheImgPath);
			JpegBitmapEncoder jpegBitmapEncoder = new JpegBitmapEncoder();
			jpegBitmapEncoder.Frames.Add(BitmapFrame.Create(image));
			jpegBitmapEncoder.QualityLevel = Properties.Settings.Default.TE_CacheQuality;
			jpegBitmapEncoder.Save(fs);
			fs.Close();
		}
		public static BitmapImage GetCachedImg(string imgPath)
		{
			string hash = SHA1Hash(imgPath);
			using FileStream stream = File.OpenRead(Path.Combine(Path.Combine(cacheFolder, hash[..2]), hash));
			BitmapImage bitmapImage = new BitmapImage();
			bitmapImage.BeginInit();
			bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
			bitmapImage.StreamSource = stream;
			bitmapImage.EndInit();
			bitmapImage.Freeze();
			stream.Close();
			return bitmapImage;
		}
	}
}
