using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace HotelManagementWPF.Common
{
    public class GenderToForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Optional: different foregrounds for different genders
            // For simplicity, returning white for both
            return new SolidColorBrush(Colors.White);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}