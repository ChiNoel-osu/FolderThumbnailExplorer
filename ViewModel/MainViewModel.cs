using CommunityToolkit.Mvvm.Input;

namespace FolderThumbnailExplorer.ViewModel
{
	public partial class MainViewModel
	{
		public NavSideBarViewModel NavSideBarViewModel { get; private set; }
		public MainPageViewModel MainPageViewModel { get; private set; }
		public SettingsPageViewModel SettingsPageViewModel { get; private set; }

		[RelayCommand]
		public static void RestartApplication()
		{
			System.Diagnostics.Process.Start(System.Diagnostics.Process.GetCurrentProcess().ProcessName);
			App.Current.Shutdown();
		}

		public MainViewModel()
		{
			App.Logger.Trace("Initializing ViewModels....");
			NavSideBarViewModel = new NavSideBarViewModel();
			MainPageViewModel = new MainPageViewModel();
			SettingsPageViewModel = new SettingsPageViewModel();
			App.Logger.Trace("ViewModels initialized.");
		}
	}
}
