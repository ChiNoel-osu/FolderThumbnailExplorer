using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FolderThumbnailExplorer.Model;
using FolderThumbnailExplorer.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
	public partial class MainPageViewModel : ObservableObject
	{
		public CancellationTokenSource cts = new CancellationTokenSource();
		public Task addItemTask;
		public static Stack<Window> wnds = new Stack<Window>();

		string _PATHtoShow; //DirBox.Text
		public string PATHtoShow
		{
			get => _PATHtoShow;
			set
			{
				_PATHtoShow = value;
				OnPropertyChanged(nameof(PATHtoShow));
				ReGetContent();
			}
		}

		[ObservableProperty]
		ObservableCollection<CustomContentItem> _Content = new ObservableCollection<CustomContentItem>();
		[ObservableProperty]
		ushort _SliderValue = 156;
		[ObservableProperty]
		bool _IsNotAddingItem = true;   //For RefreshButton IsEnabled.

		[RelayCommand]
		public void ReGetContent()
		{
			IsNotAddingItem = false;
			if (addItemTask != null && !addItemTask.IsCompleted)
				cts.Cancel();   //Cancel the task to avoid old folder being added to the list
			Content.Clear();
			//Setup cancellation token for cancelling use.
			cts = new CancellationTokenSource();
			CancellationToken ct = cts.Token;
			//Long ass task, offload to another thread.
			//This took me forever.
			addItemTask = new Task(() =>
			{
				DirShit dirShit = new DirShit();
				if (dirShit.ContentExistsInPath(_PATHtoShow))
				{
					string[] dirs = dirShit.DirInPath(_PATHtoShow);
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
								break;
							}
							FileAttributes dirAtt = new DirectoryInfo(dir).Attributes;
							if (!(dirAtt.HasFlag(FileAttributes.System)/* || dirAtt.HasFlag(FileAttributes.Hidden)*/))  //Actually hidden folders can be read now, it's handled below.
							{
								string[] allowedExt = { ".jpg", ".png", ".jpeg", ".gif" };
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
									using (FileStream stream = File.OpenRead(firstFilePath))
									{   //Use stream sourec instead of regular uri source to improve responsiveness.
										bitmap.BeginInit();
										//Use BitmapCacheOption.OnLoad to even make it display.
										bitmap.CacheOption = BitmapCacheOption.OnLoad;
										bitmap.StreamSource = stream;
										bitmap.DecodePixelWidth = SliderValue + 128;   //Make it sharper
										try { bitmap.EndInit(); }
										catch (NotSupportedException)   //Bad image file, use default.
										{
											bitmap = new BitmapImage();
											bitmap.BeginInit();
											bitmap.DecodePixelWidth = SliderValue + 128;
											bitmap.UriSource = new Uri("pack://application:,,,/folder.png");
											bitmap.EndInit();
										}
										finally //This is VITAL for it to be passed between threads.
										{ bitmap.Freeze(); }
									}
								CustomContentItem newItem = new CustomContentItem { ThumbNail = bitmap, Header = dirShit.GetFileFolderName(dir) };
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
							MessageBox.Show("One or more folder(s) has been skipped due to lack of permission:\n" + stringBuilder + "\nIf you want to access these folder(s), try running the app as Administrator.", "Unauthorized access to folder is denied.", MessageBoxButton.OK, MessageBoxImage.Asterisk);
						}
						IsNotAddingItem = true;
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
		public void LabelDoubleClicked(Label label)
		{   //Double click advance folder
			string imageFolder = label.Content.ToString();
			PATHtoShow = PATHtoShow.EndsWith('\\') ? string.Format("{0}{1}", PATHtoShow, imageFolder) : string.Format("{0}\\{1}", PATHtoShow, imageFolder);
		}

		private void ThumbnailMouseUpHandler(object sender, MouseButtonEventArgs e)
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
					PhotoViewer photoViewer = new PhotoViewer(imagePath);
					photoViewer.Left = 0; photoViewer.Top = 0;  //Spawns window at top left corner.
					wnds.Push(photoViewer); //Add this to opened windows list to close it when mainwindows closes
					photoViewer.Show();
				}
			}
			else if (e.ChangedButton == MouseButton.Right)  //Right click to Start explorer.exe
				Process.Start("explorer.exe", PATHtoShow.EndsWith('\\') ? (PATHtoShow + imageFolder) : (PATHtoShow + '\\' + imageFolder));
		}

		#region AddNewFavorite
		private Button addBtn;
		[ObservableProperty]
		int _CBBoxSelectedIndex = -1;
		[RelayCommand]
		public void AddNewFav(Button button)
		{
			addBtn = button;
			AddNewFav addNewFav = new AddNewFav(_PATHtoShow);
			wnds.Push(addNewFav);   //Add this to opened windows list to close it when mainwindow closes
			addNewFav.Show();
			addNewFav.Closed += AddNewFav_Closed;
			addBtn.IsEnabled = false;
		}
		private void AddNewFav_Closed(object? sender, EventArgs e)
		{
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
				_cbBoxSelected = value;
				PATHtoShow = _cbBoxSelected?.ToolTip.ToString() ?? string.Empty;
			}
		}
		private ObservableCollection<ComboBoxItem> _ComboBoxItems = new ObservableCollection<ComboBoxItem>();
		public ObservableCollection<ComboBoxItem> ComboBoxItems
		{
			get
			{
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
		}
	}
}
