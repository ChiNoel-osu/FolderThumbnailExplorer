using CommunityToolkit.Mvvm.ComponentModel;
using FolderThumbnailExplorer.View;
using System;
using System.Windows.Controls;

namespace FolderThumbnailExplorer.ViewModel
{
	public partial class NavSideBarViewModel : ObservableObject
	{
		int _SelectedIndex = 0;
		public int SelectedIndex
		{
			get => _SelectedIndex;
			set
			{
				switch (_SelectedIndex = value)
				{
					case 0:
						RightView = mainPage;
						MidCtrl = driveSelect;
						break;
					case 1:
						RightView = favoriteFolders;
						MidCtrl = null;	//TODO: new navigation
						break;
					default:
						throw new NotImplementedException();
				}
			}
		}

		#region Navigate
		[ObservableProperty]
		UserControl? _MidCtrl = driveSelect;
		[ObservableProperty]
		UserControl _RightView = mainPage;

		static readonly DriveSelect driveSelect = new DriveSelect();
		static readonly MainPage mainPage = new MainPage();
		static readonly FavoriteFolders favoriteFolders = new FavoriteFolders();
		#endregion
	}
}