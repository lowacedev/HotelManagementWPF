using HotelManagementWPF.ViewModels.Booking;
using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace HotelManagementWPF.Views.Booking
{
    public partial class BookingView : UserControl
    {
        public BookingView()
        {
            InitializeComponent();
            // Set the DataContext to your ViewModel
            this.DataContext = new BookingViewModel();
        }
    }
    public class StatusToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                return status switch
                {
                    "Active" => new SolidColorBrush(Color.FromRgb(76, 175, 80)),
                    "Completed" => new SolidColorBrush(Color.FromRgb(52, 152, 219)),
                    "Cancelled" => new SolidColorBrush(Color.FromRgb(231, 76, 60)),
                    _ => new SolidColorBrush(Color.FromRgb(149, 165, 166))
                };
            }
            return new SolidColorBrush(Color.FromRgb(149, 165, 166));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StatusToForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // All statuses have white foreground for contrast
            return new SolidColorBrush(Colors.White);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}