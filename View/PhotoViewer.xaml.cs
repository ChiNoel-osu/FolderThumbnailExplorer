using FolderThumbnailExplorer.Model;
using FolderThumbnailExplorer.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Shell;

namespace FolderThumbnailExplorer.View
{
	/// <summary>
	/// PhotoViewer.xaml 的交互逻辑
	/// </summary>
	public partial class PhotoViewer : Window
	{
		#region Maximized placement https://engy.us/blog/2020/01/01/implementing-a-custom-window-title-bar-in-wpf/
		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);
			((HwndSource)PresentationSource.FromVisual(this)).AddHook(HookProc);
		}

		public static IntPtr HookProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			if (msg == WM_GETMINMAXINFO)
			{
				// We need to tell the system what our size should be when maximized. Otherwise it will cover the whole screen,
				// including the task bar.
				MINMAXINFO mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

				// Adjust the maximized size and position to fit the work area of the correct monitor
				IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

				if (monitor != IntPtr.Zero)
				{
					MONITORINFO monitorInfo = new MONITORINFO();
					monitorInfo.cbSize = Marshal.SizeOf(typeof(MONITORINFO));
					GetMonitorInfo(monitor, ref monitorInfo);
					RECT rcWorkArea = monitorInfo.rcWork;
					RECT rcMonitorArea = monitorInfo.rcMonitor;
					mmi.ptMaxPosition.X = Math.Abs(rcWorkArea.Left - rcMonitorArea.Left);
					mmi.ptMaxPosition.Y = Math.Abs(rcWorkArea.Top - rcMonitorArea.Top);
					mmi.ptMaxSize.X = Math.Abs(rcWorkArea.Right - rcWorkArea.Left);
					mmi.ptMaxSize.Y = Math.Abs(rcWorkArea.Bottom - rcWorkArea.Top);
				}

				Marshal.StructureToPtr(mmi, lParam, true);
			}

			return IntPtr.Zero;
		}

		private const int WM_GETMINMAXINFO = 0x0024;

		private const uint MONITOR_DEFAULTTONEAREST = 0x00000002;

		[DllImport("user32.dll")]
		private static extern IntPtr MonitorFromWindow(IntPtr handle, uint flags);

		[DllImport("user32.dll")]
		private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

		[Serializable]
		[StructLayout(LayoutKind.Sequential)]
		public struct RECT
		{
			public int Left;
			public int Top;
			public int Right;
			public int Bottom;

			public RECT(int left, int top, int right, int bottom)
			{
				this.Left = left;
				this.Top = top;
				this.Right = right;
				this.Bottom = bottom;
			}
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct MONITORINFO
		{
			public int cbSize;
			public RECT rcMonitor;
			public RECT rcWork;
			public uint dwFlags;
		}

		[Serializable]
		[StructLayout(LayoutKind.Sequential)]
		public struct POINT
		{
			public int X;
			public int Y;

			public POINT(int x, int y)
			{
				this.X = x;
				this.Y = y;
			}
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct MINMAXINFO
		{
			public POINT ptReserved;
			public POINT ptMaxSize;
			public POINT ptMaxPosition;
			public POINT ptMinTrackSize;
			public POINT ptMaxTrackSize;
		}
		#endregion

		private PhotoViewerViewModel PVVM;  // Each viewer gets its own viewmodel
		public PhotoViewer(string folderPath)
		{
			this.Tag = folderPath;  // Set tag so ImageControl can use it.
			PVVM = new PhotoViewerViewModel(folderPath, this);
			this.DataContext = PVVM;
			WindowChrome.SetWindowChrome(this, normalWndChrome);
			InitializeComponent();
		}
		#region Shown image zoom & pan
		private void BigImage_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			if (e.RightButton == MouseButtonState.Pressed)
			{   //Zoom image
				if (e.Delta > 0 && BigImageScaleFactor.ScaleX < 8)
					BigImageScaleFactor.ScaleX = BigImageScaleFactor.ScaleY += 0.3;
				else if (e.Delta < 0 && BigImageScaleFactor.ScaleX > 1)
					BigImageScaleFactor.ScaleX = BigImageScaleFactor.ScaleY -= 0.3;
				else
					return;
			}
			else
			{   //Next image
				if (e.Delta > 0 && PVVM.ListSelectedIndex > 0)
				{
					if (PVVM.DoubleTurn) PVVM.ListSelectedIndex--;
					try { checked { PVVM.ListSelectedIndex--; } }
					catch (System.OverflowException) { }
				}
				else if (e.Delta < 0 && PVVM.ListSelectedIndex < PVVM.ImageCount)
				{
					if (PVVM.DoubleTurn) PVVM.ListSelectedIndex++;
					PVVM.ListSelectedIndex++;
				}
				else
					return;
			}
		}
		private void BigImage_MouseMove(object sender, MouseEventArgs e)
		{
			Image img = (Image)sender;
			double w = img.ActualWidth;
			double h = img.ActualHeight;
			if (e.RightButton == MouseButtonState.Pressed)
			{
				Point mousePosOnImg = e.GetPosition(img);
				Point relativePos = new Point(mousePosOnImg.X / w, mousePosOnImg.Y / h);
				img.RenderTransformOrigin = relativePos;
			}
			else
			{
				w = this.ActualWidth;
				h = this.ActualHeight;
				Point mousePosOnWnd = e.GetPosition(this);
				Point relativePos = new Point(mousePosOnWnd.X / w, mousePosOnWnd.Y / h);
				relativePos.X = img.RenderTransformOrigin.X;   //Don't change X cord.
				double startYfrom = 0.4; double startYto = 0.6; //Set startpos relative to window.
				if (relativePos.Y > startYfrom && relativePos.Y < startYto)
					relativePos.Y = (relativePos.Y - startYfrom) / (startYto - startYfrom);
				//X[Rescaled]=(X-X[min]/X[max]-X[min])	this rescales X to a range of [0,1]
				else
					relativePos.Y = img.RenderTransformOrigin.Y;   //Don't change Y cord.
				img.RenderTransformOrigin = relativePos;
			}
		}
		private void WheelSpeedScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			if (((ScrollViewer)sender).VerticalOffset >= ((ScrollViewer)sender).ExtentHeight - ((ScrollViewer)sender).ViewportHeight && e.VerticalChange != 0)
			{   // The ScrollViewer has reached the bottom													This prevents it to continue load when first checked.
				ImageList.SelectedIndex = ImageList.Items.Count - 1;    //Make the list select the last item which will trigger the continue method.
			}
		}
		#endregion
		#region Window related actions
		double lastLeft = 0;
		double lastTop = 0;
		double lastWidth = 0;
		double lastHeight = 0;
		private bool isFullScreen = false;
		// Window chromes for different window state
		readonly WindowChrome normalWndChrome = new WindowChrome { CaptionHeight = 26, ResizeBorderThickness = new Thickness(8) };
		readonly WindowChrome maxWndChrome = new WindowChrome { CaptionHeight = 26, ResizeBorderThickness = new Thickness(0) };
		readonly WindowChrome fullscrnWndChrome = new WindowChrome { CaptionHeight = 0, ResizeBorderThickness = new Thickness(0) };
		private void CloseClick(object sender, RoutedEventArgs e)
		{
			Close();
		}
		private void MaxResClick(object sender, RoutedEventArgs e)
		{
			if (isFullScreen)
			{
				Left = lastLeft;
				Top = lastTop;
				Width = lastWidth;
				Height = lastHeight;
				isFullScreen = false;
				WindowChrome.SetWindowChrome(this, normalWndChrome);
			}
			else if (WindowState == WindowState.Normal)
			{
				lastLeft = Left;
				lastTop = Top;
				lastWidth = Width;
				lastHeight = Height;
				WindowState = WindowState.Maximized;
			}
			else
			{
				WindowState = WindowState.Normal;
			}
		}
		private void MinClick(object sender, RoutedEventArgs e)
		{
			WindowState = WindowState.Minimized;
		}
		private void FullscreenClick(object sender, RoutedEventArgs e)
		{
			// Cast Width and Height because they don't always match up.
			if (Left == 0 && Top == 0 && (int)Width == (int)SystemParameters.PrimaryScreenWidth && (int)Height == (int)SystemParameters.PrimaryScreenHeight)
			{   // Exit Fullscreen
				isFullScreen = false;
				if (WindowState == WindowState.Maximized)
					WindowState = WindowState.Normal;
				Left = lastLeft;
				Top = lastTop;
				Width = lastWidth;
				Height = lastHeight;
				WindowChrome.SetWindowChrome(this, normalWndChrome);
			}
			else
			{   // Enter Fullscreen
				isFullScreen = true;
				lastLeft = Left;
				lastTop = Top;
				lastWidth = Width;
				lastHeight = Height;
				if (WindowState == WindowState.Maximized)
				{
					lastLeft = 0;
					lastTop = 0;
					WindowState = WindowState.Normal;
				}
				Left = 0;
				Top = 0;
				Width = System.Windows.SystemParameters.PrimaryScreenWidth;
				Height = System.Windows.SystemParameters.PrimaryScreenHeight;
				WindowChrome.SetWindowChrome(this, fullscrnWndChrome);
			}
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
				case Key.F:
					FullscreenClick(sender, e);
					break;
			}
		}
		#endregion

		private void ImageBox_SelectionChanged(object sender, SelectionChangedEventArgs e) =>
			((ListBox)sender).ScrollIntoView(((ListBox)sender).SelectedItem);

		private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			Point mousePosition = e.GetPosition((UIElement)sender);
			double halfWidth = ((FrameworkElement)sender).ActualWidth / 2;
			if (mousePosition.X < halfWidth && ImageList.SelectedIndex > 0) // Left half of the Grid was clicked
				ImageList.SelectedIndex--;
			else if (mousePosition.X > halfWidth && ImageList.SelectedIndex < ((ObservableCollection<CustomListItem>)ImageList.ItemsSource).Count)  // Right half of the Grid was clicked
				ImageList.SelectedIndex++;
		}

		private void Window_StateChanged(object sender, EventArgs e)
		{	// Handle WindowChrome change, including when user is not clicking buttons.
			switch(((Window)sender).WindowState)
			{
				case WindowState.Normal:
					WindowChrome.SetWindowChrome(this, normalWndChrome);
					break;
				case WindowState.Maximized:
					WindowChrome.SetWindowChrome(this, maxWndChrome);
					break;
			}
		}
	}
}
