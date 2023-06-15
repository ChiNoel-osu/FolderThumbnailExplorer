using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace FolderThumbnailExplorer.Model
{
	public class WheelSpeedScrollViewer : ScrollViewer
	{
		public static readonly DependencyProperty SpeedFactorProperty =
			DependencyProperty.Register(nameof(SpeedFactor),
										typeof(double),
										typeof(WheelSpeedScrollViewer),
										new PropertyMetadata(2.5));
		// Using a DependencyProperty as the backing store for CanControlZoom.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty CanControlZoomProperty =
			DependencyProperty.Register(nameof(CanControlZoom), typeof(bool), typeof(WheelSpeedScrollViewer), new PropertyMetadata(false));
		// Using a DependencyProperty as the backing store for BindScaleTransform2ME.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty BindScaleTransform2MEProperty =
			DependencyProperty.Register(nameof(BindScaleTransform2ME), typeof(ScaleTransform), typeof(WheelSpeedScrollViewer));

		public double SpeedFactor
		{
			get { return (double)GetValue(SpeedFactorProperty); }
			set { SetValue(SpeedFactorProperty, value); }
		}
		public bool CanControlZoom
		{
			get { return (bool)GetValue(CanControlZoomProperty); }
			set { SetValue(CanControlZoomProperty, value); }
		}
		public ScaleTransform BindScaleTransform2ME
		{   //This is fucked up.
			get { return (ScaleTransform)GetValue(BindScaleTransform2MEProperty); }
			set { SetValue(BindScaleTransform2MEProperty, value); }
		}

		protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
		{
			if (CanControlZoom && BindScaleTransform2ME is not null && e.RightButton == MouseButtonState.Pressed)
			{
				if (e.Delta > 0 && BindScaleTransform2ME.ScaleX < 8)
					BindScaleTransform2ME.ScaleX = BindScaleTransform2ME.ScaleY += 0.3;
				else if (e.Delta < 0 && BindScaleTransform2ME.ScaleX > 1)
					BindScaleTransform2ME.ScaleX = BindScaleTransform2ME.ScaleY -= 0.3;
				else
					return;
			}
			else if (ScrollInfo is ScrollContentPresenter scp && ComputedVerticalScrollBarVisibility == Visibility.Visible)
			{
				scp.SetVerticalOffset(VerticalOffset - e.Delta * SpeedFactor);
			}
			e.Handled = true;
		}
	}
	//https://stackoverflow.com/questions/1639505/wpf-scrollviewer-scroll-amount
}
