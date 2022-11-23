using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FolderThumbnailExplorer.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;

namespace FolderThumbnailExplorer.ViewModel
{
	class NaturalStringComparer : IComparer<string>
	{
		public int Compare(string left, string right)
		{
			int max = new[] { left, right }
				.SelectMany(x => Regex.Matches(x, @"\d+").Cast<Match>().Select(y => (int?)y.Value.Length))
				.Max() ?? 0;    //If it's null, return 0;

			var leftPadded = Regex.Replace(left, @"\d+", m => m.Value.PadLeft(max, '0'));
			var rightPadded = Regex.Replace(right, @"\d+", m => m.Value.PadLeft(max, '0'));

			return string.Compare(leftPadded, rightPadded);
		}
	}
	public partial class PhotoViewerViewModel : ObservableObject
	{
		private readonly string path;

		[ObservableProperty]
		ushort _ListSelectedIndex;   //Shared between PhotoViewer & ImageControl, and they need to be in the same DataContext.
		[ObservableProperty]
		BitmapImage _BigImage = new BitmapImage();

		private ushort _imageCount;
		public ushort ImageCount
		{
			get
			{
				if (_imageCount == 1)   //1 pictures
					return 1;
				else if (_imageCount == 0)  //Sorting errors
					return 0;
				else
					return (ushort)(_imageCount - 1);   //Index issues so subtract by 1
			}
			private set
			{ _imageCount = value; }
		}   //Bind target for slider maximum.

		public CustomListItem SelectedImg
		{
			set
			{
				if(value is not null)
				{	//The image viewer still uses actual image as source rather than using stream reader.
					_BigImage = new BitmapImage();
					_BigImage.BeginInit();
					_BigImage.UriSource = new Uri(value.Path);
					_BigImage.EndInit();
					OnPropertyChanged(nameof(BigImage));
				}
			}
		}

		[RelayCommand]
		public static void OpenInExplorer(string path)
		{
			Process.Start("explorer.exe", path);
		}

		//Use this as the ListBoxItem binding target
		private ObservableCollection<CustomListItem> images = new ObservableCollection<CustomListItem>();
		public ObservableCollection<CustomListItem> Images
		{
			get
			{
				AddImgs(path);
				return images;
			}
			private set { images = value; }
		}
		private void AddImgs(string path)
		{
			IEnumerable<string> imgs;
			string[] allowedExt = { ".jpg", ".png", ".jpeg", ".gif" };
			try
			{   //Get image files
				imgs = Directory.EnumerateFiles(path, "*.*").Where(s => allowedExt.Any(s.ToLower().EndsWith));
			}
			catch (InvalidOperationException) { return; }
			//Left value for name (sorting target), right value for path (displaying).
			//Natural sorting.
			SortedDictionary<string, string> map = new SortedDictionary<string, string>(new NaturalStringComparer());
			foreach (string filePath in imgs)
			{   //Use of range operator
				string fileNameWOExt = filePath[(filePath.LastIndexOf('\\') + 1)..];        //001.jpg
				fileNameWOExt = fileNameWOExt[..fileNameWOExt.LastIndexOf('.')];            //001
				map.Add(fileNameWOExt, filePath);   //The dictionary will sort itself as valuesa are being added.
			}
			foreach (KeyValuePair<string, string> img in map)
			{
				CustomListItem imgItem = new CustomListItem();
				BitmapImage bitmapImage = new BitmapImage();
				bitmapImage.BeginInit();
				bitmapImage.UriSource = new Uri(img.Value);
				bitmapImage.DecodePixelWidth = 128; //TODO: make it configurable.
				bitmapImage.EndInit();
				imgItem.Path = img.Value;
				imgItem.Name = img.Value[(img.Value.LastIndexOf('\\') + 1)..];
				imgItem.Image = bitmapImage;
				images.Add(imgItem);
				_imageCount++;
			}
		}
		public PhotoViewerViewModel(string folderPath)
		{
			this.path = folderPath;
		}
	}
}
