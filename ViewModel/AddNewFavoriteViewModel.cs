using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
			string favPath = Directory.GetCurrentDirectory() + "\\Favorites.txt";
			string[] strings = { "Name|" + Name, "Path|" + Path };
			if (!File.Exists(favPath))
				File.Create(favPath).Close();
			File.AppendAllLines(favPath, strings);
			window.Close();
		}

		public AddNewFavoriteViewModel(string path)
		{
			Path = path;
		}
	}
}
