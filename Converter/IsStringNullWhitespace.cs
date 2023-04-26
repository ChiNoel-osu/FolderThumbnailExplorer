using System;
using System.Globalization;
using System.Windows.Data;

namespace FolderThumbnailExplorer.Converter
{
	public class IsStringNullWhitespace : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return bool.Parse((string)parameter) ? !string.IsNullOrWhiteSpace((string)value) : string.IsNullOrWhiteSpace((string)value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
