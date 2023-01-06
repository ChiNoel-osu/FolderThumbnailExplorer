using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FolderThumbnailExplorer.Model;
using FolderThumbnailExplorer.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace FolderThumbnailExplorer.ViewModel
{
	public partial class MainPageViewModel : ObservableObject, IDataErrorInfo
	{
		public CancellationTokenSource cts = new CancellationTokenSource();
		public Task addItemTask;
		public static List<Window> wnds = new List<Window>();

		#region IDataErrorInfo members
		public string Error => throw new NotImplementedException();
		public string this[string data2Validate]
		{
			get
			{
				switch (data2Validate)
				{
					case nameof(PATHtoShow):
						if (isLastPathValid || string.IsNullOrEmpty(_PATHtoShow))
							return null;    //null for no error.
						else
							return "PATHtoShow is not valid[DBG]";
					default:
						throw new NotImplementedException();
				}
			}
		}
		#endregion

		List<string> history = new List<string>();  //For GoBack and GoForward.
		short historyIndex = -1;
		bool requestingBackForward = false;

		bool isLastPathValid = false;   //For validation use.
		string lastValidPath;
		string _PATHtoShow; //DirBox.Text
		public string PATHtoShow
		{
			get => _PATHtoShow;
			set
			{
				if (string.IsNullOrEmpty(value))
					_PATHtoShow = lastValidPath;   //Don't notify property change. Keep the path unchanged when this happens.
				else
				{
					_PATHtoShow = value;
					if (isLastPathValid = DirHelper.IsPathValid(_PATHtoShow))
					{   //Compare the incoming path to the last valid path and if they're the same, do nothing.
						//TODO: Bugs exists. Like if user use '/' instead of '\' for directory seperator.
						if (lastValidPath == _PATHtoShow)
							return; //Early return.
						lastValidPath = _PATHtoShow;
						if (!requestingBackForward)
						{
							history.RemoveRange(++historyIndex, history.Count - historyIndex);
							history.Add(lastValidPath);
						}
						OnPropertyChanged(nameof(PATHtoShow));
						ReGetContent();
					}
					else
						isLastPathValid = false;
				}
			}
		}

		[ObservableProperty]
		List<string> _Drives = new List<string>();
		[ObservableProperty]
		ObservableCollection<CustomContentItem> _Content = new ObservableCollection<CustomContentItem>();
		[ObservableProperty]
		ushort _SliderValue = 156;
		[ObservableProperty]
		bool _IsNotAddingItem = true;   //For RefreshButton IsEnabled.

		[RelayCommand]
		public void RefreshDrives()
		{
			Drives.Clear();
			foreach (string drive in Directory.GetLogicalDrives())
				Drives.Add(drive);
			App.Logger.Info($"User requested {System.Reflection.MethodBase.GetCurrentMethod().Name} and is completed.");
		}
		[RelayCommand]
		public void GoBack()
		{
			if (historyIndex > 0)
			{
				requestingBackForward = true;
				PATHtoShow = history[--historyIndex];
				requestingBackForward = false;
			}
		}
		[RelayCommand]
		public void GoForward()
		{
			if (historyIndex < history.Count - 1)
			{
				requestingBackForward = true;
				PATHtoShow = history[++historyIndex];
				requestingBackForward = false;
			}
		}
		[RelayCommand]
		public void ReGetContent()
		{
			App.Logger.Info($"User requested {System.Reflection.MethodBase.GetCurrentMethod().Name}.");
			IsNotAddingItem = false;
			if (addItemTask != null && !addItemTask.IsCompleted)
			{
				cts.Cancel();   //Cancel the task to avoid old folder being added to the list
				App.Logger.Info("Last loading has not completed and are canceled.");
			}
			Content.Clear();
			//Setup cancellation token for cancelling use.
			cts = new CancellationTokenSource();
			CancellationToken ct = cts.Token;
			//Long ass task, offload to another thread.
			//This took me forever.
			addItemTask = new Task(() =>
			{
				if (DirHelper.IsPathValid(_PATHtoShow))
				{
					string[] dirs = DirHelper.DirInPath(_PATHtoShow);
					if (dirs is not null)
					{
						CustomContentItem lastAdded = null;  //Mark the last added item.
						List<string> unauthorizedFolders = new List<string>();
						foreach (string dir in dirs)
						{
							try
							{
								ct.ThrowIfCancellationRequested();  //When task is canceled
							}
							catch (OperationCanceledException)
							{
								_Content.Remove(lastAdded); //Sometimes last added item can still appear on the list, this fixes it.
								App.Logger.Info("Canceling loading.");
								break;
							}
							FileAttributes dirAtt = new DirectoryInfo(dir).Attributes;
							if (!(dirAtt.HasFlag(FileAttributes.System)/* || dirAtt.HasFlag(FileAttributes.Hidden)*/))  //Actually hidden folders can be read now, it's handled below.
							{
								string[] allowedExt = { ".jpg", ".png", ".jpeg", ".gif" };
								Task<string[]> task = DirHelper.DirInPathTask(dir); //Async operation about finding if there's any subfolders.
								string[] subfolders;
								string firstFilePath;
								try //Get first image file. Is EnumerateFiles faster than GetFiles? idk.
								{ firstFilePath = Directory.EnumerateFiles(dir, "*.*").Where(s => allowedExt.Any(s.ToLower().EndsWith)).First(); }
								catch (InvalidOperationException)   //No such image file, set default
								{ firstFilePath = "Bruh"; }
								catch (UnauthorizedAccessException)
								{   //Some top secret folder encounted
									unauthorizedFolders.Add(dir);
									continue;   //Skip folder and continue with the next dir.
								}
								BitmapImage bitmap = new BitmapImage();
								if (firstFilePath == "Bruh")
								{   //Default thumbnail.
									bitmap.BeginInit();
									bitmap.DecodePixelWidth = SliderValue + 128;
									bitmap.UriSource = new Uri("pack://application:,,,/folder.png");
									bitmap.EndInit();
									bitmap.Freeze();
								}
								else
								{   //Picture in folder, load thumbnail.
									using (FileStream stream = File.OpenRead(firstFilePath))
									{   //Use stream sourec instead of regular uri source to improve responsiveness.
										bitmap.BeginInit();
										//This will fix badly encoded images.
										bitmap.CreateOptions = BitmapCreateOptions.PreservePixelFormat | BitmapCreateOptions.IgnoreColorProfile;
										//Use BitmapCacheOption.OnLoad to even make it display.
										bitmap.CacheOption = BitmapCacheOption.OnLoad;
										bitmap.StreamSource = stream;
										bitmap.DecodePixelWidth = SliderValue + 128;   //Make it sharper
										try { bitmap.EndInit(); }
										catch (NotSupportedException)   //Bad image file, use default.
										{
											App.Logger.Warn($"Bad image file detected in folder {dir}: {firstFilePath[(firstFilePath.LastIndexOf('\\') + 1)..]}. Falling back to default thumbnail.");
											bitmap = new BitmapImage();
											bitmap.BeginInit();
											bitmap.DecodePixelWidth = SliderValue + 128;
											bitmap.UriSource = new Uri("pack://application:,,,/folder.png");
											bitmap.EndInit();
										}
										//catch(ArgumentException)
										//{	//Bad encoded image? Fixed with bitmap.CreateOptions.
										//	GC.Collect();
										//	continue;
										//}
										catch (System.Runtime.InteropServices.COMException)
										{
											App.Logger.Info($"Skipping cloud storages and stuff (COMException): {dir}");
											GC.Collect();
											continue;   //Stupid cloud storages, skip.
										}
										catch (Exception e)
										{
											App.Logger.Warn($"Exception occured creating thumbnail {firstFilePath}\n{e.Message}");
											MessageBox.Show(e.Message, "im ded", MessageBoxButton.OK, MessageBoxImage.Warning);
											continue;
										}
										finally //This is VITAL for it to be passed between threads.
										{ bitmap.Freeze(); }
									}
								}
								CustomContentItem newItem = new CustomContentItem { ThumbNail = bitmap, Header = DirHelper.GetFileFolderName(dir) };
								subfolders = task.Result;
								if (subfolders.Length > 0 && bitmap.UriSource is null)
									newItem.HasSubfolder = true;
								lastAdded = newItem;
								//Items should be created in UI thread, and Dispatcher.Invoke does it.
								Application.Current.Dispatcher.Invoke(() => Content.Add(lastAdded));
								//The above line can cause exception as Application.Current will be null when the app is closed. But user have to close it anyway so yeah ill just leave it there.
							}
						}
						//Done adding stuff, notify user about unauthorized folder if any.
						if (unauthorizedFolders.Count > 0)
						{
							StringBuilder stringBuilder = new StringBuilder();
							foreach (string dir in unauthorizedFolders)
								stringBuilder.AppendLine(dir);
							App.Logger.Info("Unauthorized folders found, showing MessageBox.");
							MessageBox.Show("One or more folder(s) has been skipped due to lack of permission:\n" + stringBuilder + "\nIf you want to access these folder(s), try running the app as Administrator.", "Unauthorized access to folder is denied.", MessageBoxButton.OK, MessageBoxImage.Asterisk);
						}
						IsNotAddingItem = true;
						if (ct.IsCancellationRequested)
							App.Logger.Info("Loading has canceled.");
						else
							App.Logger.Info("Loading has finished.");
					}
				}
				else
					IsNotAddingItem = true;
			}, ct);
			addItemTask.Start();
		}
		[RelayCommand]
		public void ThumbnailClicked(Image img)
		{   //The LeftClick MouseAction only considers mouse down, I need mouse up for this.
			Mouse.RemoveMouseUpHandler(img, ThumbnailMouseUpHandler);
			Mouse.AddMouseUpHandler(img, ThumbnailMouseUpHandler);
		}
		[RelayCommand]
		public void TextDoubleClicked(TextBlock tb)
		{   //Double click advance folder
			string imageFolder = tb.Text;
			PATHtoShow = PATHtoShow.EndsWith('\\') ? string.Format("{0}{1}", PATHtoShow, imageFolder) : string.Format("{0}\\{1}", PATHtoShow, imageFolder);
		}
		[RelayCommand]
		public void SubfolderIconClicked(string folderName)
		{
			PATHtoShow = PATHtoShow.EndsWith('\\') ? string.Format("{0}{1}", PATHtoShow, folderName) : string.Format("{0}\\{1}", PATHtoShow, folderName);
		}

		private void ThumbnailMouseUpHandler(object sender, MouseButtonEventArgs e) //Open Photo Viewer or advance path.
		{
			string imageFolder = ((Image)sender).ToolTip.ToString();
			if (e.ChangedButton == MouseButton.Left)
			{
				if (((Image)sender).Source.ToString().EndsWith("folder.png"))   //No image found (default folder.png), advance path.
					PATHtoShow = PATHtoShow.EndsWith('\\') ? string.Format("{0}{1}", PATHtoShow, imageFolder) : string.Format("{0}\\{1}", PATHtoShow, imageFolder);
				else
				{   //Image found, start Photo Viewer.
					string imagePath = ((FileStream)((BitmapImage)((Image)sender).Source).StreamSource).Name;
					imagePath = imagePath.Remove(imagePath.LastIndexOf('\\'));
					App.Logger.Info("Starting PhotoViewer with path " + imagePath);
					PhotoViewer photoViewer = new PhotoViewer(imagePath)
					{
						Width = Properties.Settings.Default.PV_Width,
						Height = Properties.Settings.Default.PV_Height,
						Left = Properties.Settings.Default.PV_Left,
						Top = Properties.Settings.Default.PV_Top
					};
					wnds.Add(photoViewer); //Add this to opened windows list to close it when mainwindow closes
					photoViewer.Show();
					App.Logger.Info("PhotoViewer started with path " + imagePath);
				}
			}
			else if (e.ChangedButton == MouseButton.Right)  //Right click to Start explorer.exe
			{
				Process.Start("explorer.exe", _PATHtoShow.EndsWith('\\') ? (_PATHtoShow + imageFolder) : (_PATHtoShow + '\\' + imageFolder));
				App.Logger.Info("Started explorer at: " + _PATHtoShow + ". Folder name: " + imageFolder);
			}
		}

		#region AddNewFavorite
		private Button addBtn;
		[RelayCommand]
		public void AddNewFav(Button button)
		{
			addBtn = button;
			AddNewFav addNewFav = new AddNewFav(_PATHtoShow);
			wnds.Add(addNewFav);   //Add this to opened windows list to close it when mainwindow closes
			addNewFav.Show();
			addNewFav.Closed += AddNewFav_Closed;
			addBtn.IsEnabled = false;
			App.Logger.Info($"User requested {System.Reflection.MethodBase.GetCurrentMethod().Name} and is completed.");
		}
		private void AddNewFav_Closed(object? sender, EventArgs e)
		{
			App.Logger.Info("AddNewFav window has been closed.");
			OnPropertyChanged(nameof(ComboBoxItems));
			addBtn.IsEnabled = true;
		}
		#endregion
		#region Favorites ComboBox section.
		private Dictionary<string, string> FavItem = new Dictionary<string, string>();
		private ComboBoxItem _cbBoxSelected = new ComboBoxItem();
		public ComboBoxItem CBBoxSelected
		{
			get
			{
				if (_cbBoxSelected == null) //When user added new favorite, _cbBoxSelected will be null
				{   //because it causes the combobox to update and end up with nothing selected.
					_cbBoxSelected = new ComboBoxItem { ToolTip = "" };
					return _cbBoxSelected;
				}
				_cbBoxSelected.ToolTip = FavItem[_cbBoxSelected.Content.ToString()];
				return _cbBoxSelected;
			}
			set
			{
				if (value is not null)
				{
					_cbBoxSelected = value;
					PATHtoShow = _cbBoxSelected.ToolTip.ToString();
				}
			}
		}
		private ObservableCollection<ComboBoxItem> _ComboBoxItems = new ObservableCollection<ComboBoxItem>();
		public ObservableCollection<ComboBoxItem> ComboBoxItems
		{
			get
			{
				App.Logger.Info("Getting favorite folders from Favorites.txt.");
				Task.Run(() =>
				{
					string favPath = Directory.GetCurrentDirectory() + "\\Favorites.txt";
					if (!File.Exists(favPath))
						File.Create(favPath).Close();   //The Close() ensures that it has been created.
					string[] favTexts = File.ReadAllLines(favPath);
					List<string> nameList = new List<string>();
					bool isOddLine = true; bool isDuplicate = false; bool duplicateFound = false;
					string tempName = string.Empty;
					//Clear everything first to refresh.
					FavItem.Clear();
					_ComboBoxItems.Clear();
					foreach (string str in favTexts)
					{
						if (isDuplicate)
						{
							isDuplicate = false;
							continue;   //Skip twice
						}
						if (isOddLine)  //Getting Name
						{
							tempName = str[(str.LastIndexOf('|') + 1)..];
							if (nameList.Contains(tempName))    //Check for duplicates
							{
								isDuplicate = true;
								duplicateFound = true;
								continue;   //Skip this line
							}
							nameList.Add(tempName);
						}
						else    //Getting Path
						{
							FavItem.Add(tempName, str[(str.LastIndexOf('|') + 1)..]);
							ComboBoxItem comboBoxItem = null;
							Application.Current.Dispatcher.Invoke(() => comboBoxItem = new ComboBoxItem { Content = tempName, ToolTip = str[(str.LastIndexOf('|') + 1)..] });
							_ComboBoxItems.Add(comboBoxItem);
						}
						isOddLine = !isOddLine;
					}
					if (duplicateFound)
						MessageBox.Show("Duplicated name found in Favorites.txt and are ignored, please fix that.", "NO", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				});
				return _ComboBoxItems;
			}
			set { _ComboBoxItems = value; }
		}
		#endregion
		public MainPageViewModel()
		{
			//Do this so the ObservableCollection can be shared between threads
			BindingOperations.EnableCollectionSynchronization(Content, new object());
			BindingOperations.EnableCollectionSynchronization(_ComboBoxItems, new object());
			RefreshDrives();
		}
	}
}
