namespace FolderThumbnailExplorer.ViewModel
{
	public class MainViewModel
	{
		public NavSideBarViewModel NavSideBarViewModel { get; set; }
		public DriveSelectViewModel DriveSelectViewModel { get; set; }
		public MainPageViewModel MainPageViewModel { get; set; }

		public MainViewModel()
		{
			NavSideBarViewModel = new NavSideBarViewModel();
			DriveSelectViewModel = new DriveSelectViewModel();
			MainPageViewModel = new MainPageViewModel();
		}
	}
}
