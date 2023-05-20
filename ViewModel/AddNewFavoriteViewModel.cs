using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FolderThumbnailExplorer.Model;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace FolderThumbnailExplorer.ViewModel
{
	public partial class AddNewFavoriteViewModel : ObservableObject
	{
		public string Path { get; set; } = string.Empty;
		[ObservableProperty]
		[NotifyCanExecuteChangedFor(nameof(SaveFav2GroupCommand))]
		string _Name = string.Empty;

		public List<TaggedString> GroupList { get; } = new List<TaggedString>();
		[ObservableProperty]
		[NotifyCanExecuteChangedFor(nameof(SaveFav2GroupCommand))]
		TaggedString _SelectedGroup;

		[RelayCommand]
		public void SaveNewFav(Window window)
		{
			App.Logger.Info($"Adding a new favorite folder {Path} with name {Name}.");
			string favPath = Directory.GetCurrentDirectory() + System.IO.Path.DirectorySeparatorChar + "Favorites.txt";
			string[] strings = { "Name|" + Name, "Path|" + Path };
			if (!File.Exists(favPath))
			{
				App.Logger.Info("Favorites.txt doesn't exist! Creating a new one.");
				File.Create(favPath).Close();
			}
			File.AppendAllLines(favPath, strings);
			window.Close();
		}

		bool CanSaveFav2Group()
		{
			return !string.IsNullOrWhiteSpace(Name) || SelectedGroup.value is not null;
		}

		[RelayCommand(CanExecute = nameof(CanSaveFav2Group))]
		public void SaveFav2Group(Window window)
		{
			if (string.IsNullOrWhiteSpace(Name))
			{
				App.Logger.Info($"Adding a new favorite {Path} to existing group {SelectedGroup.value}.");
				if (File.Exists(SelectedGroup.tag))
				{
					using StreamWriter sw = File.AppendText(SelectedGroup.tag);
					sw.WriteLine(Path);
				}
			}
			else
			{
				App.Logger.Info($"Adding a new favorite {Path} to new group {Name}.");
				if (File.Exists("FavoriteGroups" + System.IO.Path.DirectorySeparatorChar + Name + ".txt") && MessageBox.Show(Localization.Loc.ReplaceGroup, Localization.Loc.ReplaceGroupCaption, MessageBoxButton.YesNo) != MessageBoxResult.Yes)
					return;
				File.WriteAllText("FavoriteGroups" + System.IO.Path.DirectorySeparatorChar + Name + ".txt", Path + '\n');
			}
			window.Close();
		}

		public AddNewFavoriteViewModel(string path)
		{
			this.Path = path;
			foreach (FileInfo group in Directory.CreateDirectory("FavoriteGroups").EnumerateFiles())
			{
				if (group.Extension != ".txt") continue;

				GroupList.Add(new TaggedString { value = System.IO.Path.GetFileNameWithoutExtension(group.Name), tag = group.FullName });
			}
		}
	}
}
