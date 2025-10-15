using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace HotelManagementWPF.Common
{
    public class StockLevelToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Brushes.Gray; // fallback color

            string stockStatus = value.ToString().Trim();

            switch (stockStatus)
            {
                case "Low Stock":
                    // darker yellow
                    return Brushes.Orange;
                case "In Stock":
                    return Brushes.Green;
                case "Out of Stock":
                    return Brushes.Red;
                default:
                    // fallback for unexpected values
                    return Brushes.Black;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}