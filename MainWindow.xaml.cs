using FolderThumbnailExplorer.ViewModel;
using System;
using System.Windows;

namespace FolderThumbnailExplorer
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public static MainViewModel MainVM { get; set; } = new MainViewModel();
		public MainWindow()
		{
			App.Logger.Info("Loading MainWindow and its components...");
			DataContext = MainVM;
			InitializeComponent();
			this.Closed += MainWindow_Closed;
			App.Logger.Info("MainWindow loaded.");
			App.Logger.Info("The Application has started and is ready to use.");
		}

		private void MainWindow_Closed(object? sender, EventArgs e)
		{
			App.Logger.Info("Closing all windows.");
			foreach (Window wnd in MainPageViewModel.wnds)  //TODO: has bugs but doesn't matter.
				wnd.Close();
			App.Logger.Info("All windows closed without errors.");
		}
	}
}
