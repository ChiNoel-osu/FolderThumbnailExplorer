using System.Windows.Media.Imaging;

namespace FolderThumbnailExplorer.Model
{
	public class CustomContentItem
	{
		public BitmapImage ThumbNail { get; set; }
		public string Header { get; set; }
		public bool HasSubfolder { get; set; } = false;
	}
}
