using CommunityToolkit.Mvvm.Input;
using System.IO;
using System.Windows;

namespace FolderThumbnailExplorer.ViewModel
{
	public partial class AddNewFavoriteViewModel
	{
		public string Path { get; set; } = string.Empty;
		public string Name { get; set; } = string.Empty;

		[RelayCommand]
		public void SaveNewFav(Window window)
		{
			App.Logger.Info($"Adding a new favorite folder {Path} with name {Name}.");
			string favPath = Directory.GetCurrentDirectory() + "\\Favorites.txt";
			string[] strings = { "Name|" + Name, "Path|" + Path };
			if (!File.Exists(favPath))
			{
				App.Logger.Info("Favorites.txt doesn't exist! Creating a new one.");
				File.Create(favPath).Close();
			}
			File.AppendAllLines(favPath, strings);
			window.Close();
		}

		public AddNewFavoriteViewModel(string path)
		{
			Path = path;
		}
	}
}
