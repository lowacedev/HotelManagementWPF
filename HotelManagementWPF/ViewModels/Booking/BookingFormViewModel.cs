using DatabaseProject;
using HotelManagementWPF.Data;
using HotelManagementWPF.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HotelManagementWPF.ViewModels.Booking
{
    public class BookingFormViewModel : INotifyPropertyChanged
    {
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

        private string _email = string.Empty;
        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
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
        public ObservableCollection<string> GenderOptions { get; } = new() { "Male", "Female", "Other" };

        // Payment Details
        private string _paymentMethod = "Cash";
        public string PaymentMethod
        {
            get => _paymentMethod;
            set { _paymentMethod = value; OnPropertyChanged(); }
        }
        public ObservableCollection<string> PaymentMethodOptions { get; } = new() { "Cash", "Credit Card", "Bank Transfer", "Online Payment" };

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

        private string _paymentStatus = "Pending";
        public string PaymentStatus
        {
            get => _paymentStatus;
            set { _paymentStatus = value; OnPropertyChanged(); }
        }
        public ObservableCollection<string> PaymentStatusOptions { get; } = new() { "Pending", "Partial", "Completed" };

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
                    _ = UpdateAvailableRooms(); // Trigger room list update when room type changes
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

        private string _selectedRoomNumber;
        public string SelectedRoomNumber
        {
            get => _selectedRoomNumber;
            set
            {
                if (_selectedRoomNumber != value)
                {
                    _selectedRoomNumber = value;
                    OnPropertyChanged();
                    _ = FetchRoomPriceAsync(_selectedRoomNumber);
                    CalculateTotalAmount();
                }
            }
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

        public DateTime CheckInDate { get; set; } = DateTime.Today;
        public DateTime CheckOutDate { get; set; } = DateTime.Today.AddDays(1);
        public int NumberOfNights => (CheckOutDate > CheckInDate) ? (CheckOutDate - CheckInDate).Days : 0;

        // Commands
        public ICommand CancelCommand { get; set; }
        public ICommand BookRoomCommand { get; set; }

        public BookingFormViewModel()
        {
            CancelCommand = new RelayCommand(() => OnCancel());
            BookRoomCommand = new RelayCommand(async () => await SaveBookingAsync());

            // Initialize
            CheckInDate = DateTime.Today;
            CheckOutDate = DateTime.Today.AddDays(1);
            _ = UpdateAvailableRooms();

            // Listen for changes to update available rooms
            PropertyChanged += async (s, e) =>
            {
                if (e.PropertyName == nameof(CheckInDate) || e.PropertyName == nameof(CheckOutDate) || e.PropertyName == nameof(RoomType))
                {
                    await UpdateAvailableRooms();
                }
            };
        }

        // Helper method to execute query asynchronously using Task.Run
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

        // Fetch available rooms based on dates and type
        private async Task UpdateAvailableRooms()
        {
            if (string.IsNullOrEmpty(RoomType))
            {
                RoomNumberOptions.Clear();
                return;
            }

            var availableRooms = await FetchAvailableRoomsAsync(CheckInDate, CheckOutDate, RoomType);
            // Update the options list
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
                      WHERE 
                          (@CheckIn BETWEEN b.check_in AND b.check_out OR
                           @CheckOut BETWEEN b.check_in AND b.check_out OR
                           b.check_in BETWEEN @CheckIn AND @CheckOut)
                  );";

            var parameters = new Dictionary<string, object>
            {
                { "@RoomType", roomType },
                { "@CheckIn", checkIn },
                { "@CheckOut", checkOut }
            };

            DataTable dt = await ExecuteQueryAsync(query, parameters);
            foreach (DataRow row in dt.Rows)
                rooms.Add(row["roomNumber"].ToString());

            return rooms;
        }

        // Fetch room price based on selected room number
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

        // Calculate total amount based on nights and room price
        private void CalculateTotalAmount()
        {
            int nights = NumberOfNights;
            if (nights < 1)
                nights = 1;
            TotalAmount = nights * RoomPrice;
        }

        private void OnCancel()
        {
            // Implement dialog close or reset logic here
        }

        private async Task SaveBookingAsync()
        {
            var db = new DbConnections();

            // Insert guest info
            string insertGuestQuery = @"
                INSERT INTO tbl_Guest (name, age, gender, phoneNumber, totalAmount, totalPaid)
                VALUES (@Name, @Age, @Gender, @PhoneNumber, @TotalAmount, @TotalPaid);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            var guestParams = new Dictionary<string, object>
            {
                { "@Name", FullName },
                { "@Age", Age ?? 0 },
                { "@Gender", Gender },
                { "@PhoneNumber", PhoneNumber },
                { "@TotalAmount", TotalAmount },
                { "@TotalPaid", AdvancedPayment }
            };

            int guestId = 0;
            object result = await db.ExecuteScalarAsync(insertGuestQuery, guestParams);
            if (result != null && int.TryParse(result.ToString(), out var id))
            {
                guestId = id;
            }

            // Get RoomId
            string getRoomQuery = "SELECT room_id FROM tbl_Room WHERE roomNumber = @RoomNumber;";
            var roomParams = new Dictionary<string, object> { { "@RoomNumber", RoomNumber } };
            object roomResult = await db.ExecuteScalarAsync(getRoomQuery, roomParams);
            int roomId = 0;
            if (roomResult != null && int.TryParse(roomResult.ToString(), out var rId))
            {
                roomId = rId;
            }
            else
            {
                throw new Exception("Room not found");
            }

            // Insert booking
            string insertBookingQuery = @"
                INSERT INTO tbl_Booking (room_id, guest_id, user_id, check_in, check_out, numberOfGuest, totalPaid, Status)
                VALUES (@RoomId, @GuestId, @UserId, @CheckIn, @CheckOut, @NumberOfGuest, @TotalPaid, @Status);";

            var bookingParams = new Dictionary<string, object>
            {
                { "@RoomId", roomId },
                { "@GuestId", guestId },
                { "@UserId", User.CurrentUserId },
                { "@CheckIn", CheckInDate },
                { "@CheckOut", CheckOutDate },
                { "@NumberOfGuest", 1 },
                { "@TotalPaid", AdvancedPayment },
                { "@Status", "Pending" }
            };
            await db.ExecuteNonQueryAsync(insertBookingQuery, bookingParams);

            // Update room status
            string updateRoomStatusQuery = "UPDATE tbl_Room SET roomStatus = 'Booked' WHERE room_id = @RoomId;";
            await db.ExecuteNonQueryAsync(updateRoomStatusQuery, new Dictionary<string, object> { { "@RoomId", roomId } });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}