using System.Windows;
using System.Windows.Controls;

namespace FolderThumbnailExplorer.View
{
	/// <summary>
	/// ImageControl.xaml 的交互逻辑
	/// </summary>
	public partial class ImageControl : UserControl
	{
		Window parentWnd;
		public ImageControl()
		{
			InitializeComponent();
		}

		private void UCLoaded(object sender, RoutedEventArgs e)
		{   //Gets the parent window.
			parentWnd = Window.GetWindow((DependencyObject)sender);
			ShowInExplorer.ToolTip = parentWnd.Tag; //The tag is the opened folderPath.
		}
		private void PreviousClick(object sender, RoutedEventArgs e)
		{
			if (ImgPosition.Value > 0)
				ImgPosition.Value--;
		}
		private void NextClick(object sender, RoutedEventArgs e)
		{
			ImgPosition.Value++;
		}
		#region Settings Popup events
		private void Label_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
		{
			SettingsPopup.IsOpen = true;
		}
		private void Label_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
		{
			if (!SettingsPopup.IsMouseOver)
				SettingsPopup.IsOpen = false;
		}
		private void SettingsPopup_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
		{
			SettingsPopup.IsOpen = false;
		}
		#endregion
	}
}
