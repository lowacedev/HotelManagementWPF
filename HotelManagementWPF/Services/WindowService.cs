using HotelManagementWPF.ViewModels;
using HotelManagementWPF.ViewModels.Guest;
using HotelManagementWPF.ViewModels.Booking;
using HotelManagementWPF.ViewModels.Supplier;
using HotelManagementWPF.ViewModels.Users;
using HotelManagementWPF.Views;
using HotelManagementWPF.Views.Booking;
using HotelManagementWPF.Views.Guest;
using HotelManagementWPF.Views.Room;
using HotelManagementWPF.Views.Users;
using HotelManagementWPF.Views.Inventory.Suppliers;
using HotelManagementWPF.Models;
using System.Windows;

namespace HotelManagementWPF.Services
{
    public class WindowService : IWindowService
    {
        // Room methods
        public void ShowAddRoomDialog()
        {
            var mainWindow = Application.Current.MainWindow;
            var mainViewModel = mainWindow.DataContext as RoomViewModel;

            // Define the callback to refresh the room list after adding a new room
            Action onRoomAdded = () => mainViewModel?.LoadRooms();

            // Pass both parameters to the constructor
            var addRoomDialog = new AddRoomFormView(mainViewModel, onRoomAdded);
            addRoomDialog.Owner = mainWindow;
            addRoomDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            addRoomDialog.ShowDialog();
        }

        public void ShowAddRoomForm()
        {
            var mainWindow = Application.Current.MainWindow;
            var mainViewModel = mainWindow.DataContext as RoomViewModel;

            // Define the callback to refresh rooms
            Action refreshAction = () => mainViewModel?.LoadRooms();

            // Pass the callback to the constructor
            var addRoomForm = new AddRoomFormView(mainViewModel, refreshAction);
            addRoomForm.Owner = mainWindow;
            addRoomForm.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            addRoomForm.ShowDialog();
        }

        public void ShowEditRoomForm(Room room)
        {
            var mainWindow = Application.Current.MainWindow;
            var mainViewModel = mainWindow.DataContext as RoomViewModel;

            var editForm = new Views.Room.EditRoomFormView(room.Id);

            // Set the startup location to center of the screen
            editForm.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            var result = editForm.ShowDialog();

            if (result == true)
            {
                // Refresh the room list
                  mainViewModel?.LoadRooms();
            }
        }


        // Booking methods
        public void ShowAddBookingForm()
        {
            var mainWindow = Application.Current.MainWindow;
            var form = new BookingFormView();
            var vm = new BookingFormViewModel();
            form.DataContext = vm;
            form.Owner = mainWindow; // Set owner here
            form.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            form.ShowDialog();
        }

        public void ShowEditBookingForm(HotelManagementWPF.Models.BookingData booking)
        {
            if (booking == null) return;
            var mainWindow = Application.Current.MainWindow;

            var form = new BookingFormView();
            var vm = new BookingFormViewModel
            {
                FullName = booking.Guest,
                RoomNumber = booking.RoomNumber,
                CheckInDate = booking.CheckIn,
                CheckOutDate = booking.CheckOut
            };

            form.DataContext = vm;
            form.Owner = mainWindow; // Set owner here
            form.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            form.ShowDialog();
        }

        // Guest methods
        public void ShowAddGuestForm()
        {
            var mainWindow = Application.Current.MainWindow;
            var addGuestForm = new AddGuestFormView();
            addGuestForm.Owner = mainWindow; // Set owner here
            addGuestForm.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            addGuestForm.ShowDialog();
        }

        public void ShowEditGuestForm(GuestModel guest)
        {
            if (guest == null) return;
            var mainWindow = Application.Current.MainWindow;
            var editForm = new EditGuestFormView(guest);
            editForm.Owner = mainWindow; // Set owner here
            editForm.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            var result = editForm.ShowDialog();
            // Handle result if needed
        }

        // User methods
        public void ShowAddUserForm()
        {
            var mainWindow = Application.Current.MainWindow;
            var vm = new AddUserFormViewModel();
            var addUserForm = new AddUserFormView();
            addUserForm.DataContext = vm;
            addUserForm.Owner = mainWindow; // Set owner here
            addUserForm.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            addUserForm.ShowDialog();
        }

        public void ShowEditUserForm(ViewModels.User user)
        {
            if (user == null) return;
            var mainWindow = Application.Current.MainWindow;
            var vm = new EditUserFormViewModel(user);
            var editForm = new EditUserFormView();
            editForm.DataContext = vm;
            editForm.Owner = mainWindow; // Set owner here
            editForm.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            editForm.ShowDialog();
        }

        // Inventory Suppliers
        public void ShowAddSupplierForm()
        {
            var mainWindow = Application.Current.MainWindow;
            var addSupplierForm = new Views.Inventory.Suppliers.AddSupplierFormView();
            addSupplierForm.Owner = mainWindow;
            addSupplierForm.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            addSupplierForm.ShowDialog();
        }

        public void ShowEditSupplierForm(Supplier supplier)
        {
            var mainWindow = Application.Current.MainWindow;
            var editSupplierForm = new Views.Inventory.Suppliers.EditSupplierFormView(supplier);
            var viewModel = new ViewModels.Supplier.AddSupplierFormViewModel();
            editSupplierForm.DataContext = viewModel;
            editSupplierForm.Owner = mainWindow;
            editSupplierForm.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            editSupplierForm.ShowDialog();
        }

        // Employee
        public void ShowAddEmployeeForm()
        {
            var mainWindow = Application.Current.MainWindow;
            var addEmployeeForm = new Views.Employees.AddEmployeeFormView();
            addEmployeeForm.Owner = mainWindow;
            addEmployeeForm.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            addEmployeeForm.ShowDialog();
        }

        public void ShowEditEmployeeForm(Models.EmployeeModel employee)
        {
            if (employee == null) return;
            var mainWindow = Application.Current.MainWindow;
            var editForm = new Views.Employees.EditEmployeeFormView(employee.Id);
            editForm.Owner = mainWindow; // Set owner here
            editForm.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            var result = editForm.ShowDialog();
            // handle result if necessary
        }
    }
}