using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FolderThumbnailExplorer.Converter
{
	public class LabelSizeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Math.Sqrt((double)value);
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			MessageBox.Show("LabelSizeConverter ConverBack not implemented.");
			throw new NotImplementedException();
		}
	}
}
