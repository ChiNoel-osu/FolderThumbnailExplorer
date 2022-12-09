using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FolderThumbnailExplorer.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
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
			//If the string is the same, return 1 meaning that left comes behind of right, similar to explorer.exe.
			return leftPadded == rightPadded ? 1 : string.Compare(leftPadded, rightPadded);
		}
	}
	public partial class PhotoViewerViewModel : ObservableObject, IDataErrorInfo
	{
		private readonly string path;
		private bool closing = false;

		[ObservableProperty]
		ushort _ListSelectedIndex;   //Shared between PhotoViewer & ImageControl, and they need to be in the same DataContext.
		[ObservableProperty]
		BitmapImage _BigImage;
		[ObservableProperty]
		BitmapImage _BigImage2; //For twin page viewing mode
		[ObservableProperty]
		bool _UseTwinPage = false;
		[ObservableProperty]    //This should be volatile as it might be used across different threads. But whatever.
		bool _SlideShow = false;
		[ObservableProperty]
		string _SlideInterval = "1000";
		[ObservableProperty]
		FlowDirection _FlowDirection = FlowDirection.LeftToRight;

		public bool DoubleTurn { get; set; } = false;

		short realSlideInterval = 1000;

		private ushort _ImageCount;
		public ushort ImageCount
		{
			get
			{
				if (_ImageCount == 1)   //1 pictures
					return 1;
				else if (_ImageCount == 0)  //Just in case it's broke.
					return 0;
				else
					return (ushort)(_ImageCount - 1);   //Index issues so subtract by 1
			}
			private set
			{ _ImageCount = value; }
		}   //Bind target for slider maximum.

		bool doubleTurnFlag;
		public CustomListItem SelectedImg   //Load the image
		{
			set
			{
				if (value is not null)
				{
					//BigImage2 will always be not null after first image is loaded.
					//The latter expression checks if user is requesting next image or not. If so, load the BigImage2 into BigImage, else reload.
					if (_BigImage2 is not null && value.Path == ((FileStream)_BigImage2.StreamSource).Name)
						_BigImage = _BigImage2;
					else
						using (FileStream stream = File.OpenRead(value.Path))
						{
							_BigImage = new BitmapImage();
							_BigImage.BeginInit();
							_BigImage.CacheOption = BitmapCacheOption.OnLoad;
							_BigImage.StreamSource = stream;
							_BigImage.EndInit();
						}
					if (_Images.Count > 1 && _Images.Count > _ListSelectedIndex + 1)
						using (FileStream stream = File.OpenRead(_Images[_ListSelectedIndex + 1].Path))
						{
							_BigImage2 = new BitmapImage();
							_BigImage2.BeginInit();
							_BigImage2.CacheOption = BitmapCacheOption.OnLoad;
							_BigImage2.StreamSource = stream;
							_BigImage2.EndInit();
						}
					else
						_BigImage2 = null;
					OnPropertyChanged(nameof(BigImage));
					OnPropertyChanged(nameof(BigImage2));
				}
			}
		}

		[RelayCommand]
		public static void OpenInExplorer(string path)
		{
			Process.Start("explorer.exe", path);
		}
		[RelayCommand]
		public void ToggleSlideShow() => SlideShow = !_SlideShow;
		[RelayCommand]
		public void ChangeFlowDirection(bool isChecked) =>
			FlowDirection = isChecked ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

		#region IDataErrorInfo members
		public string Error => throw new NotImplementedException();

		public string this[string data2Validate]
		{
			get
			{
				switch (data2Validate)
				{
					case nameof(SlideInterval):
						if (short.TryParse(SlideInterval, out realSlideInterval) && realSlideInterval != 0)
							return null;    //null for no error.
						else
							return "ShortParseFailed[DBG]";
					default:
						throw new NotImplementedException();
				}
			}
		}
		#endregion

		//Use this as the ListBoxItem binding target
		private ObservableCollection<CustomListItem> _Images = new ObservableCollection<CustomListItem>();
		public ObservableCollection<CustomListItem> Images
		{
			get
			{
				AddImgs(path);
				return _Images;
			}
			private set { _Images = value; }
		}

		private void AddImgs(string path)
		{
			Task.Run(() =>
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
				foreach (string filePath in imgs)   //The dictionary will sort itself when value's being added.
					map.Add(filePath[(filePath.LastIndexOf('\\') + 1)..], filePath);    //Use filename to compare.
				foreach (KeyValuePair<string, string> img in map)
				{
					if (closing) break; //Break out if the window is closing.
					CustomListItem imgItem = new CustomListItem();
					BitmapImage bitmapImage = new BitmapImage();
					using (FileStream stream = File.OpenRead(img.Value))
					{
						bitmapImage.BeginInit();
						bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
						bitmapImage.StreamSource = stream;
						bitmapImage.DecodePixelWidth = 128; //TODO: make it configurable.
						bitmapImage.EndInit();
					}
					bitmapImage.Freeze();
					imgItem.Path = img.Value;
					imgItem.Name = img.Value[(img.Value.LastIndexOf('\\') + 1)..];
					imgItem.Image = bitmapImage;
					Application.Current.Dispatcher.Invoke(() => _Images.Add(imgItem));
					_ImageCount++;
					OnPropertyChanged(nameof(ImageCount));  //Update ImageCount.
				}
			});
		}

		public PhotoViewerViewModel(string folderPath, object view)
		{
			BindingOperations.EnableCollectionSynchronization(_Images, new object());
			this.path = folderPath;
			((Window)view).Closed += PhotoViewerClosed;

			Task.Run(() =>
			{   //TODO: Not the best solution, idk how to pause a task properly.
				while (true)
				{   //This shit is CPU heavy
					if (closing) break; //Window is closed, release thread (Complete the Task).
					if (SlideShow)
					{
						Thread.Sleep(Math.Abs(realSlideInterval));
						if (realSlideInterval < 0 && _ListSelectedIndex > 0)
							ListSelectedIndex--;
						else if (realSlideInterval > 0 && (_ListSelectedIndex + 1 < _ImageCount))
							ListSelectedIndex++;
					}
					else
						Thread.Sleep(1000); //Relieve the CPU situation before I find a solution.
				}
			}); //Slideshow Task.
		}

		private void PhotoViewerClosed(object? sender, EventArgs e)
		{
			closing = true;
			GC.Collect();
		}
	}
}
