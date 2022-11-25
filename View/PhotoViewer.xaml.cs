using FolderThumbnailExplorer.ViewModel;
using System.Windows;
using System.Windows.Input;

namespace FolderThumbnailExplorer.View
{
	/// <summary>
	/// PhotoViewer.xaml 的交互逻辑
	/// </summary>
	public partial class PhotoViewer : Window
	{
		private PhotoViewerViewModel PVVM;  //Each viewer gets its own viewmodel.
		private int timeStamp = 0;  //For user double clicking things.
		public PhotoViewer(string folderPath)
		{
			this.Tag = folderPath;  //Set tag so ImageControl can use it.
			PVVM = new PhotoViewerViewModel(folderPath, this);
			this.DataContext = PVVM;
			InitializeComponent();
		}
		#region Shown image zoom & pan
		private void BigImage_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			if (e.RightButton == MouseButtonState.Pressed)
			{
				if (e.Delta > 0 && BigImageScaleFactor.ScaleX < 7)
					BigImageScaleFactor.ScaleX = BigImageScaleFactor.ScaleY += 0.3;
				else if (e.Delta < 0 && BigImageScaleFactor.ScaleX > 1)
					BigImageScaleFactor.ScaleX = BigImageScaleFactor.ScaleY -= 0.3;
				else
					return;
			}
			else
			{
				if (e.Delta > 0 && PVVM.ListSelectedIndex > 0)
					PVVM.ListSelectedIndex--;
				else if (e.Delta < 0 && PVVM.ListSelectedIndex < PVVM.ImageCount)
					PVVM.ListSelectedIndex++;
				else
					return;
			}
		}
		private void BigImage_MouseMove(object sender, MouseEventArgs e)
		{
			double w = BigImage.ActualWidth;
			double h = BigImage.ActualHeight;
			if (e.RightButton == MouseButtonState.Pressed)
			{
				Point mousePosOnImg = e.GetPosition(BigImage);
				Point reletivePos = new Point(mousePosOnImg.X / w, mousePosOnImg.Y / h);
				BigImage.RenderTransformOrigin = reletivePos;
			}
			else
			{
				w = this.ActualWidth;
				h = this.ActualHeight;
				Point mousePosOnWnd = e.GetPosition(this);
				Point reletivePos = new Point(mousePosOnWnd.X / w, mousePosOnWnd.Y / h);
				reletivePos.X = BigImage.RenderTransformOrigin.X;   //Don't change X coord.
				double startYfrom = 0.4; double startYto = 0.6; //Set startpos reletive to window.
				if (reletivePos.Y > startYfrom && reletivePos.Y < startYto)
					reletivePos.Y = (reletivePos.Y - startYfrom) / (startYto - startYfrom);
				//X[Rescaled]=(X-X[min]/X[max]-X[min])	this rescales X to a range of [0,1]
				else
					reletivePos.Y = BigImage.RenderTransformOrigin.Y;   //Don't change Y coord.
				BigImage.RenderTransformOrigin = reletivePos;
			}
		}
		#endregion
		#region Window related actions
		private void CloseClick(object sender, RoutedEventArgs e)
		{
			Close();
		}
		private void MaxResClick(object sender, RoutedEventArgs e)
		{
			if (WindowState == WindowState.Normal)
				WindowState = WindowState.Maximized;
			else
				WindowState = WindowState.Normal;
		}
		private void MinClick(object sender, RoutedEventArgs e)
		{
			WindowState = WindowState.Minimized;
		}
		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.Escape:
					//I dont wanna cover all the states the user will figure it out.
					if (WindowState == WindowState.Maximized)
						WindowState = WindowState.Normal;
					else
						this.Close();
					break;
			}
		}
		private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			DragMove();
			if (e.Timestamp - timeStamp < 400)  //400ms for the user to double click
				if (WindowState == WindowState.Normal)
					WindowState = WindowState.Maximized;
				else
					WindowState = WindowState.Normal;
			timeStamp = e.Timestamp;
		}
		private void Rectangle_MouseLeave(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
				WindowState = WindowState.Normal;
		}
		#endregion
	}
}
