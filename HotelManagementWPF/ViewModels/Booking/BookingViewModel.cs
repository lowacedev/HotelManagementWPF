using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows;
using System.Linq;
using System.Text; // For decoding byte arrays
using HotelManagementWPF.ViewModels.Base;
using HotelManagementWPF.Views.Booking;
using HotelManagementWPF.Models; // Ensure this namespace is included for BookingData

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
        public int CheckinBookings => Bookings?.Count(b => b.StatusText == "Check-In") ?? 0;
        public int CheckoutBookings => Bookings?.Count(b => b.StatusText == "Check-Out") ?? 0;
        public int ReservationBookings => Bookings?.Count(b => b.StatusText == "Reservation") ?? 0;

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

            // Load data from database
            LoadBookingsFromDatabase();

            // Initialize filtered list
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

        private void LoadBookingsFromDatabase()
        {
            // Fetch bookings from your database
            var bookingsFromDb = BookingData.GetAllBookings();

            // Assign to Bookings collection
            Bookings = new ObservableCollection<BookingData>(bookingsFromDb);
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

            // Filter by status
            if (_currentFilter != "All")
            {
                filtered = filtered.Where(b =>
                {
                    var statusStr = b.StatusText;
                    return statusStr == _currentFilter;
                });
            }

            // Filter by search text
            if (!string.IsNullOrWhiteSpace(_searchText))
            {
                var searchLower = _searchText.ToLower();
                filtered = filtered.Where(b =>
                    b.Guest.ToLower().Contains(searchLower) ||
                    b.RoomNumber.ToLower().Contains(searchLower) ||
                    b.StatusText.ToLower().Contains(searchLower));
            }

            FilteredBookings = new ObservableCollection<BookingData>(filtered);
            _currentPage = 1;
            UpdatePagination();

            OnPropertyChanged(nameof(TotalBookings));
            OnPropertyChanged(nameof(CheckinBookings));
            OnPropertyChanged(nameof(CheckoutBookings));
            OnPropertyChanged(nameof(ReservationBookings));
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

            // Reload data after adding
            LoadBookingsFromDatabase();
            ApplyFiltersAndSearch();
        }

        private void EditBooking(BookingData booking)
        {
            if (booking == null) return;

            var form = new BookingFormView();
            var vm = new BookingFormViewModel();
            // Pre-fill with booking data
            vm.FullName = booking.Guest;
            vm.RoomNumber = booking.RoomNumber;
            vm.CheckInDate = booking.CheckIn;
            vm.CheckOutDate = booking.CheckOut;
            // Set other properties as needed

            form.DataContext = vm;
            form.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            if (Application.Current.MainWindow != null)
            {
                form.Owner = Application.Current.MainWindow;
            }

            form.ShowDialog();

            // Reload data after editing
            LoadBookingsFromDatabase();
            ApplyFiltersAndSearch();
        }

        private int TotalPages => (int)Math.Ceiling((double)(FilteredBookings?.Count ?? 0) / _itemsPerPage);

        private void UpdatePagination()
        {
            if (FilteredBookings == null) return;

            // Update paginated items
            var skip = (_currentPage - 1) * _itemsPerPage;
            var paginatedItems = FilteredBookings.Skip(skip).Take(_itemsPerPage);

            // Clear and add new items
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
}