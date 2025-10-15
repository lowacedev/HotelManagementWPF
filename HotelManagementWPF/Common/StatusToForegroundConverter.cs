using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace HotelManagementWPF.Common
{
    public class StatusToForegroundConverters : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string status = value as string;
            switch (status)
            {
                case "Check-In":
                    return new SolidColorBrush(Colors.Green);
                case "Check-Out":
                    return new SolidColorBrush(Colors.Blue);
                case "Reservation":
                    return new SolidColorBrush(Colors.Yellow); // #999933
                default:
                    return new SolidColorBrush(Colors.Black);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
