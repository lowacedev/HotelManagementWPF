using HotelManagementWPF.ViewModels;

using HotelManagementWPF.ViewModels.Booking;
using HotelManagementWPF.ViewModels.Supplier;
using HotelManagementWPF.ViewModels.Users;
using HotelManagementWPF.Views;
using HotelManagementWPF.Views.Booking;
using HotelManagementWPF.Views.Guest;
using HotelManagementWPF.Views.Room;
using HotelManagementWPF.Views.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using HotelManagementWPF.Models;


namespace HotelManagementWPF.Services
{
    public class WindowService : IWindowService
    {
        // Existing Room methods
        public void ShowAddRoomDialog()
        {
            var mainViewModel = (Application.Current.MainWindow.DataContext as RoomViewModel);
            var window = new AddRoomFormView(mainViewModel);
            window.ShowDialog();
        }

        public void ShowAddRoomForm()
        {
            var mainViewModel = (Application.Current.MainWindow.DataContext as RoomViewModel);
            var addRoomForm = new AddRoomFormView(mainViewModel);
            addRoomForm.Owner = Application.Current.MainWindow;
            addRoomForm.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            addRoomForm.ShowDialog();
        }

        public void ShowEditRoomForm(Room room)
        {
            var editForm = new Views.Room.EditRoomFormView(room.Id);
            editForm.Owner = Application.Current.MainWindow;
            var result = editForm.ShowDialog();
            if (result == true)
            {
                // Refresh the room list after successful edit
                // You might want to raise an event or call a refresh method
            }
        }

        // Booking methods
        public void ShowAddBookingForm()
        {
            var form = new BookingFormView();
            var vm = new BookingFormViewModel();
            form.DataContext = vm;
            form.Owner = Application.Current.MainWindow;
            form.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            form.ShowDialog();
        }

        public void ShowEditBookingForm(HotelManagementWPF.Models.BookingData booking)
        {
            if (booking == null) return;

            var form = new BookingFormView();
            var vm = new BookingFormViewModel();

            // Pre-fill with booking data for editing
            vm.FullName = booking.Guest;
            vm.RoomNumber = booking.RoomNumber; // corrected property name
            vm.CheckInDate = booking.CheckIn;
            vm.CheckOutDate = booking.CheckOut;
            // set other properties as needed

            form.DataContext = vm;
            form.Owner = Application.Current.MainWindow;
            form.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            form.ShowDialog();
        }

        // Guest methods
        public void ShowAddGuestForm()
        {
            var addGuestForm = new AddGuestFormView();
            addGuestForm.Owner = Application.Current.MainWindow;
            addGuestForm.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            addGuestForm.ShowDialog();
        }

        // User methods
        public void ShowAddUserForm()
        {
            var addUserForm = new AddUserFormView();
            var vm = new AddUserFormViewModel();
            addUserForm.DataContext = vm;
            addUserForm.Owner = Application.Current.MainWindow;
            addUserForm.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            addUserForm.ShowDialog();
        }

        public void ShowEditUserForm(ViewModels.User user)
        {
            if (user == null) return;

            var editForm = new EditUserFormView();
            var vm = new EditUserFormViewModel(user);
            editForm.DataContext = vm;
            editForm.Owner = Application.Current.MainWindow;
            editForm.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            editForm.ShowDialog();
        }

        public void ShowAddSupplierForm()
        {
            var addSupplierForm = new Views.Inventory.Suppliers.AddSupplierFormView();
            addSupplierForm.Owner = Application.Current.MainWindow;
            addSupplierForm.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            addSupplierForm.ShowDialog();
        }

        public void ShowEditSupplierForm(SupplierData supplier)
        {
            var editSupplierForm = new Views.Inventory.Suppliers.AddSupplierFormView();
            var viewModel = new ViewModels.Supplier.AddSupplierFormViewModel();
            editSupplierForm.DataContext = viewModel;
            editSupplierForm.Owner = Application.Current.MainWindow;
            editSupplierForm.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            editSupplierForm.ShowDialog();
        }

    }
}