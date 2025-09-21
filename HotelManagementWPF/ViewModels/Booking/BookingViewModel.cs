using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows;
using System.Linq;
using HotelManagementWPF.ViewModels.Base;
using HotelManagementWPF.Views.Booking;

namespace HotelManagementWPF.ViewModels.Booking
{
    public class BookingViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<BookingData> _bookings;
        private ObservableCollection<BookingData> _filteredBookings;
        private string _currentFilter = "All";
        private string _searchText = string.Empty;
        private int _currentPage = 1;
        private const int _itemsPerPage = 10;

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                ApplyFiltersAndSearch();
            }
        }

        public ObservableCollection<BookingData> Bookings
        {
            get => _bookings;
            set
            {
                _bookings = value;
                OnPropertyChanged();
                UpdatePagination();
            }
        }

        public ObservableCollection<BookingData> FilteredBookings
        {
            get => _filteredBookings;
            set
            {
                _filteredBookings = value;
                OnPropertyChanged();
                UpdatePagination();
            }
        }

        public ObservableCollection<BookingData> PaginatedBookings { get; set; }
        public ObservableCollection<int> PageNumbers { get; set; }

        public int TotalBookings => Bookings?.Count ?? 0;
        public int ActiveBookings => Bookings?.Count(b => b.Status == "Active") ?? 0;
        public int CompletedBookings => Bookings?.Count(b => b.Status == "Completed") ?? 0;
        public int CancelledBookings => Bookings?.Count(b => b.Status == "Cancelled") ?? 0;

        public ICommand FilterCommand { get; }
        public ICommand AddBookingCommand { get; }
        public ICommand EditBookingCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand GoToPageCommand { get; }

        public BookingViewModel()
        {
            PaginatedBookings = new ObservableCollection<BookingData>();
            PageNumbers = new ObservableCollection<int>();

            // Initialize with sample data
            Bookings = new ObservableCollection<BookingData>
            {
                new BookingData {
                    Id = 1,
                    Guest = "Said el hadrry",
                    Room = "Room 1",
                    CheckIn = new DateTime(2025, 2, 9),
                    CheckOut = new DateTime(2025, 2, 11),
                    Status = "Active"
                },
                new BookingData {
                    Id = 2,
                    Guest = "Said el hadrry",
                    Room = "Room 2",
                    CheckIn = new DateTime(2025, 2, 10),
                    CheckOut = new DateTime(2025, 2, 19),
                    Status = "Active"
                },
                new BookingData {
                    Id = 3,
                    Guest = "Jane Cooper",
                    Room = "Room 4",
                    CheckIn = new DateTime(2025, 2, 11),
                    CheckOut = new DateTime(2025, 2, 16),
                    Status = "Active"
                },
                new BookingData {
                    Id = 4,
                    Guest = "Said el hadrry",
                    Room = "Room 6",
                    CheckIn = new DateTime(2025, 2, 1),
                    CheckOut = new DateTime(2025, 2, 9),
                    Status = "Completed"
                },
                new BookingData {
                    Id = 5,
                    Guest = "John Smith",
                    Room = "Room 3",
                    CheckIn = new DateTime(2025, 1, 15),
                    CheckOut = new DateTime(2025, 1, 20),
                    Status = "Completed"
                },
                new BookingData {
                    Id = 6,
                    Guest = "Emma Wilson",
                    Room = "Room 5",
                    CheckIn = new DateTime(2025, 3, 1),
                    CheckOut = new DateTime(2025, 3, 5),
                    Status = "Cancelled"
                }
            };

            FilteredBookings = new ObservableCollection<BookingData>(Bookings);

            // Initialize commands
            FilterCommand = new RelayCommand<string>(FilterBookings);
            AddBookingCommand = new RelayCommand(AddBooking);
            EditBookingCommand = new RelayCommand<BookingData>(EditBooking);
            PreviousPageCommand = new RelayCommand(PreviousPage, () => _currentPage > 1);
            NextPageCommand = new RelayCommand(NextPage, () => _currentPage < TotalPages);
            GoToPageCommand = new RelayCommand<int>(GoToPage);

            UpdatePagination();
        }

        private void FilterBookings(string filter)
        {
            _currentFilter = filter;
            _currentPage = 1;
            ApplyFiltersAndSearch();
        }

        private void ApplyFiltersAndSearch()
        {
            var filtered = Bookings.AsEnumerable();

            // Apply status filter
            if (_currentFilter != "All")
            {
                filtered = filtered.Where(b => b.Status == _currentFilter);
            }

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(_searchText))
            {
                var searchLower = _searchText.ToLower();
                filtered = filtered.Where(b =>
                    b.Guest.ToLower().Contains(searchLower) ||
                    b.Room.ToLower().Contains(searchLower) ||
                    b.Status.ToLower().Contains(searchLower));
            }

            FilteredBookings = new ObservableCollection<BookingData>(filtered);
            UpdatePagination();
            OnPropertyChanged(nameof(TotalBookings));
            OnPropertyChanged(nameof(ActiveBookings));
            OnPropertyChanged(nameof(CompletedBookings));
            OnPropertyChanged(nameof(CancelledBookings));
        }

        private void AddBooking()
        {
            var form = new BookingFormView();
            var vm = new BookingFormViewModel();
            form.DataContext = vm;

            // Center the dialog
            form.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            if (Application.Current.MainWindow != null)
            {
                form.Owner = Application.Current.MainWindow;
            }

            form.ShowDialog();
        }

        private void EditBooking(BookingData booking)
        {
            if (booking == null) return;

            var form = new BookingFormView();
            var vm = new BookingFormViewModel();
            // Pre-fill with booking data for editing
            vm.FullName = booking.Guest;
            vm.RoomNumber = booking.Room;
            vm.CheckInDate = booking.CheckIn;
            vm.CheckOutDate = booking.CheckOut;
            // Set other properties as needed

            form.DataContext = vm;

            // Center the dialog - you'll need to pass the parent window
            // This requires accessing the current window from the view
            form.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            // Note: To properly set the owner, you'll need to modify this method
            // to accept the parent window as a parameter, or use Application.Current.MainWindow
            if (Application.Current.MainWindow != null)
            {
                form.Owner = Application.Current.MainWindow;
            }

            form.ShowDialog();
        }

        private int TotalPages => (int)Math.Ceiling((double)(FilteredBookings?.Count ?? 0) / _itemsPerPage);

        private void UpdatePagination()
        {
            if (FilteredBookings == null) return;

            // Update paginated items
            var skip = (_currentPage - 1) * _itemsPerPage;
            var paginatedItems = FilteredBookings.Skip(skip).Take(_itemsPerPage);

            PaginatedBookings.Clear();
            foreach (var item in paginatedItems)
            {
                PaginatedBookings.Add(item);
            }

            // Update page numbers
            PageNumbers.Clear();
            for (int i = 1; i <= TotalPages; i++)
            {
                PageNumbers.Add(i);
            }

            OnPropertyChanged(nameof(PaginatedBookings));
            OnPropertyChanged(nameof(PageNumbers));
        }

        private void PreviousPage()
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                UpdatePagination();
            }
        }

        private void NextPage()
        {
            if (_currentPage < TotalPages)
            {
                _currentPage++;
                UpdatePagination();
            }
        }

        private void GoToPage(int pageNumber)
        {
            _currentPage = pageNumber;
            UpdatePagination();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class BookingData
    {
        public int Id { get; set; }
        public required string Guest { get; set; }
        public required string Room { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public required string Status { get; set; }

        public string CheckInFormatted => CheckIn.ToString("MMM dd, yyyy");
        public string CheckOutFormatted => CheckOut.ToString("MMM dd, yyyy");
        public int Nights => (CheckOut - CheckIn).Days;
    }

    // Keep the old classes for backward compatibility if needed elsewhere
    public class RoomResource
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string Type { get; set; }
    }

    public class BookingAppointment
    {
        public required string Subject { get; set; }
        public required DateTime StartTime { get; set; }
        public required DateTime EndTime { get; set; }
        public required int RoomId { get; set; }
        public required string StatusColor { get; set; }
    }

    public class AppointmentMapping
    {
        public required string Subject { get; set; }
        public required string StartTime { get; set; }
        public required string EndTime { get; set; }
        public required string ResourceId { get; set; }
        public required string AppointmentBackground { get; set; }
    }

    public class ResourceMapping
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
    }
}