using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;

namespace FolderThumbnailExplorer
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public App()
		{
#if DEBUG
			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
			Dispatcher.UnhandledException += Dispatcher_UnhandledException;
			Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
			TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
#endif
			#region Load settings.
			try
			{
				CultureInfo.CurrentUICulture = new CultureInfo(FolderThumbnailExplorer.Properties.Settings.Default.Locale);
			}
			catch (System.Exception ex)
			{
				Task.Run(() => MessageBox.Show($"{ex.Message}\nThe app will now use en-US as default language.", "Yo Fucked UP!", MessageBoxButton.OK, MessageBoxImage.Asterisk));
				CultureInfo.CurrentUICulture = new CultureInfo("en-US");
			}
			#endregion
		}

#if DEBUG
		private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
		{
			throw new NotImplementedException();
		}

		private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
		{
			throw new NotImplementedException();
		}

		private void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
		{
			throw new NotImplementedException();
		}

		public void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			throw new NotImplementedException();
		}
#endif
	}
}
