using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FolderThumbnailExplorer.Converter
{
	public class Bool2VisivilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (bool)value ? Visibility.Visible : Visibility.Hidden;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (Visibility)value switch
			{
				Visibility.Visible => true,
				Visibility.Hidden => (object)false,
				_ => throw new NotImplementedException(),
			};
		}
	}
}
