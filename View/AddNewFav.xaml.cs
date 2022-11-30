using FolderThumbnailExplorer.ViewModel;
using System.Windows;

namespace FolderThumbnailExplorer.View
{
	/// <summary>
	/// AddNewFav.xaml 的交互逻辑
	/// </summary>
	public partial class AddNewFav : Window
	{
		public AddNewFav(string defaultPath = "")
		{
			InitializeComponent();
			DataContext = new AddNewFavoriteViewModel(defaultPath);
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			NameBox.Focus();
		}
	}
}
