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
		public static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();    //Global Logger initialize
		public App()
		{
			Logger.Info("The Application is starting.");
			#region Global exception handling
#if DEBUG
			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
			Dispatcher.UnhandledException += Dispatcher_UnhandledException;
			Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
			TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
#endif
			#endregion
			#region Load settings.
			try
			{
				Logger.Trace("Loading shit");
				CultureInfo.CurrentUICulture = new CultureInfo(FolderThumbnailExplorer.Properties.Settings.Default.Locale);
			}
			catch (System.Exception ex)
			{
				Logger.Warn("Something happened and language setting cannot be load.\nDefaulting to en-US.");
				Task.Run(() => MessageBox.Show($"{ex.Message}\nThe app will now use en-US as default language.", "Yo Fucked UP!", MessageBoxButton.OK, MessageBoxImage.Asterisk));
				CultureInfo.CurrentUICulture = new CultureInfo("en-US");
			}
			#endregion
		}
		#region Global exception handling
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
		#endregion
	}
}
