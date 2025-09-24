using DatabaseProject; // your DbConnections namespace
using HotelManagementWPF.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace HotelManagementWPF.ViewModels
{
    public class EditRoomViewModel : INotifyPropertyChanged
    {
        private readonly int _roomId; // room id to load
        private Room _originalRoom;
        private readonly DbConnections _db;

        private string _roomNumber;
        private string _bedType;
        private decimal _price;
        private RoomStatus _status;
        private Room room;

        public EditRoomViewModel(int roomId)
        {
            _roomId = roomId;
            _db = new DbConnections();

            LoadRoomFromDatabase();

            SaveChangesCommand = new RelayCommand(SaveChanges);
        }


        private void LoadRoomFromDatabase()
        {
            var dt = new DataTable();
            string query = $"SELECT * FROM tbl_Room WHERE room_id = {_roomId}";
            _db.readDatathroughAdapter(query, dt);

            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                _originalRoom = new Room
                {
                    RoomId = (int)row["room_id"], // no trailing space
                    RoomNumber = row["roomNumber"].ToString(),
                    BedType = row["roomType"].ToString(),
                    Price = Convert.ToDecimal(row["price"]),
                    Status = (RoomStatus)Enum.Parse(typeof(RoomStatus), row["roomStatus"].ToString()),
                    ModifiedDate = DateTime.Now
                };

                // Populate properties for binding
                RoomNumber = _originalRoom.RoomNumber;
                BedType = _originalRoom.BedType;
                Price = _originalRoom.Price;
                Status = _originalRoom.Status;
            }
            else
            {
                System.Windows.MessageBox.Show("Room not found!", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        // Properties with INotifyPropertyChanged
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

        public List<string> BedTypeOptions => new List<string>
        {
            "Single", "Double", "Queen", "King", "Twin", "Suite", "Deluxe", "Presidential Suite"
        };

        public List<RoomStatus> StatusOptions => new List<RoomStatus>
        {
            RoomStatus.Available, RoomStatus.Booked, RoomStatus.Reserved,
            RoomStatus.Waitlist, RoomStatus.Blocked
        };

        public ICommand SaveChangesCommand { get; }

        private void SaveChanges()
        {
            try
            {
                // Update the database record
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

                _db.ExecuteNonQuery(updateQuery, parameters);

                System.Windows.MessageBox.Show($"Room {RoomNumber} updated successfully!", "Success", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);


                // Close the window
                if (System.Windows.Application.Current.MainWindow is System.Windows.Window window)
                {
                    foreach (System.Windows.Window w in System.Windows.Application.Current.Windows)
                    {
                        if (w is HotelManagementWPF.Views.Room.EditRoomFormView && w.DataContext == this)
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
                System.Windows.MessageBox.Show($"Error updating: {ex.Message}", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}