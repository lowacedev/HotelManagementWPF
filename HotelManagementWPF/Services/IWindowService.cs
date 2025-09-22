using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotelManagementWPF.ViewModels.Booking;
using HotelManagementWPF.ViewModels;
using HotelManagementWPF.Views;

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
        void ShowEditBookingForm(BookingData booking);

        // Guest methods
        void ShowAddGuestForm();

        // User methods
        void ShowAddUserForm();
        void ShowEditUserForm(User user);
    }
}