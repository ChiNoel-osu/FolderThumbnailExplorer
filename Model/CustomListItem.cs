using System.Windows.Media.Imaging;

namespace FolderThumbnailExplorer.Model
{
	public struct CustomListItem
	{   //For PhotoViewer use
		public BitmapImage Image { get; set; }
		public string Name { get; set; }
		public string Path { get; set; }
	}
}
