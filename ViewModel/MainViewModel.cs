namespace FolderThumbnailExplorer.ViewModel
{
	public partial class MainViewModel
	{
		public NavSideBarViewModel NavSideBarViewModel { get; private set; }
		public MainPageViewModel MainPageViewModel { get; private set; }

		public MainViewModel()
		{
			App.Logger.Trace("Initializing ViewModels");
			NavSideBarViewModel = new NavSideBarViewModel();
			MainPageViewModel = new MainPageViewModel();
		}
	}
}
