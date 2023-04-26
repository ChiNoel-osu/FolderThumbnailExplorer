using FolderThumbnailExplorer.ViewModel;
using System.Windows;

namespace FolderThumbnailExplorer.View
{
	/// <summary>
	/// Interaction logic for AddFav2Group.xaml
	/// </summary>
	public partial class AddFav2Group : Window
	{
		public AddFav2Group(string path)
		{
			InitializeComponent();
			DataContext = new AddNewFavoriteViewModel(path);
		}
	}
}
