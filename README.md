# Folder Thumbnail Explorer
Revised version of the custom file explorer(https://github.com/ChiNoel-osu/NETCoreWPFTest/tree/master/LocalFileExplorer). Written in C# WPF/.NET 8.

[![CodeFactor](https://www.codefactor.io/repository/github/chinoel-osu/folderthumbnailexplorer/badge)](https://www.codefactor.io/repository/github/chinoel-osu/folderthumbnailexplorer)

## What this does
- It shows the first image in a folder as that folder's thumbnail (if there is one).
- It also comes with a photo viewer that shows images in a folder.

## Running the release package
1. Download and install [.NET 8 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) if your PC doesn't have one.
	- Look for `.NET Desktop Runtime 8.0.11`! You don't have to install the whole SDK.
2. You're all set! Run the `FolderThumbnailExplorer.exe` to start exploring your folders!

## Building it yourself
1. Get Visual Studio 2022 with .NET desktop development workload. Make sure to have .NET 8 selected.
2. Open the `FolderThumbnailExplorer.sln` in Visual Studio.
3. Install the required NuGet packages.
	- CommunityToolkit.Mvvm
	- NLog
4. Start criticizing my code and post issues 😉.


## Why this
- It's simple and small.
- Comes with the following features:
	1. Favorite directories.
	2. Favorite folders.
	3. A photo viewer that gets the job done.
		- Full screen mode.
		- Incremental loading.
		- Twin-page viewing.
	4. Image caching system that doesn't rape your harddrive.
	5. Multi-Language support. (Currently zh-CN and en-US, it's fairly easy to add languages.) 

## What needs to be done
- A better UI and UX.
- More shortcuts.
- Fix bugs.
- *And more....Please post an issue.*

## Very sfw screenshots
![A screenshot of the software's main page. ](./ReadmeResource/Mainpage.jpg?raw=true "Main Page")
![A screenshot of the Photo Viewer.](./ReadmeResource/PhotoViewer.jpg?raw=true "Photo Viewer")

## Known issues
- Very high RAM usage for what it does. But I'm sure Windows can manage.

## Copyright Statement
While the code is licensed under the AGPL-3.0 license. The compiled application (or software) should be licensed under the [CC BY-NC-SA 4.0](./LICENSE-CC-BY-NC-SA) license. You can freely share the application to wherever you like, for NonCommercial purposes only. Meaning that you cannot sell the application, or redistribute the appcation behind some kind of pay wall. The application should remain absolutely free for everyone.
