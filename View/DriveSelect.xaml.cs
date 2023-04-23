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

		private void ListBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{   //Make it unresponsive to keyboard inputs.
			e.Handled = true;
		}
	}
}
