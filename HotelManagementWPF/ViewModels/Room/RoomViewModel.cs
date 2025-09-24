using DatabaseProject;
using HotelManagementWPF.Models;
using HotelManagementWPF.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace HotelManagementWPF.ViewModels
{
    public class RoomViewModel : INotifyPropertyChanged
    {
        private readonly DbConnections _db = new();
        private ObservableCollection<Room> _allRooms = new();
        private ObservableCollection<Room> _paginatedRooms = new();
        private RoomStatus? _selectedFilter;
        private int _currentPage = 1;
        private readonly int _pageSize = 10;

        public RoomViewModel(IWindowService windowService)
        {
            LoadRoomsCommand = new RelayCommand(() => LoadRooms());
            NextPageCommand = new RelayCommand(() => NextPage(), () => CanGoNext());
            PreviousPageCommand = new RelayCommand(() => PreviousPage(), () => CanGoPrevious());
            GoToPageCommand = new RelayCommand<int>(page => GoToPage(page));
            FilterCommand = new RelayCommand<string>(param => FilterRooms(param));
            AddRoomCommand = new RelayCommand(() => windowService.ShowAddRoomForm());
            EditRoomCommand = new RelayCommand<Room>(room => windowService.ShowEditRoomForm(room));

            LoadRooms();
        }

        public ObservableCollection<Room> PaginatedRooms
        {
            get => _paginatedRooms;
            set { _paginatedRooms = value; OnPropertyChanged(); }
        }

        public ICommand LoadRoomsCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand GoToPageCommand { get; }
        public ICommand FilterCommand { get; }
        public ICommand AddRoomCommand { get; }
        public ICommand EditRoomCommand { get; }

        public int TotalRooms => _allRooms.Count;
        public int AvailableRooms => _allRooms.Count(r => r.Status == RoomStatus.Available);
        public int BookedRooms => _allRooms.Count(r => r.Status == RoomStatus.Booked);
        public int TotalPages => (int)Math.Ceiling((double)FilteredRooms.Count / _pageSize);
        public List<int> PageNumbers => Enumerable.Range(1, TotalPages).ToList();

        private List<Room> FilteredRooms =>
            _selectedFilter == null ? _allRooms.ToList() : _allRooms.Where(r => r.Status == _selectedFilter).ToList();

        public void LoadRooms()
        {
            var dt = new DataTable();
            string query = "SELECT room_id, roomNumber, roomType, price, roomStatus FROM tbl_Room"; // Adjust table/columns if needed
            try
            {
                _db.readDatathroughAdapter(query, dt);
                _allRooms.Clear();

                foreach (DataRow row in dt.Rows)
                {
                    var room = new Room
                    {
                        Id = Convert.ToInt32(row["room_id"]),
                        RoomNumber = "#" + row["roomNumber"].ToString(),
                        BedType = row["roomType"].ToString(),
                        PricePerNight = Convert.ToDecimal(row["price"]),
                        Status = ParseStatus(row["roomStatus"].ToString())
                    };
                    _allRooms.Add(room);
                }

                // Set the current page to the last page to show the latest data
                _currentPage = TotalPages > 0 ? TotalPages : 1;
                UpdatePagination();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading rooms: {ex.Message}");
            }
        }

        private RoomStatus ParseStatus(string statusStr)
        {
            return statusStr switch
            {
                "Available" => RoomStatus.Available,
                "Booked" => RoomStatus.Booked,
                "Reserved" => RoomStatus.Reserved,
                "Waitlist" => RoomStatus.Waitlist,
                "Blocked" => RoomStatus.Blocked,
                _ => RoomStatus.Available,
            };
        }

        private void FilterRooms(string param)
        {
            if (string.IsNullOrEmpty(param) || param == "All")
                _selectedFilter = null;
            else
                _selectedFilter = Enum.Parse<RoomStatus>(param);

            _currentPage = 1;
            UpdatePagination();
        }

        private void UpdatePagination()
        {
            var totalPages = TotalPages;
            if (_currentPage < 1) _currentPage = 1;
            if (_currentPage > totalPages) _currentPage = totalPages;

            var pageRooms = FilteredRooms.Skip((_currentPage - 1) * _pageSize).Take(_pageSize).ToList();

            // Refresh the collection for UI
            PaginatedRooms.Clear();
            foreach (var r in pageRooms)
                PaginatedRooms.Add(r);

            OnPropertyChanged(nameof(TotalPages));
            OnPropertyChanged(nameof(PageNumbers));
            ((RelayCommand)NextPageCommand).RaiseCanExecuteChanged();
            ((RelayCommand)PreviousPageCommand).RaiseCanExecuteChanged();
        }

        private bool CanGoNext() => _currentPage < TotalPages;
        private bool CanGoPrevious() => _currentPage > 1;

        private void NextPage()
        {
            if (CanGoNext())
            {
                _currentPage++;
                UpdatePagination();
            }
        }

        private void PreviousPage()
        {
            if (CanGoPrevious())
            {
                _currentPage--;
                UpdatePagination();
            }
        }

        private void GoToPage(int page)
        {
            if (page >= 1 && page <= TotalPages)
            {
                _currentPage = page;
                UpdatePagination();
            }
        }

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string n = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
    }

    // RelayCommand for ICommand implementation
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;
        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute; _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke() ?? true;
        public void Execute(object parameter) => _execute();
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Predicate<T> _canExecute;
        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action<T> execute, Predicate<T> canExecute = null)
        {
            _execute = execute; _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (parameter == null && typeof(T).IsValueType)
                return _canExecute == null;
            return _canExecute == null || _canExecute((T)parameter);
        }

        public void Execute(object parameter) => _execute((T)parameter);
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}