namespace FolderThumbnailExplorer.ViewModel
{
	public class MainViewModel
	{
		public DriveSelectViewModel DriveSelectViewModel { get; set; }
		public MainPageViewModel MainPageViewModel { get; set; }

		public MainViewModel()
		{
			DriveSelectViewModel = new DriveSelectViewModel();
			MainPageViewModel = new MainPageViewModel();
		}
	}
}
