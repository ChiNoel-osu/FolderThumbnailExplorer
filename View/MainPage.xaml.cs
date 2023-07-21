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

		private void MainGrid_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
		{   //Focus the parent control (MainPage) every time the mouse is in the grid.
			((UserControl)(((Grid)sender).Parent)).Focus();
		}
	}
}
