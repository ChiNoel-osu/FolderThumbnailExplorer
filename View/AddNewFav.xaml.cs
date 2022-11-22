using System.IO;
using System.Windows;

namespace FolderThumbnailExplorer.View
{
	/// <summary>
	/// AddNewFav.xaml 的交互逻辑
	/// </summary>
	public partial class AddNewFav : Window
	{
		public AddNewFav(string defaultPath)
		{
			InitializeComponent();
			PathBox.Text = defaultPath;
		}
		private void SaveBtn_Click(object sender, RoutedEventArgs e)
		{
			string favPath = Directory.GetCurrentDirectory() + "\\Favorites.txt";
			string[] strings = { "Name|" + NameBox.Text, "Path|" + PathBox.Text };
			if (!File.Exists(favPath))
				File.Create(favPath).Close();
			File.AppendAllLines(favPath, strings);
			Close();
		}
	}
}
