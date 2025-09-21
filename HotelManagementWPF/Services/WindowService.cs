using HotelManagementWPF.Views.Room;
using HotelManagementWPF.Views.Booking;
using HotelManagementWPF.ViewModels.Booking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HotelManagementWPF.Services
{
    public class WindowService : IWindowService
    {
        // Existing Room methods
        public void ShowAddRoomDialog()
        {
            var window = new AddRoomFormView();
            window.ShowDialog();
        }

        public void ShowAddRoomForm()
        {
            var addRoomForm = new AddRoomFormView();
            addRoomForm.Owner = Application.Current.MainWindow;
            addRoomForm.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            addRoomForm.ShowDialog();
        }

        public void ShowEditRoomForm(Models.Room room)
        {
            var editForm = new Views.Room.EditRoomFormView(room);
            editForm.Owner = Application.Current.MainWindow;
            var result = editForm.ShowDialog();
            if (result == true)
            {
                // Refresh the room list after successful edit
                // You might want to raise an event or call a refresh method
            }
        }

        // New Booking methods
        public void ShowAddBookingForm()
        {
            var form = new BookingFormView();
            var vm = new BookingFormViewModel();
            form.DataContext = vm;
            form.Owner = Application.Current.MainWindow;
            form.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            form.ShowDialog();
        }

        public void ShowEditBookingForm(BookingData booking)
        {
            if (booking == null) return;

            var form = new BookingFormView();
            var vm = new BookingFormViewModel();

            // Pre-fill with booking data for editing
            vm.FullName = booking.Guest;
            vm.RoomNumber = booking.Room;
            vm.CheckInDate = booking.CheckIn;
            vm.CheckOutDate = booking.CheckOut;
            // Set other properties as needed based on your BookingData model

            form.DataContext = vm;
            form.Owner = Application.Current.MainWindow;
            form.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            form.ShowDialog();
        }
    }
}