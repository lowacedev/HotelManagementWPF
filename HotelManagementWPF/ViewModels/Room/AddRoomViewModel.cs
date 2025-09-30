using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using HotelManagementWPF.Models;
using DatabaseProject;

namespace HotelManagementWPF.ViewModels.Room
{
    public class AddRoomViewModel : INotifyPropertyChanged
    {
        private readonly RoomViewModel _mainViewModel;
        private readonly Action _onRoomAdded; // Callback to refresh rooms

        public event Action CloseAction;

        public AddRoomViewModel(RoomViewModel mainViewModel, Action onRoomAdded = null)
        {
            _mainViewModel = mainViewModel;
            _onRoomAdded = onRoomAdded;

            BedTypes = new ObservableCollection<string> { "Single", "Double", "Presidential Suite" };
            RoomNumber = string.Empty;
            BedType = BedTypes[0];
            Price = 0;
            // Initialize SelectedStatus with default value
            SelectedStatus = "Available";

            AddRoomCommand = new RelayCommand(ExecuteAddRoom);
        }

        public ObservableCollection<string> BedTypes { get; }

        private string _roomNumber;
        public string RoomNumber
        {
            get => _roomNumber;
            set { _roomNumber = value; OnPropertyChanged(); }
        }

        private string _bedType;
        public string BedType
        {
            get => _bedType;
            set { _bedType = value; OnPropertyChanged(); }
        }

        private decimal _price;
        public decimal Price
        {
            get => _price;
            set { _price = value; OnPropertyChanged(); }
        }

        // Property for Room Status selection
        private string _selectedStatus;
        public string SelectedStatus
        {
            get => _selectedStatus;
            set { _selectedStatus = value; OnPropertyChanged(); }
        }

        public ICommand AddRoomCommand { get; }

        private void ExecuteAddRoom()
        {
            try
            {
                var db = new DbConnections();
                string insertQuery = @"
                    INSERT INTO tbl_Room (roomNumber, roomType, price, roomStatus)
                    VALUES (@RoomNumber, @BedType, @PricePerNight, @Status)";

                var parameters = new Dictionary<string, object>
                {
                    { "@RoomNumber", RoomNumber },
                    { "@BedType", BedType },
                    { "@PricePerNight", Price },
                    { "@Status", SelectedStatus } // Use SelectedStatus here
                };

                db.ExecuteNonQuery(insertQuery, parameters);

                MessageBox.Show("Room added successfully");

                // Invoke callback to refresh rooms
                _onRoomAdded?.Invoke();

                // Clear fields
                RoomNumber = string.Empty;
                BedType = BedTypes[0];
                Price = 0;
                SelectedStatus = "Available";

                // Close the add window/dialog
                CloseAction?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}