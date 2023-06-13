using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FolderThumbnailExplorer.Model
{
	public class NaturalStringComparer : IComparer<string>
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
}
