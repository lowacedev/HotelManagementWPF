using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotelManagementWPF.ViewModels.Booking;

namespace HotelManagementWPF.Services
{
    public interface IWindowService
    {
        // Existing Room methods
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
        void ShowEditUserForm(ViewModels.User user);
    }
}