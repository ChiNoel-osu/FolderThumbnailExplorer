using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;

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

		private void ListBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{   //Make it unresponsive to keyboard inputs.
			e.Handled = true;
		}

		private void Label_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			Process.Start(new ProcessStartInfo("cmd", @"/c start https://github.com/ChiNoel-osu/FolderThumbnailExplorer") { CreateNoWindow = true });
		}
	}
}
