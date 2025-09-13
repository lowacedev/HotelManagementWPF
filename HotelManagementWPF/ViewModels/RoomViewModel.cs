using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using HotelManagementWPF.Models;
using HotelManagementWPF.Services.Interfaces;
using HotelManagementWPF.ViewModels.Base;

namespace HotelManagementWPF.ViewModels
{
    public class RoomViewModel : BaseViewModel
    {
        private readonly IRoomService _roomService;
        private ObservableCollection<Room> _allRooms;
        private ObservableCollection<Room> _filteredRooms;
        private ObservableCollection<Room> _paginatedRooms;
        private RoomStatus _selectedFilter;
        private int _currentPage = 1;
        private int _totalPages;
        private int _totalRooms;
        private int _availableRooms;
        private int _bookedRooms;
        private bool _isLoading;
        private ObservableCollection<int> _pageNumbers;

        public RoomViewModel(IRoomService roomService)
        {
            _roomService = roomService;
            _allRooms = new ObservableCollection<Room>();
            _filteredRooms = new ObservableCollection<Room>();
            _paginatedRooms = new ObservableCollection<Room>();
            _pageNumbers = new ObservableCollection<int>();
            _selectedFilter = RoomStatus.Available; // Default to show all

            // Initialize commands
            LoadRoomsCommand = new RelayCommand(async () => await LoadRoomsAsync());
            FilterRoomsCommand = new RelayCommand<RoomStatus>(async (status) => await FilterRoomsAsync(status));
            AddRoomCommand = new RelayCommand(() => AddRoom());
            EditRoomCommand = new RelayCommand<Room>(async (room) => await EditRoomAsync(room));
            DeleteRoomCommand = new RelayCommand<Room>(async (room) => await DeleteRoomAsync(room));
            ViewRoomCommand = new RelayCommand<Room>(async (room) => await ViewRoomAsync(room));
            PreviousPageCommand = new RelayCommand(() => PreviousPage(), () => CurrentPage > 1);
            NextPageCommand = new RelayCommand(() => NextPage(), () => CurrentPage < TotalPages);
            GoToPageCommand = new RelayCommand<int>(async (page) => await GoToPageAsync(page));

            // Load initial data
            LoadRoomsCommand.Execute(null);
        }

        #region Properties

        public ObservableCollection<Room> PaginatedRooms
        {
            get => _paginatedRooms;
            set => SetProperty(ref _paginatedRooms, value);
        }

        public RoomStatus SelectedFilter
        {
            get => _selectedFilter;
            set => SetProperty(ref _selectedFilter, value);
        }

        public int CurrentPage
        {
            get => _currentPage;
            set => SetProperty(ref _currentPage, value);
        }

        public int TotalPages
        {
            get => _totalPages;
            set => SetProperty(ref _totalPages, value);
        }

        public int TotalRooms
        {
            get => _totalRooms;
            set => SetProperty(ref _totalRooms, value);
        }

        public int AvailableRooms
        {
            get => _availableRooms;
            set => SetProperty(ref _availableRooms, value);
        }

        public int BookedRooms
        {
            get => _bookedRooms;
            set => SetProperty(ref _bookedRooms, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ObservableCollection<int> PageNumbers
        {
            get => _pageNumbers;
            set => SetProperty(ref _pageNumbers, value);
        }

        #endregion

        #region Commands

        public RelayCommand LoadRoomsCommand { get; }
        public RelayCommand<RoomStatus> FilterRoomsCommand { get; }
        public RelayCommand AddRoomCommand { get; }
        public RelayCommand<Room> EditRoomCommand { get; }
        public RelayCommand<Room> DeleteRoomCommand { get; }
        public RelayCommand<Room> ViewRoomCommand { get; }
        public RelayCommand PreviousPageCommand { get; }
        public RelayCommand NextPageCommand { get; }
        public RelayCommand<int> GoToPageCommand { get; }

        #endregion

        #region Command Methods

        private async Task LoadRoomsAsync()
        {
            try
            {
                IsLoading = true;
                var rooms = await _roomService.GetAllRoomsAsync();
                
                _allRooms.Clear();
                foreach (var room in rooms)
                {
                    _allRooms.Add(room);
                }

                // Update counts
                TotalRooms = await _roomService.GetTotalRoomCountAsync();
                AvailableRooms = await _roomService.GetRoomCountByStatusAsync(RoomStatus.Available);
                BookedRooms = await _roomService.GetRoomCountByStatusAsync(RoomStatus.Booked);

                // Apply current filter
                await FilterRoomsAsync(SelectedFilter);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading rooms: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task FilterRoomsAsync(RoomStatus status)
        {
            try
            {
                IsLoading = true;
                SelectedFilter = status;

                _filteredRooms.Clear();
                
                if (status == RoomStatus.Available) // Show all rooms
                {
                    foreach (var room in _allRooms)
                    {
                        _filteredRooms.Add(room);
                    }
                }
                else
                {
                    var filtered = _allRooms.Where(r => r.Status == status);
                    foreach (var room in filtered)
                    {
                        _filteredRooms.Add(room);
                    }
                }

                // Reset to first page and update pagination
                CurrentPage = 1;
                UpdatePagination();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error filtering rooms: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void UpdatePagination()
        {
            const int pageSize = 10;
            TotalPages = (int)Math.Ceiling((double)_filteredRooms.Count / pageSize);

            if (TotalPages == 0) TotalPages = 1;
            if (CurrentPage > TotalPages) CurrentPage = TotalPages;

            var startIndex = (CurrentPage - 1) * pageSize;
            var endIndex = Math.Min(startIndex + pageSize, _filteredRooms.Count);

            PaginatedRooms.Clear();
            for (int i = startIndex; i < endIndex; i++)
            {
                PaginatedRooms.Add(_filteredRooms[i]);
            }

            // Update page numbers
            PageNumbers.Clear();
            for (int i = 1; i <= TotalPages; i++)
            {
                PageNumbers.Add(i);
            }

            // Update command states
            PreviousPageCommand.RaiseCanExecuteChanged();
            NextPageCommand.RaiseCanExecuteChanged();
        }

        private void AddRoom()
        {
            // TODO: Open Add Room dialog
            MessageBox.Show("Add Room functionality will be implemented", "Add Room", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async Task EditRoomAsync(Room room)
        {
            if (room == null) return;
            
            // TODO: Open Edit Room dialog
            MessageBox.Show($"Edit Room {room.RoomNumber} functionality will be implemented", "Edit Room", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async Task DeleteRoomAsync(Room room)
        {
            if (room == null) return;

            var result = MessageBox.Show($"Are you sure you want to delete room {room.RoomNumber}?", 
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    IsLoading = true;
                    var success = await _roomService.DeleteRoomAsync(room.Id);
                    if (success)
                    {
                        _allRooms.Remove(room);
                        await LoadRoomsAsync(); // Refresh data
                        MessageBox.Show("Room deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting room: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    IsLoading = false;
                }
            }
        }

        private async Task ViewRoomAsync(Room room)
        {
            if (room == null) return;
            
            // TODO: Open View Room Details dialog
            MessageBox.Show($"View Room {room.RoomNumber} details functionality will be implemented", "Room Details", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private async Task GoToPageAsync(int page)
        {
            if (page >= 1 && page <= TotalPages)
            {
                CurrentPage = page;
                UpdatePagination();
            }
        }

        #endregion
    }
}
