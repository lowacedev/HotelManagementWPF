using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace HotelManagementWPF.Common
{
    // Returns background brush for page button: highlighted for selected page
    public class PageBackgroundConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length >= 2 && values[0] is int page && values[1] is int current)
            {
                if (page == current)
                {
                    return new SolidColorBrush(Color.FromRgb(227, 240, 255)); // light blue
                }
            }
            return Brushes.White;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    // Returns foreground brush for page button
    public class PageForegroundConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length >= 2 && values[0] is int page && values[1] is int current)
            {
                if (page == current)
                {
                    return new SolidColorBrush(Color.FromRgb(74, 144, 226)); // blue
                }
            }
            return new SolidColorBrush(Color.FromRgb(44,62,80)); // default text
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
