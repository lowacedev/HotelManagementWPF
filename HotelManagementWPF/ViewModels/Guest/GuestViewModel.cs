
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using HotelManagementWPF.Services;
using HotelManagementWPF.ViewModels.Base;

namespace HotelManagementWPF.ViewModels
{
    public class GuestViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Guest> _guests;
        private ObservableCollection<Guest> _filteredGuests;
        private string _searchText = string.Empty;
        private int _currentPage = 1;
        private const int _itemsPerPage = 10;
        private readonly IWindowService _windowService;


        public ICommand AddGuestCommand { get; }

        public GuestViewModel(IWindowService windowService)
        {
            _windowService = windowService;
            AddGuestCommand = new RelayCommand(OpenAddGuestForm);
        }

        private void OpenAddGuestForm()
        {
            _windowService.ShowAddGuestForm();
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                ApplySearch(); // Automatic search like BookingView
            }
        }

        public ObservableCollection<Guest> Guests
        {
            get => _guests;
            set
            {
                _guests = value;
                OnPropertyChanged();
                UpdatePagination();
            }
        }

        public ObservableCollection<Guest> FilteredGuests
        {
            get => _filteredGuests;
            set
            {
                _filteredGuests = value;
                OnPropertyChanged();
                UpdatePagination();
            }
        }

        public ObservableCollection<Guest> PaginatedGuests { get; set; }
        public ObservableCollection<int> PageNumbers { get; set; }

        // Commands
    
        public ICommand EditGuestCommand { get; private set; }
        public ICommand PreviousPageCommand { get; private set; }
        public ICommand NextPageCommand { get; private set; }
        public ICommand GoToPageCommand { get; private set; }

     

        private void ApplySearch()
        {
            _currentPage = 1;

            if (string.IsNullOrWhiteSpace(_searchText))
            {
                FilteredGuests = new ObservableCollection<Guest>(_guests);
                return;
            }

            var searchLower = _searchText.ToLower();
            var filtered = _guests.Where(guest =>
                guest.Name.ToLower().Contains(searchLower) ||
                guest.PhoneNumber.ToLower().Contains(searchLower) ||
                guest.Gender.ToLower().Contains(searchLower)
            ).ToList();

            FilteredGuests = new ObservableCollection<Guest>(filtered);
        }

        private void ExecuteAddGuest()
        {
            _windowService.ShowAddGuestForm();
            // Optionally refresh guests after adding
            // LoadGuests();
        }

        private void ExecuteEditGuest(Guest guest)
        {
            if (guest == null) return;

            // Implement edit guest functionality
            // You can create a ShowEditGuestForm method in IWindowService
            // _windowService.ShowEditGuestForm(guest);
        }

        private void LoadGuests()
        {
            // Sample data - replace with actual data loading
            _guests = new ObservableCollection<Guest>
            {
                new Guest { Id = 1, Name = "John Smith", Age = 35, Gender = "Male", PhoneNumber = "+1234567890" },
                new Guest { Id = 2, Name = "Jane Doe", Age = 28, Gender = "Female", PhoneNumber = "+1234567891" },
                new Guest { Id = 3, Name = "Bob Johnson", Age = 42, Gender = "Male", PhoneNumber = "+1234567892" },
                new Guest { Id = 4, Name = "Alice Brown", Age = 30, Gender = "Female", PhoneNumber = "+1234567893" },
                new Guest { Id = 5, Name = "Charlie Wilson", Age = 25, Gender = "Male", PhoneNumber = "+1234567894" },
            };

            FilteredGuests = new ObservableCollection<Guest>(_guests);
        }

        private int TotalPages => (int)Math.Ceiling((double)(FilteredGuests?.Count ?? 0) / _itemsPerPage);

        private void UpdatePagination()
        {
            if (FilteredGuests == null) return;

            // Update paginated items
            var skip = (_currentPage - 1) * _itemsPerPage;
            var paginatedItems = FilteredGuests.Skip(skip).Take(_itemsPerPage);

            PaginatedGuests.Clear();
            foreach (var item in paginatedItems)
            {
                PaginatedGuests.Add(item);
            }

            // Update page numbers
            PageNumbers.Clear();
            for (int i = 1; i <= TotalPages; i++)
            {
                PageNumbers.Add(i);
            }

            OnPropertyChanged(nameof(PaginatedGuests));
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

    public class Guest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string PhoneNumber { get; set; }
    }
}