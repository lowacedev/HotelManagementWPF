using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace HotelManagementWPF.Models
{
    public class RoomStatusToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is RoomStatus status)
            {
                return status switch
                {
                    RoomStatus.Available => new SolidColorBrush(Color.FromRgb(74, 144, 226)),   // Blue
                    RoomStatus.Booked => new SolidColorBrush(Color.FromRgb(255, 99, 99)),      // Red
                    RoomStatus.Reserved => new SolidColorBrush(Color.FromRgb(76, 175, 80)),    // Green
                    RoomStatus.Waitlist => new SolidColorBrush(Color.FromRgb(255, 167, 38)),   // Orange
                    RoomStatus.Blocked => new SolidColorBrush(Color.FromRgb(160, 160, 160)),   // Gray
                    _ => new SolidColorBrush(Color.FromRgb(176, 190, 197))
                };
            }
            return new SolidColorBrush(Color.FromRgb(176, 190, 197));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}