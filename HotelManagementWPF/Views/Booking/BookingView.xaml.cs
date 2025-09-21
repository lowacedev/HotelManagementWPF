using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using HotelManagementWPF.ViewModels.Booking;

namespace HotelManagementWPF.Views.Booking
{
    public partial class BookingView : UserControl
    {
        public BookingView()
        {
            InitializeComponent();
            DataContext = new BookingViewModel();
        }
    }

    // Status to Background Color Converter for booking statuses
    public class StatusToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                return status switch
                {
                    "Active" => new SolidColorBrush(Color.FromRgb(76, 175, 80)), // Green
                    "Completed" => new SolidColorBrush(Color.FromRgb(52, 152, 219)), // Blue
                    "Cancelled" => new SolidColorBrush(Color.FromRgb(231, 76, 60)), // Red
                    _ => new SolidColorBrush(Color.FromRgb(149, 165, 166)) // Gray
                };
            }
            return new SolidColorBrush(Color.FromRgb(149, 165, 166));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Status to Foreground Color Converter for booking statuses
    public class StatusToForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // All statuses use white text for better contrast
            return new SolidColorBrush(Colors.White);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}