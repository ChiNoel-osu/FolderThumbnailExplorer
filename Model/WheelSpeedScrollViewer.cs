using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FolderThumbnailExplorer.Model
{
	public class WheelSpeedScrollViewer : ScrollViewer
	{
		public static readonly DependencyProperty SpeedFactorProperty =
			DependencyProperty.Register(nameof(SpeedFactor),
										typeof(double),
										typeof(WheelSpeedScrollViewer),
										new PropertyMetadata(2.5));

		public double SpeedFactor
		{
			get { return (double)GetValue(SpeedFactorProperty); }
			set { SetValue(SpeedFactorProperty, value); }
		}

		protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
		{
			if (ScrollInfo is ScrollContentPresenter scp &&
				ComputedVerticalScrollBarVisibility == Visibility.Visible)
			{
				scp.SetVerticalOffset(VerticalOffset - e.Delta * SpeedFactor);
				e.Handled = true;
			}
		}
	}
	//https://stackoverflow.com/questions/1639505/wpf-scrollviewer-scroll-amount
}
