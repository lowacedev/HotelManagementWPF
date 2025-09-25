using HotelManagementWPF.ViewModels;
using HotelManagementWPF.ViewModels.Booking;
using HotelManagementWPF.ViewModels.Supplier;
using HotelManagementWPF.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementWPF.Services
{
    public interface IWindowService
    {
        // Room methods
        void ShowAddRoomDialog();
        void ShowAddRoomForm();
        void ShowEditRoomForm(Models.Room room);

        // Booking methods
        void ShowAddBookingForm();
        void ShowEditBookingForm(Models.BookingData booking);

        // Guest methods
        void ShowAddGuestForm();

        // User methods
        void ShowAddUserForm();
        void ShowEditUserForm(User user);

        // Supplier methods  
        void ShowAddSupplierForm();
        void ShowEditSupplierForm(SupplierData supplier);
    }
}