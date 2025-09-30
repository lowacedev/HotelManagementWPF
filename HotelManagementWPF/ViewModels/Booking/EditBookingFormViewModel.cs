using DatabaseProject;
using HotelManagementWPF.Data;
using HotelManagementWPF.ViewModels.Base;
using System;
using System.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using HotelManagementWPF.Models;

namespace HotelManagementWPF.ViewModels.Booking
{
    public class EditBookingFormViewModel : INotifyPropertyChanged
    {
        private int _bookingId;
        private int _guestId;
        private int _roomId;

        // Guest Information
        private string _fullName = string.Empty;
        public string FullName
        {
            get => _fullName;
            set { _fullName = value; OnPropertyChanged(); }
        }

        private string _phoneNumber = string.Empty;
        public string PhoneNumber
        {
            get => _phoneNumber;
            set { _phoneNumber = value; OnPropertyChanged(); }
        }

    

        private int? _age;
        public int? Age
        {
            get => _age;
            set { _age = value; OnPropertyChanged(); }
        }

        private string _gender = "Male";
        public string Gender
        {
            get => _gender;
            set { _gender = value; OnPropertyChanged(); }
        }
        public ObservableCollection<string> GenderOptions { get; } = new() { "Male", "Female" };

        // Payment Details
        private string _paymentMethod = "Cash";
        public string PaymentMethod
        {
            get => _paymentMethod;
            set { _paymentMethod = value; OnPropertyChanged(); }
        }
        public ObservableCollection<string> PaymentMethodOptions { get; } = new() { "Cash", "Credit Card", "Online Payment" };

        private decimal _advancedPayment = 0;
        public decimal AdvancedPayment
        {
            get => _advancedPayment;
            set { _advancedPayment = value; OnPropertyChanged(); }
        }

        private decimal _totalAmount;
        public decimal TotalAmount
        {
            get => _totalAmount;
            set { _totalAmount = value; OnPropertyChanged(); }
        }

        private string _Status = "Check-In";
        public string status
        {
            get => _Status;
            set { _Status = value; OnPropertyChanged(); }
        }

        public ObservableCollection<string> Status { get; } = new() { "Check-In", "Check-Out", "Reservation" };

        // Stay Details
        private string _roomNumber = string.Empty;
        public string RoomNumber
        {
            get => _roomNumber;
            set
            {
                if (_roomNumber != value)
                {
                    _roomNumber = value;
                    OnPropertyChanged();
                    _ = FetchRoomPriceAsync(_roomNumber);
                    CalculateTotalAmount();
                }
            }
        }

        private string _roomType = string.Empty;
        public string RoomType
        {
            get => _roomType;
            set
            {
                if (_roomType != value)
                {
                    _roomType = value;
                    OnPropertyChanged();
                    _ = UpdateAvailableRooms();
                }
            }
        }
        public ObservableCollection<string> RoomTypeOptions { get; } = new() { "Single", "Double", "Presidential Suite" };

        private ObservableCollection<string> _roomNumberOptions = new();
        public ObservableCollection<string> RoomNumberOptions
        {
            get => _roomNumberOptions;
            private set { _roomNumberOptions = value; OnPropertyChanged(); }
        }

        private decimal _roomPrice;
        public decimal RoomPrice
        {
            get => _roomPrice;
            set
            {
                if (_roomPrice != value)
                {
                    _roomPrice = value;
                    OnPropertyChanged();
                    CalculateTotalAmount();
                }
            }
        }

        private int _numberOfGuests = 1;
        public int NumberOfGuests
        {
            get => _numberOfGuests;
            set
            {
                if (_numberOfGuests != value)
                {
                    _numberOfGuests = value;
                    OnPropertyChanged();
                    CalculateTotalAmount();
                }
            }
        }

        private DateTime _checkInDate = DateTime.Today;
        public DateTime CheckInDate
        {
            get => _checkInDate;
            set
            {
                if (_checkInDate != value)
                {
                    _checkInDate = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(NumberOfNights));
                    CalculateTotalAmount();
                }
            }
        }

        private DateTime _checkOutDate = DateTime.Today.AddDays(1);
        public DateTime CheckOutDate
        {
            get => _checkOutDate;
            set
            {
                if (_checkOutDate != value)
                {
                    _checkOutDate = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(NumberOfNights));
                    CalculateTotalAmount();
                }
            }
        }

        public int NumberOfNights => (CheckOutDate > CheckInDate) ? (CheckOutDate - CheckInDate).Days : 0;

        // Commands
        public ICommand CancelCommand { get; set; }
        public ICommand UpdateBookingCommand { get; set; }

        public EditBookingFormViewModel(int bookingId)
        {
            _bookingId = bookingId;

            CancelCommand = new RelayCommand(() => OnCancel());
            UpdateBookingCommand = new RelayCommand(async () => await UpdateBookingAsync());

            // Load existing booking data
            _ = LoadBookingDataAsync();
        }

        private async Task LoadBookingDataAsync()
        {
            try
            {
                string query = @"
                    SELECT 
                        b.booking_id, b.room_id, b.guest_id, b.check_in, b.check_out, 
                        b.numberOfGuest, b.totalAmount, b.totalPaid, b.Status,
                        g.name, g.age, g.gender, g.phoneNumber,
                        r.roomNumber, r.roomType, r.price
                    FROM tbl_Booking b
                    INNER JOIN tbl_Guest g ON b.guest_id = g.guest_id
                    INNER JOIN tbl_Room r ON b.room_id = r.room_id
                    WHERE b.booking_id = @BookingId";

                var parameters = new Dictionary<string, object> { { "@BookingId", _bookingId } };
                DataTable dt = await ExecuteQueryAsync(query, parameters);

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];

                    // Store IDs
                    _guestId = Convert.ToInt32(row["guest_id"]);
                    _roomId = Convert.ToInt32(row["room_id"]);

                    // Guest Information
                    FullName = row["name"].ToString();
                    Age = row["age"] != DBNull.Value ? Convert.ToInt32(row["age"]) : (int?)null;
                    Gender = row["gender"].ToString();
                    PhoneNumber = row["phoneNumber"].ToString();

                    // Stay Details
                    RoomType = row["roomType"].ToString();
                    RoomNumber = row["roomNumber"].ToString();
                    RoomPrice = Convert.ToDecimal(row["price"]);
                    NumberOfGuests = Convert.ToInt32(row["numberOfGuest"]);
                    CheckInDate = Convert.ToDateTime(row["check_in"]);
                    CheckOutDate = Convert.ToDateTime(row["check_out"]);

                    // Payment Details
                    TotalAmount = Convert.ToDecimal(row["totalAmount"]);
                    AdvancedPayment = Convert.ToDecimal(row["totalPaid"]);
                    status = row["Status"].ToString();

                    // Load available rooms for the selected type
                    await UpdateAvailableRooms();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading booking data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task<DataTable> ExecuteQueryAsync(string query, Dictionary<string, object> parameters = null)
        {
            return await Task.Run(() =>
            {
                var dt = new DataTable();
                using (var db = new DbConnections())
                {
                    if (parameters != null)
                        db.readDataWithParameters(query, dt, parameters);
                    else
                        db.readDatathroughAdapter(query, dt);
                }
                return dt;
            });
        }

        private async Task UpdateAvailableRooms()
        {
            if (string.IsNullOrEmpty(RoomType))
            {
                RoomNumberOptions.Clear();
                return;
            }

            var availableRooms = await FetchAvailableRoomsAsync(CheckInDate, CheckOutDate, RoomType);

            // Include current room in the list
            if (!availableRooms.Contains(RoomNumber))
            {
                availableRooms.Insert(0, RoomNumber);
            }

            RoomNumberOptions = new ObservableCollection<string>(availableRooms);
        }

        private async Task<ObservableCollection<string>> FetchAvailableRoomsAsync(DateTime checkIn, DateTime checkOut, string roomType)
        {
            var rooms = new ObservableCollection<string>();
            string query = @"
                SELECT roomNumber FROM tbl_Room 
                WHERE roomType = @RoomType 
                  AND roomStatus = 'Available' 
                  AND room_id NOT IN (
                      SELECT r.room_id FROM tbl_Room r
                      INNER JOIN tbl_Booking b ON r.room_id = b.room_id
                      WHERE b.booking_id != @BookingId
                        AND (@CheckIn BETWEEN b.check_in AND b.check_out OR
                             @CheckOut BETWEEN b.check_in AND b.check_out OR
                             b.check_in BETWEEN @CheckIn AND @CheckOut)
                  );";

            var parameters = new Dictionary<string, object>
            {
                { "@RoomType", roomType },
                { "@CheckIn", checkIn },
                { "@CheckOut", checkOut },
                { "@BookingId", _bookingId }
            };

            DataTable dt = await ExecuteQueryAsync(query, parameters);
            foreach (DataRow row in dt.Rows)
                rooms.Add(row["roomNumber"].ToString());

            return rooms;
        }

        private async Task FetchRoomPriceAsync(string roomNumber)
        {
            if (string.IsNullOrEmpty(roomNumber))
            {
                RoomPrice = 0;
                return;
            }

            string query = "SELECT price FROM tbl_Room WHERE roomNumber = @RoomNumber";
            var parameters = new Dictionary<string, object> { { "@RoomNumber", roomNumber } };
            DataTable dt = await ExecuteQueryAsync(query, parameters);
            if (dt.Rows.Count > 0)
                RoomPrice = Convert.ToDecimal(dt.Rows[0]["price"]);
            else
                RoomPrice = 0;
        }

        private void CalculateTotalAmount()
        {
            int nights = NumberOfNights;
            if (nights < 1)
                nights = 1;
            TotalAmount = nights * RoomPrice;
        }

        private void OnCancel()
        {
            CloseWindow();
        }

        public async Task UpdateBookingAsync()
        {
            try
            {
                if (Session.CurrentUserId <= 0)
                {
                    MessageBox.Show("Please log in before updating.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var db = new DbConnections();

                // Update guest information
                string updateGuestQuery = @"
                    UPDATE tbl_Guest 
                    SET name = @Name, age = @Age, gender = @Gender, 
                        phoneNumber = @PhoneNumber, totalAmount = @TotalAmount, totalPaid = @TotalPaid
                    WHERE guest_id = @GuestId";

                var guestParams = new Dictionary<string, object>
                {
                    { "@Name", FullName },
                    { "@Age", Age ?? 0 },
                    { "@Gender", Gender },
                    { "@PhoneNumber", PhoneNumber },
                    { "@TotalAmount", TotalAmount },
                    { "@TotalPaid", AdvancedPayment },
                    { "@GuestId", _guestId }
                };

                await db.ExecuteNonQueryAsync(updateGuestQuery, guestParams);

                // Get new room id if room changed
                int newRoomId = _roomId;
                if (!string.IsNullOrEmpty(RoomNumber))
                {
                    string getRoomIdQuery = "SELECT room_id FROM tbl_Room WHERE roomNumber = @RoomNumber;";
                    var roomIdParams = new Dictionary<string, object> { { "@RoomNumber", RoomNumber } };
                    object roomIdResult = await db.ExecuteScalarAsync(getRoomIdQuery, roomIdParams);
                    newRoomId = roomIdResult != null && int.TryParse(roomIdResult.ToString(), out var rId) ? rId : _roomId;
                }

                // Update booking
                string updateBookingQuery = @"
                    UPDATE tbl_Booking 
                    SET room_id = @RoomId, check_in = @CheckIn, check_out = @CheckOut, 
                        numberOfGuest = @NumberOfGuest, totalAmount = @TotalAmount, 
                        totalPaid = @TotalPaid, Status = @Status
                    WHERE booking_id = @BookingId";

                var bookingParams = new Dictionary<string, object>
                {
                    { "@RoomId", newRoomId },
                    { "@CheckIn", CheckInDate },
                    { "@CheckOut", CheckOutDate },
                    { "@NumberOfGuest", NumberOfGuests },
                    { "@TotalAmount", TotalAmount },
                    { "@TotalPaid", AdvancedPayment },
                    { "@Status", status },
                    { "@BookingId", _bookingId }
                };

                await db.ExecuteNonQueryAsync(updateBookingQuery, bookingParams);

                // Update room status if room changed
                if (newRoomId != _roomId)
                {
                    // Set old room back to Available
                    string updateOldRoomQuery = "UPDATE tbl_Room SET roomStatus = 'Available' WHERE room_id = @RoomId;";
                    await db.ExecuteNonQueryAsync(updateOldRoomQuery, new Dictionary<string, object> { { "@RoomId", _roomId } });

                    // Set new room to Booked
                    string updateNewRoomQuery = "UPDATE tbl_Room SET roomStatus = 'Booked' WHERE room_id = @RoomId;";
                    await db.ExecuteNonQueryAsync(updateNewRoomQuery, new Dictionary<string, object> { { "@RoomId", newRoomId } });
                }

                MessageBox.Show("Booking updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                CloseWindow();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseWindow()
        {
            Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive)?.Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}