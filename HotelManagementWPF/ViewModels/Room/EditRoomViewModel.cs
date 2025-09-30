using DatabaseProject;
using HotelManagementWPF.Models;
using HotelManagementWPF.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows;

namespace HotelManagementWPF.ViewModels
{
    public class EditRoomViewModel : INotifyPropertyChanged
    {
        private int _roomId;
        private DbConnections _db; // Removed readonly for dynamic switching

        private string _roomNumber;
        private string _bedType;
        private decimal _price;
        private RoomStatus _status;

        public List<string> BedTypeOptions { get; } = new List<string>
        {
            "Single", "Double", "Presidential Suite"
        };

        public List<RoomStatus> StatusOptions { get; } = new List<RoomStatus>
        {
            RoomStatus.Available, RoomStatus.Booked, RoomStatus.Reserved,
            RoomStatus.Waitlist, RoomStatus.Blocked
        };

        public string RoomNumber
        {
            get => _roomNumber;
            set { _roomNumber = value; OnPropertyChanged(); }
        }

        public string BedType
        {
            get => _bedType;
            set { _bedType = value; OnPropertyChanged(); }
        }

        public decimal Price
        {
            get => _price;
            set { _price = value; OnPropertyChanged(); }
        }

        public RoomStatus Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(); }
        }

        public ICommand SaveChangesCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Constructor to initialize ViewModel with database mode.
        /// </summary>
        /// <param name="roomId">ID of the room to edit</param>
        /// <param name="connectToOnlineDb">true for online, false for local</param>
        public EditRoomViewModel(int roomId, bool connectToOnlineDb = false)
        {
            _roomId = roomId;
            _db = new DbConnections(connectToOnlineDb); // Initialize with mode

            LoadRoomFromDatabase();

            SaveChangesCommand = new RelayCommand(SaveChanges);
        }

        /// <summary>
        /// Switch between online and local database dynamically.
        /// </summary>
        /// <param name="isOnline">true for online, false for local</param>
        public void SetOnlineMode(bool isOnline)
        {
            if (_db != null)
            {
                _db.Dispose(); // Dispose previous connection
            }
            _db = new DbConnections(isOnline); // Initialize new connection
            LoadRoomFromDatabase(); // Reload data from new database
        }

        private void LoadRoomFromDatabase()
        {
            var dt = new DataTable();
            string query = $"SELECT * FROM tbl_Room WHERE room_id = {_roomId}";
            _db.readDatathroughAdapter(query, dt);

            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];

                RoomNumber = row["roomNumber"].ToString();
                BedType = row["roomType"].ToString();
                Price = Convert.ToDecimal(row["price"]);
                Status = (RoomStatus)Enum.Parse(typeof(RoomStatus), row["roomStatus"].ToString());
            }
            else
            {
                MessageBox.Show("Room not found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void SaveChanges()
        {
            try
            {
                string updateQuery = @"
                UPDATE tbl_Room SET
                    roomNumber = @RoomNumber,
                    roomType = @BedType,
                    price = @Price,
                    roomStatus = @Status
                WHERE room_id = @RoomId";

                var parameters = new Dictionary<string, object>
                {
                    { "@RoomNumber", RoomNumber },
                    { "@BedType", BedType },
                    { "@Price", Price },
                    { "@Status", Status.ToString() },
                    { "@RoomId", _roomId }
                };

                // Log for debugging
                System.Diagnostics.Debug.WriteLine("Executing SQL: " + updateQuery);
                foreach (var param in parameters)
                {
                    System.Diagnostics.Debug.WriteLine($"{param.Key} = {param.Value}");
                }

                _db.ExecuteNonQuery(updateQuery, parameters);

                // Notify other ViewModels that a room was updated
                RoomUpdateNotifier.NotifyRoomUpdated();

                MessageBox.Show($"Room {RoomNumber} updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // Close the window
                if (Application.Current.MainWindow is Window window)
                {
                    foreach (Window w in Application.Current.Windows)
                    {
                        if (w is Views.Room.EditRoomFormView && w.DataContext == this)
                        {
                            w.DialogResult = true;
                            w.Close();
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in SaveChanges: " + ex.Message);
                MessageBox.Show($"Error updating: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    // Static notifier class inside same file
    public static class RoomUpdateNotifier
    {
        public static event Action RoomUpdated;

        public static void NotifyRoomUpdated()
        {
            RoomUpdated?.Invoke();
        }
    }
}