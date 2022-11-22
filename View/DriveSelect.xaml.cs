using System.Windows.Controls;

namespace FolderThumbnailExplorer.View
{
	/// <summary>
	/// DriveSelect.xaml 的交互逻辑
	/// </summary>
	public partial class DriveSelect : UserControl
	{
		public DriveSelect()
		{
			InitializeComponent();
		}

		private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count > 0)
				MainWindow.MainVM.MainPageViewModel.PATHtoShow = e.AddedItems[0] as string;
		}
	}
}
