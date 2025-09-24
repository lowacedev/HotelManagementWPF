using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace HotelManagementWPF.Common
{
    public class GenderToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string gender = value as string;
            if (string.Equals(gender, "Male", StringComparison.OrdinalIgnoreCase))
                return new SolidColorBrush(Color.FromRgb(74, 144, 226));
            else if (string.Equals(gender, "Female", StringComparison.OrdinalIgnoreCase))
                return new SolidColorBrush(Color.FromRgb(231, 84, 128));
            else
                return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}