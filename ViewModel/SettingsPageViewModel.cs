using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Globalization;
using System.Windows;

namespace FolderThumbnailExplorer.ViewModel
{
	public partial class SettingsPageViewModel : ObservableObject
	{
		[ObservableProperty]
		int _LanguageIndex = -1;
		[ObservableProperty]
		bool _IsLangRestartVisible = false;
		[ObservableProperty]
		string _LanguageRestartTT = "";
		[ObservableProperty]
		bool _UseCache = false;
		[ObservableProperty]
		byte _CacheQuality;

		bool langInit = true;
		bool usecacheInit = true;
		partial void OnLanguageIndexChanged(int value)
		{
			if (langInit)
			{   //Ignore first trigger.
				langInit = false;
				return;
			}
			switch (value)
			{
				case -1:
					return;
				case 0:
					Properties.Settings.Default.Locale = "en-US";
					break;
				case 1:
					Properties.Settings.Default.Locale = "zh-CN";
					break;
				default:
					App.Logger.Warn("Language CbBox selected index error. No corresponding language found.");
					return;
			}
			Properties.Settings.Default.Save();
			//Display the user-selected-language tooltip.
			LanguageRestartTT = Localization.Loc.ResourceManager.GetString("SettingsLangRestartTT", new CultureInfo(Properties.Settings.Default.Locale));
			IsLangRestartVisible = true;
		}
		partial void OnUseCacheChanged(bool value)
		{
			if (usecacheInit)
			{   //Ignore first trigger.
				usecacheInit = false;
				return;
			}
			else
			{
				Properties.Settings.Default.TE_UseCache = value;
				Properties.Settings.Default.Save();
			}
		}

		[RelayCommand]
		public void SaveCacheQuality()
		{
			Properties.Settings.Default.TE_CacheQuality = _CacheQuality;
			Properties.Settings.Default.Save();
		}
		[RelayCommand]
		public void SaveWndPosSize(System.Windows.Controls.Button btn)
		{
			Window wnd = Window.GetWindow(btn);
			Properties.Settings.Default.WindowLeft = wnd.Left;
			Properties.Settings.Default.WindowTop = wnd.Top;
			Properties.Settings.Default.WindowWidth = wnd.Width;
			Properties.Settings.Default.WindowHeight = wnd.Height;
			Properties.Settings.Default.Save();
		}
		[RelayCommand]
		public void ResetWndPosSize()
		{
			Properties.Settings.Default.WindowLeft = 200;
			Properties.Settings.Default.WindowTop = 200;
			Properties.Settings.Default.WindowWidth = 800;
			Properties.Settings.Default.WindowHeight = 600;
			Properties.Settings.Default.Save();
		}

		public SettingsPageViewModel()
		{
			switch (Properties.Settings.Default.Locale)
			{
				case "en-US":
					LanguageIndex = 0;
					break;
				case "zh-CN":
					LanguageIndex = 1;
					break;
				default:
					App.Logger.Warn("Locale parse in settings page failed. Defaulting to en-US[0]");
					LanguageIndex = 0;
					break;
			}
			UseCache = Properties.Settings.Default.TE_UseCache;
			CacheQuality = Properties.Settings.Default.TE_CacheQuality;
			usecacheInit = false;
		}
	}
}
