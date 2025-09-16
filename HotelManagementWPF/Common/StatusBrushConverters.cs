using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using HotelManagementWPF.Models;

namespace HotelManagementWPF.Common
{
    public class StatusToForegroundConverter : IValueConverter
    {
        public StatusToForegroundConverter() { }
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
                    RoomStatus.Blocked => new SolidColorBrush(Color.FromRgb(255, 180, 60)),    // Orange for Blocked (matches screenshot)
                    _ => new SolidColorBrush(Color.FromRgb(160, 160, 160))
                };
            }
            return new SolidColorBrush(Color.FromRgb(160,160,160));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class StatusToBackgroundConverter : IValueConverter
    {
        public StatusToBackgroundConverter() { }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is RoomStatus status)
            {
                return status switch
                {
                    RoomStatus.Available => new SolidColorBrush(Color.FromRgb(230, 242, 255)),   // Very light blue
                    RoomStatus.Booked => new SolidColorBrush(Color.FromRgb(255, 237, 237)),      // Very light red/pink
                    RoomStatus.Reserved => new SolidColorBrush(Color.FromRgb(230, 255, 241)),    // Very light green
                    RoomStatus.Waitlist => new SolidColorBrush(Color.FromRgb(255, 249, 230)),    // Very light orange
                    RoomStatus.Blocked => new SolidColorBrush(Color.FromRgb(255, 236, 200)),     // Very light orange for Blocked
                    _ => new SolidColorBrush(Color.FromRgb(245, 245, 245))
                };
            }
            return new SolidColorBrush(Color.FromRgb(245,245,245));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
