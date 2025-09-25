using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementWPF.Models
{
    public class BookingData
    {
        public int Id { get; set; } // booking_id
        public int GuestId { get; set; }
        public int RoomId { get; set; }
        public int UserId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int NumberOfGuest { get; set; }
        public decimal TotalPaid { get; set; }
        public byte[] Status { get; set; } // Assuming varbinary(50)

        // Additional display properties
        public string Guest { get; set; }
        public string RoomType { get; set; }
        public string RoomNumber { get; set; }
        public string User { get; set; }
        public string StatusText { get; set; } // "Check-in" or "Check-out"

        public string CheckInFormatted => CheckIn.ToString("MMM dd, yyyy");
        public string CheckOutFormatted => CheckOut.ToString("MMM dd, yyyy");
    }
}
