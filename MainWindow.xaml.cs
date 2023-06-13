using FolderThumbnailExplorer.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows;

namespace FolderThumbnailExplorer
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public static MainViewModel MainVM { get; private set; } = new MainViewModel();
		public MainWindow(string path)
		{
			App.Logger.Info("Loading MainWindow and its components...");
			MainVM.MainPageViewModel.PATHtoShow = path; //Command line argument.
			DataContext = MainVM;
			InitializeComponent();
			this.Closed += MainWindow_Closed;
			App.Logger.Info("MainWindow loaded.");
			App.Logger.Info("The Application has started and is ready to use.");
		}

		private void MainWindow_Closed(object? sender, EventArgs e)
		{
			App.Logger.Info("Closing all windows.");
			//Make a copy of windows list so it won't change during the enumeration.
			List<Window> windows2Close = new List<Window>(MainPageViewModel.wnds);
			foreach (Window wnd in windows2Close)
				wnd.Close();
			App.Logger.Info("All windows closed without errors.");
		}
	}
}
