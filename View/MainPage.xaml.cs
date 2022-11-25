using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
		#region GOUP function
		private void UserControl_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Back)  //Go up
				GoUP();
		}
		private void GoUpBtnClicked(object sender, RoutedEventArgs e)
		{
			GoUP();
		}
		private void GoUP()
		{
			if (DirBox.Text != string.Empty)
				if (DirBox.Text.Remove(DirBox.Text.LastIndexOf('\\')).Length == 2)
					DirBox.Text = string.Format("{0}:\\", DirBox.Text[0]);
				else
					DirBox.Text = DirBox.Text.Remove(DirBox.Text.LastIndexOf('\\'));
		}
		#endregion
	}
}
