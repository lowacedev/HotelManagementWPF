using System;
using System.Windows;
using System.Windows.Controls;
using HotelManagementWPF.ViewModels.Booking;
using HotelManagementWPF.Views.Booking;
using Syncfusion.UI.Xaml.Scheduler;

namespace HotelManagementWPF.Views.Booking
{
    public partial class BookingView : UserControl
    {
        public BookingView()
        {
            InitializeComponent();
            DataContext = new BookingViewModel();
            BookingScheduler.CellTapped += new EventHandler<CellTappedEventArgs>(BookingScheduler_CellTapped);
        }

        private void BookingScheduler_CellTapped(object? sender, CellTappedEventArgs e)
        {
            // Only open booking form for empty cells (no appointment)
            if (e.Appointments == null || e.Appointments.Count == 0)
            {
                // Get room and date info from cell
                var resource = e.Resource as dynamic;
                string roomNumber = resource?.Name ?? "";
                string roomType = resource?.Type ?? "";
                DateTime checkInDate = e.DateTime;
                OpenBookingForm(roomNumber, roomType, checkInDate);
            }
        }

        // This method can be called from the CellTemplate's Button click event handler
        public void OpenBookingForm(string roomNumber, string roomType, DateTime checkInDate)
        {
            var form = new BookingFormView();
            var vm = new BookingFormViewModel();
            vm.PreFill(roomNumber, roomType, checkInDate);
            form.DataContext = vm;
            form.Owner = Window.GetWindow(this);    
            form.ShowDialog();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
