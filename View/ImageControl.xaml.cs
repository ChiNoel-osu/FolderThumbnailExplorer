using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FolderThumbnailExplorer.View
{
	/// <summary>
	/// ImageControl.xaml 的交互逻辑
	/// </summary>
	public partial class ImageControl : UserControl
	{
		public ImageControl()
		{
			InitializeComponent();
		}

		private void PreviousClick(object sender, RoutedEventArgs e)
		{
			if (ImgPosition.Value > 0)
				ImgPosition.Value--;
		}
		private void NextClick(object sender, RoutedEventArgs e) => ImgPosition.Value++;

		#region Settings Popup events
		private void SettingsLabel_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
		{
			SettingsPopup.IsOpen = true;
		}
		private async void SettingsLabel_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
		{
			await Task.Run(() => Task.Delay(200));  //Add small delay incase of small gap between Label and Popup.
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
		{   //Use mouse wheel to control slideshow interval.
			if (short.TryParse(((TextBox)sender).Text, out short parsed))
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
		private void LoadThreshold_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
		{
			if (short.TryParse(((TextBox)sender).Text, out short parsed))
			{
				try
				{
					checked
					{
						parsed = (short)(e.Delta > 0 ? parsed + 1 : parsed - 1);
						if (parsed < 1) parsed = 1;
					}
					((TextBox)sender).Text = parsed.ToString();
				}
				catch (OverflowException) { }
			}
		}
		private void ScrollFactor_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
		{
			if (double.TryParse(((TextBox)sender).Text, out double parsed))
			{
				try
				{
					checked
					{ parsed = Math.Round(e.Delta > 0d ? parsed + 0.1d : parsed - 0.1d, 1); }
					((TextBox)sender).Text = parsed.ToString();
				}
				catch (OverflowException) { }
			}
		}
		private void AOTBtn_Changed(object sender, RoutedEventArgs e)
		{
			Window.GetWindow((System.Windows.Controls.Primitives.ToggleButton)sender).Topmost = e.RoutedEvent.Name switch
			{
				"Checked" => true,
				"Unchecked" => false,
				_ => throw new NotImplementedException()
			};
		}
		#endregion
	}
}
