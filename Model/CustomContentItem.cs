using System.Windows.Media.Imaging;

namespace FolderThumbnailExplorer.Model
{
	public struct CustomContentItem
	{
		public BitmapImage ThumbNail { get; set; }
		public string Header { get; set; }
		public string FullPath { get; set; }
		public bool HasSubfolder { get; set; }
	}
}
