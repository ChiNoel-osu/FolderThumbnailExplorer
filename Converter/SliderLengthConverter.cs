using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FolderThumbnailExplorer.Converter
{
	public class SliderLengthConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if ((double)value > 2)
				return (double)value * 10;
			return 10;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			MessageBox.Show("SliderLengthConverter ConvertBack not implemented.");
			throw new NotImplementedException();
		}
	}
}
