using DatabaseProject;
using HotelManagementWPF.Services;
using Syncfusion.Windows.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

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
        private string _fullName;
        public string FullName
        {
            get => _fullName;
            set
            {
                if (_fullName != value)
                {
                    _fullName = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(FirstLetter));
                }
            }
        }

        // New property for just first name
        private string _firstName;
public string FirstName
{
    get => _firstName;
    set
    {
        if (_firstName != value)
        {
            _firstName = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(FirstLetter)); // triggers update for first letter
        }
    }
}

        public string FirstLetter
        {
            get
            {
                if (string.IsNullOrWhiteSpace(FirstName))
                    return "";
                return FirstName.Substring(0, 1).ToUpper();
            }
        }
        public void UpdateCurrentUser(string fullName, string role)
        {
            FirstName = fullName.Split(' ').FirstOrDefault() ?? "";
            Role = role;
        }
        public void SetCurrentUserFullName(string fullName, string role)
        {
            FirstName = fullName.Split(' ').FirstOrDefault() ?? "";
            Role = role;

        }

        private string _role;
        public string Role
        {
            get => _role;
            set
            {
                if (_role != value)
                {
                    _role = value;
                    OnPropertyChanged();
                }
            }
        }

        public UserViewModel(IWindowService windowService)
        {
            _windowService = windowService;
            InitializeCommands();
            InitializeCollections();

            // Load users during initialization
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
            LoadUsers(); // refresh after adding
        }

        private void ExecuteEditUser(User user)
        {
            if (user == null) return;
            _windowService.ShowEditUserForm(user);
            LoadUsers(); // refresh after editing
        }

        // Method to load users directly from DB
        // When loading users, call SetCurrentUserFullName with both name and role
        public void LoadUsers()
        {
            var usersFromDb = new List<User>();
            try
            {
                using (var db = new DbConnections())
                {
                    string query = "SELECT user_id, name, role, email, username, password, GETDATE() as CreatedDate FROM tbl_User";
                    DataTable dt = new DataTable();
                    db.readDatathroughAdapter(query, dt);
                    foreach (DataRow row in dt.Rows)
                    {
                        string fullNameFromDb = row["name"].ToString();
                        usersFromDb.Add(new User
                        {
                            UserId = Convert.ToInt32(row["user_id"]),
                            Name = fullNameFromDb,
                            Role = row["role"].ToString(),
                            Email = row["email"].ToString(),
                            Username = row["username"].ToString(),
                            Password = row["password"].ToString(),
                            CreatedDate = row["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(row["CreatedDate"]) : DateTime.Now
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading users: {ex.Message}");
            }

            if (usersFromDb.Any())
            {
                var currentUser = usersFromDb.First();
                SetCurrentUserFullName(currentUser.Name, currentUser.Role);
            }

            // Set both Users and FilteredUsers
            Users = new ObservableCollection<User>(usersFromDb);
            FilteredUsers = new ObservableCollection<User>(usersFromDb);
            _currentPage = 1;
            UpdatePagination();
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
                PageNumbers.Add(i);

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

    // User class as already provided
    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}