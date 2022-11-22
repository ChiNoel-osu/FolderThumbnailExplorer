using FolderThumbnailExplorer.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FolderThumbnailExplorer.View
{
	/// <summary>
	/// MainPage.xaml 的交互逻辑
	/// </summary>
	public partial class MainPage : UserControl
	{
		public MainPage()
		{
			InitializeComponent();
		}

		private void DirBox_TextChanged(object sender, TextChangedEventArgs e)
		{   //Update folder view.
			MainWindow.MainVM.MainPageViewModel.PATHtoShow = DirBox.Text;
			if (MainWindow.MainVM.MainPageViewModel.addItemTask != null && !MainWindow.MainVM.MainPageViewModel.addItemTask.IsCompleted)
				MainWindow.MainVM.MainPageViewModel.cts.Cancel();   //Cancel the task to avoid old folder being added to the list
			MainWindow.MainVM.MainPageViewModel.ReGetContent();
		}
		#region GOUP function
		private void UserControl_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Back)  //Go up
				GoUP();
		}
		private void GoUpBtnClicked(object sender, RoutedEventArgs e)
		{
			GoUP();
		}
		private void GoUP()
		{
			if (DirBox.Text != string.Empty)
				if (DirBox.Text.Remove(DirBox.Text.LastIndexOf('\\')).Length == 2)
					DirBox.Text = string.Format("{0}:\\", DirBox.Text[0]);
				else
					DirBox.Text = DirBox.Text.Remove(DirBox.Text.LastIndexOf('\\'));
		}
		#endregion

		private void RefreshButton_Click(object sender, RoutedEventArgs e)
		{
			DirBox_TextChanged(null, null);
		}
		private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			DirBox.Text = MainWindow.MainVM.MainPageViewModel.CBBoxSelected.ToolTip.ToString();
		}
		#region ListBox item event. Had to use code-behind bc i suck
		private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			string imagePath = ((Image)sender).Source.ToString()[8..].Replace('/', '\\');
			string imageFolder = ((Image)sender).ToolTip.ToString();
			if (imagePath.EndsWith("folder.png"))
			{   //No image found (default folder.png), advance path.
				if (DirBox.Text.EndsWith('\\')) //Check drive root
					DirBox.Text = string.Format("{0}{1}", DirBox.Text, imageFolder);
				else
					DirBox.Text = string.Format("{0}\\{1}", DirBox.Text, imageFolder);
			}
			else
			{   //Image found, start Photo Viewer.
				string folderToView = ((Image)sender).Source.ToString()[8..].Replace('/', '\\');
				if (folderToView.EndsWith("folder.png")) return;    //No image in folder, return.
				folderToView = folderToView.Remove(folderToView.LastIndexOf('\\'));
				PhotoViewer photoViewer = new PhotoViewer(folderToView);
				photoViewer.Left = 0; photoViewer.Top = 0;  //Spawns window at top left corner.

				MainPageViewModel.wnds.Push(photoViewer); //Add this to opened windows list to close it when mainwindows closes
				photoViewer.Show();
				if (((PhotoViewerViewModel)photoViewer.DataContext).ImageCount == 0)
					photoViewer.Close(); //No image after init, close the opened window.
										 //Because the ImageCount is only used after Show() has called (It's bound to the view)
										 //I have to check AFTER it's showed, which will make it confusing to users.
			}
		}
		private void Image_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
		{
			//Right click to Start explorer.exe
			string imageFolder = ((Image)sender).ToolTip.ToString();
			string path;
			if (DirBox.Text.EndsWith('\\'))
				path = DirBox.Text + imageFolder;
			else
				path = DirBox.Text + '\\' + imageFolder;
			//The SelectedPath changed and will run the "set property" code.
			MainWindow.MainVM.MainPageViewModel.SelectedPath = path;
		}
		private void Label_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			string imageFolder = ((Label)sender).Content.ToString();
			if (DirBox.Text.EndsWith('\\')) //Check drive root
				DirBox.Text = string.Format("{0}{1}", DirBox.Text, imageFolder);
			else
				DirBox.Text = string.Format("{0}\\{1}", DirBox.Text, imageFolder);
		}
		#endregion

		private void NewFav_Click(object sender, RoutedEventArgs e)
		{
			AddNewFav addNewFav = new AddNewFav(DirBox.Text);
			MainPageViewModel.wnds.Push(addNewFav);   //Add this to opened windows list to close it when mainwindow closes
			addNewFav.Show();
			addNewFav.Closed += AddNewFav_Closed;
			AddButton.IsEnabled = false;
		}
		private void AddNewFav_Closed(object sender, EventArgs e)
		{
			//Update ItemsSource
			int beforeUpdateCnt = FavCB.Items.Count;
			int beforeUpdateIndex = FavCB.SelectedIndex;
			//This will trigger the <get> block of ComboBoxitems
			var thing = MainWindow.MainVM.MainPageViewModel.ComboBoxItems; thing = null;
			if (beforeUpdateCnt == FavCB.Items.Count)
				FavCB.SelectedIndex = beforeUpdateIndex;
			else
				FavCB.SelectedIndex = FavCB.Items.Count - 1;
			GC.Collect();   //Does it delete the thing? idk.
			AddButton.IsEnabled = true;
		}
	}
}
