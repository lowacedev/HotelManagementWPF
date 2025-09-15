using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using HotelManagementWPF.Models;
using HotelManagementWPF.ViewModels.Base;

namespace HotelManagementWPF.ViewModels
{
    public class RoomViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Room> _allRooms;
        private ObservableCollection<Room> _paginatedRooms;
        private RoomStatus? _selectedFilter;
        private int _currentPage = 1;
        private int _pageSize = 10;

        public RoomViewModel()
        {
            // Sample data
            _allRooms = new ObservableCollection<Room>
            {
                new Room { RoomNumber = "#001", BedType = "Double bed", Price = 120, Status = RoomStatus.Available },
                new Room { RoomNumber = "#002", BedType = "Single bed", Price = 100, Status = RoomStatus.Booked },
                new Room { RoomNumber = "#003", BedType = "VIP", Price = 200, Status = RoomStatus.Booked },
                new Room { RoomNumber = "#004", BedType = "VIP", Price = 200, Status = RoomStatus.Reserved },
                new Room { RoomNumber = "#005", BedType = "Single bed", Price = 100, Status = RoomStatus.Reserved },
                new Room { RoomNumber = "#006", BedType = "Double bed", Price = 120, Status = RoomStatus.Waitlist },
                new Room { RoomNumber = "#007", BedType = "Double bed", Price = 120, Status = RoomStatus.Reserved },
                new Room { RoomNumber = "#008", BedType = "Single bed", Price = 100, Status = RoomStatus.Blocked }
            };

            PaginatedRooms = new ObservableCollection<Room>();
            FilterCommand = new RelayCommand<string>(param => FilterRooms(param));
            AddRoomCommand = new RelayCommand(() => AddRoom());
            PreviousPageCommand = new RelayCommand(() => PreviousPage(), () => CurrentPage > 1);
            NextPageCommand = new RelayCommand(() => NextPage(), () => CurrentPage < TotalPages);
            GoToPageCommand = new RelayCommand<int>(page => GoToPage(page));

            UpdatePagination();
        }

        public ObservableCollection<Room> PaginatedRooms
        {
            get => _paginatedRooms;
            set { _paginatedRooms = value; OnPropertyChanged(); }
        }

        public int TotalRooms => _allRooms.Count;
        public int AvailableRooms => _allRooms.Count(r => r.Status == RoomStatus.Available);
        public int BookedRooms => _allRooms.Count(r => r.Status == RoomStatus.Booked);

        public int CurrentPage
        {
            get => _currentPage;
            set { _currentPage = value; OnPropertyChanged(); }
        }

        public int TotalPages => (int)Math.Ceiling((double)FilteredRooms.Count / _pageSize);

        public List<int> PageNumbers => Enumerable.Range(1, TotalPages).ToList();

        public ICommand FilterCommand { get; }
        public ICommand AddRoomCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand GoToPageCommand { get; }

        private List<Room> FilteredRooms =>
            _selectedFilter == null ? _allRooms.ToList() : _allRooms.Where(r => r.Status == _selectedFilter).ToList();

        private void FilterRooms(string param)
        {
            if (string.IsNullOrEmpty(param) || param == "All")
                _selectedFilter = null;
            else
                _selectedFilter = Enum.Parse<RoomStatus>(param);

            CurrentPage = 1;
            UpdatePagination();
        }

        private void AddRoom()
        {
            // UI only: show dialog or add logic here if needed
        }

        private void PreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                UpdatePagination();
            }
        }

        private void NextPage()
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                UpdatePagination();
            }
        }

        private void GoToPage(int page)
        {
            if (page >= 1 && page <= TotalPages)
            {
                CurrentPage = page;
                UpdatePagination();
            }
        }

        private void UpdatePagination()
        {
            PaginatedRooms.Clear();
            var rooms = FilteredRooms.Skip((CurrentPage - 1) * _pageSize).Take(_pageSize);
            foreach (var room in rooms)
                PaginatedRooms.Add(room);

            (PreviousPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (NextPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}