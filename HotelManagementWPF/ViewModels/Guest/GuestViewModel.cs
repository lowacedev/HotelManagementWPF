using DatabaseProject;
using HotelManagementWPF.Services;
using HotelManagementWPF.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace HotelManagementWPF.ViewModels
{
    public class GuestViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<GuestModel> _guests;
        private ObservableCollection<GuestModel> _filteredGuests;
        private string _searchText = string.Empty;
        private int _currentPage = 1;
        private const int _itemsPerPage = 10;
        private readonly IWindowService _windowService;

        public ICommand AddGuestCommand { get; }
        public ICommand EditGuestCommand { get; private set; }
        public ICommand PreviousPageCommand { get; private set; }
        public ICommand NextPageCommand { get; private set; }
        public ICommand GoToPageCommand { get; private set; }

        public ObservableCollection<GuestModel> Guests
        {
            get => _guests;
            set
            {
                _guests = value;
                OnPropertyChanged();
                UpdatePagination();
            }
        }

        public ObservableCollection<GuestModel> FilteredGuests
        {
            get => _filteredGuests;
            set
            {
                _filteredGuests = value;
                OnPropertyChanged();
                UpdatePagination();
            }
        }

        public ObservableCollection<GuestModel> PaginatedGuests { get; set; } = new ObservableCollection<GuestModel>();
        public ObservableCollection<int> PageNumbers { get; set; } = new ObservableCollection<int>();

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                ApplySearch();
            }
        }

        public GuestViewModel(IWindowService windowService)
        {
            _windowService = windowService;
            AddGuestCommand = new RelayCommand(OpenAddGuestForm);
            EditGuestCommand = new RelayCommand<GuestModel>(OpenEditGuestForm);

            // Initialize collections
            PaginatedGuests = new ObservableCollection<GuestModel>();
            PageNumbers = new ObservableCollection<int>();

            // Load guests from database
            LoadGuests();

            // Initialize pagination commands
            PreviousPageCommand = new RelayCommand<int>(_ => PreviousPage(), _ => _currentPage > 1);
            NextPageCommand = new RelayCommand<int>(_ => NextPage(), _ => _currentPage < TotalPages);
            GoToPageCommand = new RelayCommand<int>(param => GoToPage(param), param => true);
        }

        private void OpenAddGuestForm()
        {
            _windowService.ShowAddGuestForm();
            // After adding a guest, refresh the list:
            LoadGuests();
        }

        private void OpenEditGuestForm(GuestModel guest)
        {
            if (guest == null) return;

            _windowService.ShowEditGuestForm(guest);
            // After editing a guest, refresh the list:
            LoadGuests();
        }

        private void ApplySearch()
        {
            _currentPage = 1;

            if (string.IsNullOrWhiteSpace(_searchText))
            {
                FilteredGuests = new ObservableCollection<GuestModel>(_guests);
                return;
            }

            var searchLower = _searchText.ToLower();
            var filtered = _guests.Where(guest =>
                guest.Name.ToLower().Contains(searchLower) ||
                guest.PhoneNumber.ToLower().Contains(searchLower) ||
                guest.Gender.ToLower().Contains(searchLower)
            ).ToList();

            FilteredGuests = new ObservableCollection<GuestModel>(filtered);
        }

        private void LoadGuests()
        {
            var db = new DbConnections();
            DataTable dt = new DataTable();

            string query = "SELECT guest_id, name, age, gender, phoneNumber FROM tbl_Guest";

            try
            {
                db.readDatathroughAdapter(query, dt);

                var guestList = new ObservableCollection<GuestModel>();
                foreach (DataRow row in dt.Rows)
                {
                    guestList.Add(new GuestModel(
                        Id: Convert.ToInt32(row["guest_id"]),   // ✅ use real ID
                        Name: row["name"].ToString(),
                        Age: Convert.ToInt32(row["age"]),
                        Gender: row["gender"].ToString(),
                        PhoneNumber: row["phoneNumber"].ToString()
                    ));
                }

                Guests = guestList;
                FilteredGuests = new ObservableCollection<GuestModel>(Guests);
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Console.WriteLine(ex.Message);
            }
        }

        private int TotalPages => (int)Math.Ceiling((double)(FilteredGuests?.Count ?? 0) / _itemsPerPage);

        private void UpdatePagination()
        {
            if (FilteredGuests == null) return;

            // Clear current paginated list
            PaginatedGuests.Clear();

            // Calculate items to display
            var skip = (_currentPage - 1) * _itemsPerPage;
            var paginatedItems = FilteredGuests.Skip(skip).Take(_itemsPerPage).ToList();

            foreach (var item in paginatedItems)
            {
                PaginatedGuests.Add(item);
            }

            // Rebuild page numbers
            PageNumbers.Clear();
            for (int i = 1; i <= TotalPages; i++)
            {
                PageNumbers.Add(i);
            }

            // Notify commands
            (PreviousPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (NextPageCommand as RelayCommand)?.RaiseCanExecuteChanged();

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

    // Make sure this record is in a file accessible to your project
    public record GuestModel(int Id, string Name, int Age, string Gender, string PhoneNumber);
}