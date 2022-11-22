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
			DataContext = MainVM;
			InitializeComponent();
			this.Closed += MainWindow_Closed;
		}

		private void MainWindow_Closed(object? sender, EventArgs e)
		{
			foreach (Window wnd in MainPageViewModel.wnds)
				wnd.Close();
		}
	}
}
