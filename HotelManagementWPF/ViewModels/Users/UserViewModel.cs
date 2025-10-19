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
using System.Windows;
using System.Windows.Input;

namespace HotelManagementWPF.ViewModels
{
    public class UserViewModel : INotifyPropertyChanged
    {
        public int CurrentUserId { get; private set; }
        private ObservableCollection<User> _users;
        private ObservableCollection<User> _filteredUsers;
        private string _searchText = string.Empty;
        private int _currentPage = 1;
        private const int _itemsPerPage = 10;
        private readonly IWindowService _windowService;
        private AuditLogger _auditLogger;
        private readonly ISessionService _sessionService;

        public UserViewModel(IWindowService windowService, ISessionService sessionService)
        {
            _windowService = windowService;
            _sessionService = sessionService;

            InitializeCollections();
            InitializeCommands();

            if (_sessionService.IsLoggedIn)
            {
                CurrentUserId = _sessionService.CurrentUserId.Value;
                _auditLogger = new AuditLogger(CurrentUserId, _windowService);

                // Load activities using the audit logger
                _auditLogger.LoadRecentActivities();

                // Log the access using the audit logger
                _auditLogger.LogActivity($"{_sessionService.UserName} accessed User Management");
            }

            LoadUsers();
        }

        private void InitializeCollections()
        {
            _users = new ObservableCollection<User>();
            _filteredUsers = new ObservableCollection<User>();
            PaginatedUsers = new ObservableCollection<User>();
            PageNumbers = new ObservableCollection<int>();
        }

        private void InitializeCommands()
        {
            AddUserCommand = new RelayCommand(ExecuteAddUser);
            EditUserCommand = new RelayCommand<User>(ExecuteEditUser);
            PreviousPageCommand = new RelayCommand(PreviousPage, () => _currentPage > 1);
            NextPageCommand = new RelayCommand(NextPage, () => _currentPage < TotalPages);
            GoToPageCommand = new RelayCommand<int>(GoToPage);
        }

        // User info properties
        private string _fullName;
        public string FullName
        {
            get => _fullName;
            set { _fullName = value; OnPropertyChanged(); }
        }

        private string _role;
        public string Role
        {
            get => _role;
            set { _role = value; OnPropertyChanged(); }
        }

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
                    OnPropertyChanged(nameof(FirstLetter));
                }
            }
        }

        public string FirstLetter => string.IsNullOrWhiteSpace(FirstName) ? "" : FirstName.Substring(0, 1).ToUpper();

        public void UpdateCurrentUser(string fullName, string role)
        {
            FirstName = fullName.Split(' ').FirstOrDefault() ?? "";
            Role = role;
        }

        public void SetCurrentUser(int userId, string fullName, string role)
        {
            CurrentUserId = userId;
            UpdateCurrentUser(fullName, role);
        }

        // Recent activities property (uses AuditLogger's collection)
        public ObservableCollection<string> RecentActivities => _auditLogger?.RecentActivities ?? new ObservableCollection<string>();

        // Log user action using AuditLogger
        public void LogUserAction(string description)
        {
            _auditLogger?.LogActivity(description);
        }

        // Load user data from database
        private void LoadUsers()
        {
            var usersFromDb = new List<User>();
            try
            {
                using (var db = new DbConnections())
                {
                    string query = "SELECT user_id, name, role, email, username, password, createddate FROM tbl_User";
                    var dt = new DataTable();
                    db.readDatathroughAdapter(query, dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        usersFromDb.Add(new User
                        {
                            UserId = Convert.ToInt32(row["user_id"]),
                            Name = row["name"].ToString(),
                            Role = row["role"].ToString(),
                            Email = row["email"].ToString(),
                            Username = row["username"].ToString(),
                            Password = row["password"].ToString(),
                            CreatedDate = row["createddate"] != DBNull.Value ? Convert.ToDateTime(row["createddate"]) : DateTime.Now
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading users: {ex.Message}");
            }

            Users = new ObservableCollection<User>(usersFromDb);
            FilteredUsers = new ObservableCollection<User>(usersFromDb);
            _currentPage = 1;
            UpdatePagination();
        }

        // Commands
        public ICommand AddUserCommand { get; private set; }
        public ICommand EditUserCommand { get; private set; }
        public ICommand PreviousPageCommand { get; private set; }
        public ICommand NextPageCommand { get; private set; }
        public ICommand GoToPageCommand { get; private set; }

        // Search property
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

        // Search filtering
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

        // Command methods
        private void ExecuteAddUser()
        {
            _windowService.ShowAddUserForm();
            LoadUsers();
        }

        private void ExecuteEditUser(User user)
        {
            if (user == null) return;
            _windowService.ShowEditUserForm(user);
            LoadUsers();
        }

        private int TotalPages => (int)Math.Ceiling((double)(FilteredUsers?.Count ?? 0) / _itemsPerPage);

        private void UpdatePagination()
        {
            if (FilteredUsers == null) return;

            var skip = (_currentPage - 1) * _itemsPerPage;
            var paginatedItems = FilteredUsers.Skip(skip).Take(_itemsPerPage).ToList();

            PaginatedUsers.Clear();
            foreach (var item in paginatedItems)
            {
                PaginatedUsers.Add(item);
            }

            PageNumbers.Clear();
            for (int i = 1; i <= TotalPages; i++)
            {
                PageNumbers.Add(i);
            }

            OnPropertyChanged(nameof(PaginatedUsers));
            OnPropertyChanged(nameof(PageNumbers));
        }

        public void OnViewLoaded()
        {
            if (_sessionService.IsLoggedIn && _auditLogger != null)
            {
                _auditLogger.LoadRecentActivities();
                OnPropertyChanged(nameof(RecentActivities));
            }
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
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}