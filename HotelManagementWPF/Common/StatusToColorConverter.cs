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
                    RoomStatus.Available => Colors.Green,
                    RoomStatus.Booked => Colors.Red,
                    RoomStatus.Reserved => Colors.Orange,
                    RoomStatus.Waitlist => Colors.Blue,
                    RoomStatus.Blocked => Colors.Gray,
                    _ => Colors.Gray
                };
            }
            return Colors.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
