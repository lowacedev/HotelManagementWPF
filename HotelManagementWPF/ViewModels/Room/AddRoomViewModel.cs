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
        private readonly Action _onRoomAdded; // Callback to refresh room list
        private bool _isAdding; // To prevent multiple submissions

        public event Action CloseAction; // To close the window/dialog

        public AddRoomViewModel(RoomViewModel mainViewModel, Action onRoomAdded = null)
        {
            _mainViewModel = mainViewModel;
            _onRoomAdded = onRoomAdded;

            // Initialize bed types collection
            BedTypes = new ObservableCollection<string> { "Single", "Double", "Presidential Suite" };

            // Set default values
            RoomNumber = string.Empty;
            BedType = BedTypes[0];
            Price = 0;
            SelectedStatus = "Available";

            // Initialize command
            AddRoomCommand = new RelayCommand(async () => await ExecuteAddRoomAsync(), () => !IsAdding);
        }

        // Collection for dropdown
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

        private string _selectedStatus;
        public string SelectedStatus
        {
            get => _selectedStatus;
            set { _selectedStatus = value; OnPropertyChanged(); }
        }

        public ICommand AddRoomCommand { get; }

        public bool IsAdding
        {
            get => _isAdding;
            private set
            {
                _isAdding = value;
                OnPropertyChanged();
                (AddRoomCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        private async System.Threading.Tasks.Task ExecuteAddRoomAsync()
        {
            if (IsAdding)
                return;

            IsAdding = true;

            try
            {
                // Prepare data for insertion
                var data = new Dictionary<string, object>
                {
                    { "roomNumber", RoomNumber },
                    { "roomType", BedType },
                    { "price", Price },
                    { "roomStatus", SelectedStatus }
                };

                string sql = @"
                    INSERT INTO tbl_Room (roomNumber, roomType, price, roomStatus)
                    VALUES (@roomNumber, @roomType, @price, @roomStatus)";

                // Insert into local database
                using (var db = new DbConnections())
                {
                    db.ExecuteNonQuery(sql, data);
                }

                // Insert into online database
                using (var onlineDb = new DbConnections(connectToOnlineDb: true))
                {
                    onlineDb.InsertDataWithSync("tbl_Room", data);
                }

                MessageBox.Show("Room added successfully");
                RoomUpdateNotifier.NotifyRoomUpdated();
                // Notify main view model to refresh list
                _onRoomAdded?.Invoke();

                // Reset fields
                RoomNumber = string.Empty;
                BedType = BedTypes[0];
                Price = 0;
                SelectedStatus = "Available";

                // Close the window/dialog
                CloseAction?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
            finally
            {
                IsAdding = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // Basic RelayCommand implementation
    public class RelayCommand : ICommand
    {
        private readonly Func<System.Threading.Tasks.Task> _executeAsync;
        private readonly Func<bool> _canExecute;

        public event EventHandler CanExecuteChanged;

        public RelayCommand(Func<System.Threading.Tasks.Task> executeAsync, Func<bool> canExecute = null)
        {
            _executeAsync = executeAsync;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute();

        public async void Execute(object parameter) => await _executeAsync();

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}