using HotelManagementWPF.Models;
using HotelManagementWPF.ViewModels.Base;
using HotelManagementWPF.Views.Booking;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace HotelManagementWPF.ViewModels.Booking
{
    public class BookingViewModel : INotifyPropertyChanged
    {
        private readonly int _itemsPerPage = 7; // Show 7 items per page
        private int _currentPage = 1;

        private ObservableCollection<BookingData> _bookings; // All bookings
        private ObservableCollection<BookingData> _filteredBookings; // After filter/search
        private ObservableCollection<BookingData> _paginatedBookings; // Current page
        private ObservableCollection<int> _pageNumbers; // Page number list

        private string _searchText = string.Empty;
        private string _currentFilter = "All";

        public BookingViewModel()
        {
            // Initialize collections
            Bookings = new ObservableCollection<BookingData>();
            FilteredBookings = new ObservableCollection<BookingData>();
            PaginatedBookings = new ObservableCollection<BookingData>();
            PageNumbers = new ObservableCollection<int>();

            // Load initial data
            LoadBookingsFromDatabase();

            // Initialize commands
            FilterCommand = new RelayCommand<string>(FilterBookings);
            AddBookingCommand = new RelayCommand(AddBooking);
            EditBookingCommand = new RelayCommand<BookingData>(EditBooking);
            PreviousPageCommand = new RelayCommand(PreviousPage, () => _currentPage > 1);
            NextPageCommand = new RelayCommand(NextPage, () => _currentPage < TotalPages);
            GoToPageCommand = new RelayCommand<int>(GoToPage);

            // Apply initial filter/search
            ApplyFiltersAndSearch();
        }

        // Properties
        public ObservableCollection<BookingData> Bookings
        {
            get => _bookings;
            set
            {
                _bookings = value;
                OnPropertyChanged();
                ApplyFiltersAndSearch();
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

        public ObservableCollection<BookingData> PaginatedBookings
        {
            get => _paginatedBookings;
            set
            {
                _paginatedBookings = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<int> PageNumbers
        {
            get => _pageNumbers;
            set
            {
                _pageNumbers = value;
                OnPropertyChanged();
            }
        }

        public int TotalBookings => Bookings?.Count ?? 0;
        public int CheckinBookings => Bookings?.Count(b => b.StatusText == "Check-In") ?? 0;
        public int CheckoutBookings => Bookings?.Count(b => b.StatusText == "Check-Out") ?? 0;
        public int ReservationBookings => Bookings?.Count(b => b.StatusText == "Reservation") ?? 0;

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

        // Commands
        public ICommand FilterCommand { get; }
        public ICommand AddBookingCommand { get; }
        public ICommand EditBookingCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand GoToPageCommand { get; }

        // Load data from database
        private void LoadBookingsFromDatabase()
        {
            var bookingsFromDb = BookingData.GetAllBookings();
            Bookings = new ObservableCollection<BookingData>(bookingsFromDb);
        }

        // Filter bookings based on filter criteria
        private void FilterBookings(string filter)
        {
            _currentFilter = filter;
            _currentPage = 1;
            ApplyFiltersAndSearch();
        }

        // Apply filters and search
        private void ApplyFiltersAndSearch()
        {
            var filtered = Bookings.AsEnumerable();

            if (_currentFilter != "All")
            {
                var filterLower = _currentFilter.ToLower();
                filtered = filtered.Where(b =>
                    b.StatusText != null && b.StatusText.ToLower() == filterLower);
            }

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

            // Notify counts
            OnPropertyChanged(nameof(TotalBookings));
            OnPropertyChanged(nameof(CheckinBookings));
            OnPropertyChanged(nameof(CheckoutBookings));
            OnPropertyChanged(nameof(ReservationBookings));
        }

        // Update pagination data
        private void UpdatePagination()
        {

            if (FilteredBookings == null || !FilteredBookings.Any())
            {
                PaginatedBookings = new ObservableCollection<BookingData>();
                if (PageNumbers == null)
                    PageNumbers = new ObservableCollection<int>();
                PageNumbers.Clear();
                return;
            }

            int totalPages = TotalPages;

            // Clamp current page
            if (_currentPage > totalPages) _currentPage = totalPages;
            if (_currentPage < 1) _currentPage = 1;

            // Get current page items
            var skip = (_currentPage - 1) * _itemsPerPage;
            var pageItems = FilteredBookings.Skip(skip).Take(_itemsPerPage).ToList();

            // Assign to PaginatedBookings
            PaginatedBookings = new ObservableCollection<BookingData>(pageItems);

            // Generate page numbers
            if (PageNumbers == null)
                PageNumbers = new ObservableCollection<int>();
            PageNumbers.Clear();
            for (int i = 1; i <= totalPages; i++)
            {
                PageNumbers.Add(i);
            }

            // Update command CanExecute
        }

        // Calculate total pages
        private int TotalPages => (int)Math.Ceiling((double)(FilteredBookings?.Count ?? 0) / _itemsPerPage);

        // Navigation methods
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

        // Add booking
        private void AddBooking()
        {
            var form = new BookingFormView();
            var vm = new BookingFormViewModel();
            form.DataContext = vm;
            form.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            if (Application.Current.MainWindow != null)
            {
                form.Owner = Application.Current.MainWindow;
            }

            form.ShowDialog();

            // Reload data
            LoadBookingsFromDatabase();
            ApplyFiltersAndSearch();
        }

        // Edit booking
        private void EditBooking(BookingData booking)
        {
            if (booking == null) return;

            var form = new BookingFormView();
            var vm = new BookingFormViewModel();

            // Pre-fill data
            vm.FullName = booking.Guest;
            vm.RoomNumber = booking.RoomNumber;
            vm.CheckInDate = booking.CheckIn;
            vm.CheckOutDate = booking.CheckOut;
            // Add other properties as needed

            form.DataContext = vm;
            form.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            if (Application.Current.MainWindow != null)
            {
                form.Owner = Application.Current.MainWindow;
            }

            form.ShowDialog();

            // Reload data
            LoadBookingsFromDatabase();
            ApplyFiltersAndSearch();
        }

        // INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}