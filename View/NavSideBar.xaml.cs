using System;
using System.Diagnostics;
using System.Windows.Controls;

namespace FolderThumbnailExplorer.View
{
	/// <summary>
	/// NavSideBar.xaml 的交互逻辑
	/// </summary>
	public partial class NavSideBar : UserControl
	{
		public NavSideBar()
		{
			InitializeComponent();
		}

		private void Label_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			Process.Start(new ProcessStartInfo("cmd", @"/c start https://github.com/ChiNoel-osu/FolderThumbnailExplorer") { CreateNoWindow = true });
		}

		private void NavBar_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (CC is not null)
				CC.Content = ((ListBox)sender).SelectedIndex switch
				{
					-1 => null,
					0 => MainWindow.MainVM.DriveSelectViewModel,
					1 => MainWindow.MainVM.DriveSelectViewModel,
					_ => throw new NotImplementedException()
				};
		}
	}
}
