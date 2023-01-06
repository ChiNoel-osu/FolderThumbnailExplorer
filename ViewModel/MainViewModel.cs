using CommunityToolkit.Mvvm.ComponentModel;

namespace FolderThumbnailExplorer.ViewModel
{
	public partial class MainViewModel : ObservableObject
	{
		public NavSideBarViewModel NavSideBarViewModel { get; set; }
		public FavFoldersViewModel FavFoldersViewModel { get; set; }
		public MainPageViewModel MainPageViewModel { get; set; }

		[ObservableProperty]
		object _RightView;

		public MainViewModel()
		{
			App.Logger.Trace("Initialzing ViewModels");
			NavSideBarViewModel = new NavSideBarViewModel();
			FavFoldersViewModel = new FavFoldersViewModel();
			MainPageViewModel = new MainPageViewModel();
		}
	}
}
