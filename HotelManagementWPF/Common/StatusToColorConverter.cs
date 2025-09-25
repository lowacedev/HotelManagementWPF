using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using HotelManagementWPF.Models;

namespace HotelManagementWPF.Common
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is RoomStatus status)
            {
                return status switch
                {
                    RoomStatus.Available => new SolidColorBrush(Color.FromRgb(74, 144, 226)), // blue
                    RoomStatus.Booked => new SolidColorBrush(Color.FromRgb(255, 99, 99)), // red
                    RoomStatus.Reserved => new SolidColorBrush(Color.FromRgb(76, 175, 80)), // green
                    RoomStatus.Waitlist => new SolidColorBrush(Color.FromRgb(255, 167, 38)), // orange
                    RoomStatus.Blocked => new SolidColorBrush(Color.FromRgb(160, 160, 160)), // gray
                    _ => new SolidColorBrush(Color.FromRgb(160, 160, 160))
                };
            }
            return new SolidColorBrush(Color.FromRgb(160, 160, 160));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
