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
	}
}
