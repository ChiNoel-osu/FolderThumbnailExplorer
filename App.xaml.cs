using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
			MessageBox.Show(e.Exception.InnerException.Message);
			Logger.Fatal(e.Exception.InnerException.StackTrace);
		}

		public void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			throw new NotImplementedException();
		}
		#endregion
		#region Startup, Read command line arguments.
		Dictionary<string, string> cliArgs = new Dictionary<string, string>();
		private void Application_Startup(object sender, StartupEventArgs e)
		{
			string path = string.Empty;
			if (e.Args.Length > 0)  //Command line arguments provided.
			{
				try
				{
					for (byte index = 0; index < e.Args.Length; index += 2)
						cliArgs.Add(e.Args[index], e.Args[index + 1]);
				}
				catch (ArgumentException ex)
				{   //Duplicated option
					Logger.Error("Duplicated console option detected, the app will use first valid option.\n" + ex);
				}
				catch (IndexOutOfRangeException ex)
				{   //Incomplete argument
					Logger.Error("Incomplete console arguments ignored.\n" + ex);
				}
				if (cliArgs.Keys.Contains("--path"))
				{
					path = cliArgs["--path"];
					Logger.Info("The app will start with path: " + path);
				}
			}
			new MainWindow(path).Show();
		}
		#endregion
	}
}
