using FolderThumbnailExplorer.ViewModel;
using System.Windows.Controls;

namespace FolderThumbnailExplorer.View
{
	/// <summary>
	/// MainPage.xaml 的交互逻辑
	/// </summary>
	public partial class MainPage : UserControl
	{
		public MainPage()
		{
			InitializeComponent();
		}

		private void MainPage_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			switch (e.ChangedButton)
			{
				case System.Windows.Input.MouseButton.XButton1:
					((MainViewModel)DataContext).MainPageViewModel.GoBack();
					break;
				case System.Windows.Input.MouseButton.XButton2:
					((MainViewModel)DataContext).MainPageViewModel.GoForward();
					break;
				default:
					return;
			}
		}

		private void ListBox_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{   //Focus the ListBox every time the mouse clicks so the go back command can trigger properly.
			//It has to be Preview event so it travels top to bottom.
			//If it's normal event, user clicking thumbnails will capture the event and this will not trigger.
			((ListBox)sender).Focus();
		}
	}
}
