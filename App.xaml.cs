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
	}
}
