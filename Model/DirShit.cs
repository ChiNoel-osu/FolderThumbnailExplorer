using System;
using System.IO;
using System.Threading.Tasks;

namespace FolderThumbnailExplorer.Model
{	//God damnit what have I done.
	public class DirShit
	{
		public string[] GetLDrives() => Directory.GetLogicalDrives();
		public string[] DirInPath(string path)
		{
			try
			{
				return Directory.GetDirectories(path);
			}
			catch (Exception)
			{
				return null;
			}
		}
		public string[] FileInPath(string path)
		{
			try
			{
				return Directory.GetFiles(path);
			}
			catch (Exception)
			{
				return null;
			}
		}
		public Task<string[]> DirInPathTask(string path)
		{
			Task<string[]> task = new Task<string[]>(() => DirInPath(path));
			task.Start();
			return task;
		}
		public Task<string[]> FileInPathTask(string path)
		{
			Task<string[]> task = new Task<string[]>(() => FileInPath(path));
			task.Start();
			return task;
		}
		public string GetFileFolderName(string path)
		{
			if (path != null)
				return path[(path.LastIndexOf('\\') + 1)..];
			else
				return string.Empty;
		}
		public bool ContentExistsInPath(string path)
		{
			return Directory.Exists(path) || File.Exists(path);
		}
	}
}
