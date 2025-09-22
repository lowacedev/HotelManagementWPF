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
    public class UserViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<User> _users;
        private ObservableCollection<User> _filteredUsers;
        private string _searchText = string.Empty;
        private int _currentPage = 1;
        private const int _itemsPerPage = 10;
        private readonly IWindowService _windowService;

        public UserViewModel(IWindowService windowService)
        {
            _windowService = windowService;
            InitializeCommands();
            InitializeCollections();
            LoadUsers();
        }

        private void InitializeCommands()
        {
            AddUserCommand = new RelayCommand(ExecuteAddUser);
            EditUserCommand = new RelayCommand<User>(ExecuteEditUser);
            PreviousPageCommand = new RelayCommand(PreviousPage, () => _currentPage > 1);
            NextPageCommand = new RelayCommand(NextPage, () => _currentPage < TotalPages);
            GoToPageCommand = new RelayCommand<int>(GoToPage);
        }

        private void InitializeCollections()
        {
            _users = new ObservableCollection<User>();
            _filteredUsers = new ObservableCollection<User>();
            PaginatedUsers = new ObservableCollection<User>();
            PageNumbers = new ObservableCollection<int>();
        }

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

        public ObservableCollection<User> Users
        {
            get => _users;
            set
            {
                _users = value;
                OnPropertyChanged();
                UpdatePagination();
            }
        }

        public ObservableCollection<User> FilteredUsers
        {
            get => _filteredUsers;
            set
            {
                _filteredUsers = value;
                OnPropertyChanged();
                UpdatePagination();
            }
        }

        public ObservableCollection<User> PaginatedUsers { get; set; }
        public ObservableCollection<int> PageNumbers { get; set; }

        // Commands
        public ICommand AddUserCommand { get; private set; }
        public ICommand EditUserCommand { get; private set; }
        public ICommand PreviousPageCommand { get; private set; }
        public ICommand NextPageCommand { get; private set; }
        public ICommand GoToPageCommand { get; private set; }

        private void ApplySearch()
        {
            _currentPage = 1;

            if (string.IsNullOrWhiteSpace(_searchText))
            {
                FilteredUsers = new ObservableCollection<User>(_users);
                return;
            }

            var searchLower = _searchText.ToLower();
            var filtered = _users.Where(user =>
                user.Name.ToLower().Contains(searchLower) ||
                user.Email.ToLower().Contains(searchLower) ||
                user.Role.ToLower().Contains(searchLower)
            ).ToList();

            FilteredUsers = new ObservableCollection<User>(filtered);
        }

        private void ExecuteAddUser()
        {
            _windowService.ShowAddUserForm();
            // Optionally refresh users after adding
            // LoadUsers();
        }

        private void ExecuteEditUser(User user)
        {
            if (user == null) return;
            _windowService.ShowEditUserForm(user);


            // Implement edit user functionality
            // You can create a ShowEditUserForm method in IWindowService
            // _windowService.ShowEditUserForm(user);
        }

        private void LoadUsers()
        {
            // Sample data - replace with actual data loading
            _users = new ObservableCollection<User>
            {
                new User { Id = 1, Name = "John Admin", Email = "john@hotel.com", Role = "Administrator", CreatedDate = DateTime.Now.AddDays(-30) },
                new User { Id = 2, Name = "Sarah Manager", Email = "sarah@hotel.com", Role = "Manager", CreatedDate = DateTime.Now.AddDays(-25) },
                new User { Id = 3, Name = "Mike Reception", Email = "mike@hotel.com", Role = "Receptionist", CreatedDate = DateTime.Now.AddDays(-20) },
                new User { Id = 4, Name = "Lisa Staff", Email = "lisa@hotel.com", Role = "Staff", CreatedDate = DateTime.Now.AddDays(-15) },
                new User { Id = 5, Name = "Tom Supervisor", Email = "tom@hotel.com", Role = "Supervisor", CreatedDate = DateTime.Now.AddDays(-10) },
                new User { Id = 6, Name = "Emma Reception", Email = "emma@hotel.com", Role = "Receptionist", CreatedDate = DateTime.Now.AddDays(-8) },
                new User { Id = 7, Name = "David Manager", Email = "david@hotel.com", Role = "Manager", CreatedDate = DateTime.Now.AddDays(-5) },
                new User { Id = 8, Name = "Anna Staff", Email = "anna@hotel.com", Role = "Staff", CreatedDate = DateTime.Now.AddDays(-3) },
            };

            FilteredUsers = new ObservableCollection<User>(_users);
        }

        private int TotalPages => (int)Math.Ceiling((double)(FilteredUsers?.Count ?? 0) / _itemsPerPage);

        private void UpdatePagination()
        {
            if (FilteredUsers == null) return;

            // Update paginated items
            var skip = (_currentPage - 1) * _itemsPerPage;
            var paginatedItems = FilteredUsers.Skip(skip).Take(_itemsPerPage);

            PaginatedUsers.Clear();
            foreach (var item in paginatedItems)
            {
                PaginatedUsers.Add(item);
            }

            // Update page numbers
            PageNumbers.Clear();
            for (int i = 1; i <= TotalPages; i++)
            {
                PageNumbers.Add(i);
            }

            OnPropertyChanged(nameof(PaginatedUsers));
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

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}