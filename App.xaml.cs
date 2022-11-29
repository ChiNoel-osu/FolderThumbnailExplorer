using System.Windows;
using System.Globalization;

namespace FolderThumbnailExplorer
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public App()
		{
			CultureInfo.CurrentUICulture = new CultureInfo("zh-CN");
		}
	}
}
