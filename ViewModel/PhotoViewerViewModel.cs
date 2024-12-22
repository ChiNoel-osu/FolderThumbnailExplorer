using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FolderThumbnailExplorer.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace FolderThumbnailExplorer.ViewModel
{
	public partial class PhotoViewerViewModel : ObservableObject
	{
		bool closing = false;

		[ObservableProperty]
		ushort _ListSelectedIndex;   //Shared between PhotoViewer & ImageControl, and they need to be in the same DataContext.
		[ObservableProperty]
		BitmapImage _BigImage;
		[ObservableProperty]
		BitmapImage _BigImage2; //For twin page viewing mode
		[ObservableProperty]
		bool _MainView = true;  //The visibility of the big photo.
		[ObservableProperty]
		bool _UseTwinPage = false;
		[ObservableProperty]    //This should be volatile as it might be used across different threads. But whatever.
		bool _SlideShow = false;
		[ObservableProperty]
		short _SlideInterval = 1000;
		[ObservableProperty]
		FlowDirection _FlowDir = FlowDirection.LeftToRight;
		[ObservableProperty]
		bool _ScrollView = false;
		[ObservableProperty]
		ObservableCollection<BitmapImage> _ScrollImg = new ObservableCollection<BitmapImage>();
		[ObservableProperty]
		sbyte _PosFlag = 0; //Save window position animation
		[ObservableProperty]
		string _Status = string.Empty;
		[ObservableProperty]
		string _CurrentImageDir = string.Empty;
		[ObservableProperty]
		private ObservableCollection<CustomListItem> _Images = new ObservableCollection<CustomListItem>();  //ListBoxItem binding target

		public bool DoubleTurn { get; set; } = false;
		public ushort LoadThreshold
		{
			get => Properties.Settings.Default.PV_LoadThreshold;
			set
			{
				Properties.Settings.Default.PV_LoadThreshold = value;
				Properties.Settings.Default.Save();
			}
		}
		public double ScrollFactor
		{
			get => Properties.Settings.Default.PV_ScrollSpeedFactor;
			set
			{
				Properties.Settings.Default.PV_ScrollSpeedFactor = value;
				Properties.Settings.Default.Save();
				OnPropertyChanged(nameof(ScrollFactor));
			}
		}

		public ushort ImageCount
		{
			get
			{
				if (loadedCount == 1)   //1 pictures
					return 1;
				else if (loadedCount == 0)  //Just in case it's broke.
					return 0;
				else
				{
					return (ushort)(loadedCount - 1);   //Index issues so subtract by 1
				}
			}
			private set
			{ loadedCount = value; }
		}   //Binding target for slider maximum.

		public CustomListItem SelectedImg   //Load the image
		{
			set
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
						_BigImage.CreateOptions = BitmapCreateOptions.PreservePixelFormat | BitmapCreateOptions.IgnoreColorProfile;
						_BigImage.CacheOption = BitmapCacheOption.OnLoad;
						_BigImage.StreamSource = stream;
						_BigImage.EndInit();
					}
				if (_Images.Count > 1 && _Images.Count > _ListSelectedIndex + 1)    //More than one image exists
					using (FileStream stream = File.OpenRead(_Images[_ListSelectedIndex + 1].Path))
					{
						_BigImage2 = new BitmapImage();
						_BigImage2.BeginInit();
						_BigImage2.CreateOptions = BitmapCreateOptions.PreservePixelFormat | BitmapCreateOptions.IgnoreColorProfile;
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

		ushort scrLoadedCount = 0; //Total loaded image in ItemsControl (ScrollView)

		[RelayCommand]
		public void OpenInExplorer()
		{
			Process.Start("explorer.exe", CurrentImageDir);
			App.Logger.Info($"User requested {System.Reflection.MethodBase.GetCurrentMethod().Name} and is completed.");
		}
		[RelayCommand]
		public void ChangeFlowDirection(bool isChecked)
		{
			FlowDir = isChecked ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
			App.Logger.Info($"User requested {System.Reflection.MethodBase.GetCurrentMethod().Name} and is completed.");
		}
		[RelayCommand]
		public void LoadScrollView()
		{
			MainView = !ScrollView;
			if (ScrollView)
				Task.Run(() =>
				{
					for (ushort i = 0; i < loadedCount; i++)
					{
						if (i < scrLoadedCount) continue;  //Skip loaded image.
						if (closing) break;
						BitmapImage scrollImg = new BitmapImage();
						using (FileStream fs = File.OpenRead(((FileStream)_Images[i].Image.StreamSource).Name))
						{
							scrollImg.BeginInit();
							scrollImg.CacheOption = BitmapCacheOption.OnLoad;
							scrollImg.StreamSource = fs;
							scrollImg.EndInit();
						}
						scrollImg.Freeze();
						Application.Current.Dispatcher.Invoke(() => ScrollImg.Add(scrollImg));
						scrLoadedCount++;
					}
				});
			App.Logger.Info($"User requested {System.Reflection.MethodBase.GetCurrentMethod().Name} and is completed.");
		}
		[RelayCommand]
		public void SavePos(Window wnd)
		{
			PosFlag = 0;
			Properties.Settings.Default.PV_Width = wnd.Width;
			Properties.Settings.Default.PV_Height = wnd.Height;
			Properties.Settings.Default.PV_Left = wnd.Left;
			Properties.Settings.Default.PV_Top = wnd.Top;
			Properties.Settings.Default.Save();
			PosFlag = 1;
			App.Logger.Info($"User requested {System.Reflection.MethodBase.GetCurrentMethod().Name} and is completed.");
		}
		[RelayCommand]
		public void DefaultPos()
		{
			PosFlag = 0;
			Properties.Settings.Default.PV_Width = 480;
			Properties.Settings.Default.PV_Height = 600;
			Properties.Settings.Default.PV_Left = 100;
			Properties.Settings.Default.PV_Top = 100;
			Properties.Settings.Default.Save();
			PosFlag = -1;
			App.Logger.Info($"User requested {System.Reflection.MethodBase.GetCurrentMethod().Name} and is completed.");
		}
		[RelayCommand]
		public void AddFav2Group()
		{
			new View.AddFav2Group(CurrentImageDir).ShowDialog();
		}
		[RelayCommand]
		public async void Copy2Clipboard()
		{
			DataObject dataObject = new DataObject();
			dataObject.SetData(DataFormats.Bitmap, _BigImage);
			dataObject.SetData(DataFormats.FileDrop, new string[] { ((FileStream)_BigImage.StreamSource).Name });
			Clipboard.SetDataObject(dataObject, true);
			Status = Localization.Loc.Copied2Clipboard;
			await Task.Run(() => Thread.Sleep(1000));
			Status = string.Empty;
		}

		SortedDictionary<string, string> imageMap;
		ushort loadedCount = 0; //Total loaded image in ListBox.
		private bool AddImgs(string path)
		{
			App.Logger.Info("The PhotoViewer has started to load images.");
			IEnumerable<string> imgs;
			string[] allowedExt = [".jpg", ".png", ".jpeg", ".gif", ".jfif"];
			try
			{   //Get image files
				imgs = Directory.EnumerateFiles(path, "*.*").Where(s => allowedExt.Any(s.ToLower().EndsWith));
			}
			catch (Exception e)
			{
				switch (e)
				{
					case InvalidOperationException:
						App.Logger.Error($"PhotoViewer got InvalidOperationException while attempting to add image.");
						break;
					case DirectoryNotFoundException:
						App.Logger.Error($"PhotoViewer got DirectoryNotFoundException while attempting to add image. The path probably does not exist anymore.");
						break;
					default:
						break;
				}
				return false;
			}
			//Left value for name (sorting target), right value for path (displaying).
			//Natural sorting.
			imageMap = new SortedDictionary<string, string>(new NaturalStringComparer());
			foreach (string filePath in imgs)   //The dictionary will sort itself when value's being added.
												//Use filename to compare.
				imageMap.Add(Path.GetFileName(filePath), filePath);
			//Add image to collection.
			foreach (KeyValuePair<string, string> img in imageMap)
			{
				if (closing) break; //Break out if the window is closing.
				LoadImagePreview(img);
				loadedCount++;
				OnPropertyChanged(nameof(ImageCount));
				if (loadedCount % Properties.Settings.Default.PV_LoadThreshold == 0) break;  //Break after reaching image limit.
			}
			App.Logger.Info($"The PhotoViewer has loaded {loadedCount} images in total.");
			return true;
		}
		List<ushort> loadedIndex = new List<ushort>();
		partial void OnListSelectedIndexChanged(ushort value)
		{   //Continue add.
			if ((value + 1) % Properties.Settings.Default.PV_LoadThreshold == 0 && !loadedIndex.Contains(value))
			{   //When user selected last image in list, load next batch of image.
				loadedIndex.Add(value);
				Task.Run(() =>
				{
					App.Logger.Info("The PhotoViewer is continuing to load images.");
					ushort current = 0;
					foreach (KeyValuePair<string, string> img in imageMap)
					{
						if (current++ < loadedCount) continue;  //Skip loaded image.
						if (closing) break; //Break out if the window is closing.
						LoadImagePreview(img);
						loadedCount++;
						OnPropertyChanged(nameof(ImageCount));
						if (loadedCount % Properties.Settings.Default.PV_LoadThreshold == 0) break;  //Break after reaching image limit.
					}
					if (ScrollView) LoadScrollView();
					App.Logger.Info($"The PhotoViewer has loaded {loadedCount} images in total.");
				});
			}
		}
		private void LoadImagePreview(KeyValuePair<string, string> img)
		{
			CustomListItem imgItem = new CustomListItem();
			BitmapImage bitmapImage = new BitmapImage();
			using (FileStream stream = File.OpenRead(img.Value))
			{
				bitmapImage.BeginInit();
				bitmapImage.CreateOptions = BitmapCreateOptions.PreservePixelFormat | BitmapCreateOptions.IgnoreColorProfile;
				bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
				bitmapImage.StreamSource = stream;
				bitmapImage.DecodePixelWidth = 128; //TODO: make it configurable.
				bitmapImage.EndInit();
			}
			bitmapImage.Freeze();
			imgItem.Path = img.Value;
			imgItem.Name = img.Value[(img.Value.LastIndexOf(Path.DirectorySeparatorChar) + 1)..];
			imgItem.Image = bitmapImage;
			_Images.Add(imgItem);
		}
		public void ResetReload(string folderPath)
		{
			CurrentImageDir = folderPath;
			ListSelectedIndex = 1;  //This somehow fixes the problem of the list not being selected after reset.
			imageMap.Clear();
			Images.Clear();
			ScrollImg.Clear();
			loadedIndex.Clear();
			ListSelectedIndex = loadedCount = scrLoadedCount = 0;
			BigImage = BigImage2 = null;
			Task.Run(() => AddImgs(CurrentImageDir = folderPath));
		}

		public PhotoViewerViewModel(string folderPath, object view)
		{
			BindingOperations.EnableCollectionSynchronization(_Images, new object());
			((Window)view).Closed += PhotoViewerClosed;

			Task.Run(() => AddImgs(CurrentImageDir = folderPath));  //Starts a task that adds images to the collection.
			Task.Run(() =>
			{   //TODO: Not the best solution, idk how to pause a task properly.
				while (true)
				{   //This shit is CPU heavy
					if (closing) break; //Window is closed, release thread (Complete the Task).
					if (SlideShow)
					{
						Thread.Sleep(Math.Abs(SlideInterval));
						if (SlideInterval < 0 && _ListSelectedIndex > 0)
							ListSelectedIndex--;
						else if (SlideInterval > 0 && (_ListSelectedIndex + 1 < loadedCount))
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
			MainPageViewModel.wnds.Remove((Window)sender);
			App.Logger.Info("PhotoViewer has closed.");
		}
	}
}
