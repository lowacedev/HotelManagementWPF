using DatabaseProject;
using System;
using System.Collections.Generic;
using System.Data;

namespace HotelManagementWPF.Models
{
    public class BookingData
    {
        public int Id { get; set; }
        public int GuestId { get; set; }
        public int RoomId { get; set; }
        public int UserId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int NumberOfGuest { get; set; }
        public decimal TotalPaid { get; set; }
        public string Status { get; set; } // Original status as string (e.g., "Check-In")

        // Additional display properties
        public string Guest { get; set; }
        public string RoomType { get; set; }
        public string RoomNumber { get; set; }
        public string User { get; set; }
        public string StatusText { get; set; }

        // Formatted date strings
        public string CheckInFormatted => CheckIn.ToString("MMM dd, yyyy");
        public string CheckOutFormatted => CheckOut.ToString("MMM dd, yyyy");

        // Computed property: number of days booked
        public int NumberOfDays => (CheckOut - CheckIn).Days;

        // Computed property: total amount (Room Price * Number of Days)
        public decimal TotalAmount => TotalPaid; // Assuming TotalPaid is already calculated as RoomPrice * Days

        // Display property for TotalPaid
        public string TotalAmountDisplay => $"₱{TotalPaid:N2}";

        // Helper method to safely get data from DataRow
        private static T GetSafeValue<T>(DataRow row, string columnName, T defaultValue = default)
        {
            return row[columnName] != DBNull.Value ? (T)row[columnName] : defaultValue;
        }

        // Method to fetch all bookings from database
        public static List<BookingData> GetAllBookings()
        {
            using (var db = new DbConnections())
            {
                string query = @"
            SELECT 
                b.booking_id AS Id,
                b.guest_id AS GuestId,
                b.room_id AS RoomId,
                b.user_id AS UserId,
                b.check_in AS CheckIn,
                b.check_out AS CheckOut,
                b.numberOfGuest AS NumberOfGuest,
                b.totalPaid AS TotalPaid,
                b.Status AS Status,
                g.name AS GuestName,
                r.roomType AS RoomType,
                r.roomNumber AS RoomNumber,
                u.username AS UserName
            FROM tbl_Booking b
            LEFT JOIN tbl_Guest g ON b.guest_id = g.guest_id
            LEFT JOIN tbl_Room r ON b.room_id = r.room_id
            LEFT JOIN tbl_User u ON b.user_id = u.user_id";

                DataTable dt = new DataTable();
                db.readDatathroughAdapter(query, dt);
                var bookings = new List<BookingData>();

                foreach (DataRow row in dt.Rows)
                {
                    // Read Status as string
                    string statusStr = row["Status"] != DBNull.Value ? row["Status"].ToString() : "Unknown";

                    // Optionally, you might want to parse or process status further

                    bookings.Add(new BookingData
                    {
                        Id = GetSafeValue<int>(row, "Id"),
                        GuestId = GetSafeValue<int>(row, "GuestId"),
                        Guest = row["GuestName"] != DBNull.Value ? row["GuestName"].ToString() : "Unknown",
                        RoomId = GetSafeValue<int>(row, "RoomId"),
                        RoomType = row["RoomType"] != DBNull.Value ? row["RoomType"].ToString() : "Unknown",
                        RoomNumber = row["RoomNumber"] != DBNull.Value ? row["RoomNumber"].ToString() : "Unknown",
                        UserId = GetSafeValue<int>(row, "UserId"),
                        CheckIn = GetSafeValue<DateTime>(row, "CheckIn"),
                        CheckOut = GetSafeValue<DateTime>(row, "CheckOut"),
                        NumberOfGuest = GetSafeValue<int>(row, "NumberOfGuest"),
                        TotalPaid = GetSafeValue<decimal>(row, "TotalPaid"),
                        Status = statusStr,
                        StatusText = statusStr // or process further if needed
                    });
                }

                return bookings;
            }
        }
    }
}