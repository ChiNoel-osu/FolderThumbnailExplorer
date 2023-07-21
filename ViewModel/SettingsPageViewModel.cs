﻿using CommunityToolkit.Mvvm.ComponentModel;
using System.Globalization;

namespace FolderThumbnailExplorer.ViewModel
{
	public partial class SettingsPageViewModel : ObservableObject
	{
		[ObservableProperty]
		int _LanguageIndex = -1;
		[ObservableProperty]
		bool _IsLangRestartVisible = false;
		[ObservableProperty]
		string _LanguageRestartTT;

		bool init = true;
		partial void OnLanguageIndexChanged(int value)
		{
			if (init)
			{   //Ignore first trigger.
				init = false;
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

		}
	}
}