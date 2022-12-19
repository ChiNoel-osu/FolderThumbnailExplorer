using System;
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
		private void NextClick(object sender, RoutedEventArgs e) => ImgPosition.Value++;

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
			if (!SettingsPopup.IsKeyboardFocusWithin)
				SettingsPopup.IsOpen = false;
		}
		#endregion

		#region Settings related UI Code
		private void SlideInterval_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
		{
			if (short.TryParse((string?)((TextBox)sender).Text, out short parsed))
			{
				try
				{
					checked
					{ parsed = (short)(e.Delta > 0 ? parsed + 400 : parsed - 400); }
					((TextBox)sender).Text = parsed.ToString();
				}
				catch (OverflowException) { }
			}
		}
		private void AOTBtn_Changed(object sender, RoutedEventArgs e)
		{
			parentWnd.Topmost = e.RoutedEvent.Name switch
			{
				"Checked" => true,
				"Unchecked" => false,
				_ => throw new NotImplementedException()
			};
		}
		#endregion
    }
}
