using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.IO;

namespace FolderThumbnailExplorer.ViewModel
{
	public partial class DriveSelectViewModel : ObservableObject
	{
		[ObservableProperty]
		List<string> _Drives = new List<string>();

		[RelayCommand]
		public void RefreshDrives()
		{
			Drives.Clear();
			foreach (string drive in Directory.GetLogicalDrives())
				Drives.Add(drive);
		}

		public DriveSelectViewModel()
		{
			RefreshDrives();
		}
	}
}
