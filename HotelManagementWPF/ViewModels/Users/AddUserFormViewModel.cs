using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using HotelManagementWPF.ViewModels.Base;

namespace HotelManagementWPF.ViewModels
{
    public class AddUserFormViewModel : INotifyPropertyChanged
    {
        private string _name = string.Empty;
        private string _email = string.Empty;
        private string _role = string.Empty;
        public string Username { get; set; }
        public string Password { get; set; }
        public AddUserFormViewModel()
        {
            InitializeCommands();
            InitializeRoleOptions();
        }

        private void InitializeCommands()
        {
            AddUserCommand = new RelayCommand(ExecuteAddUser, CanAddUser);
        }

        private void InitializeRoleOptions()
        {
            RoleOptions = new ObservableCollection<string>
            {
                "Administrator",
                "Manager",
                "Supervisor",
                "Receptionist",
                "Staff"
            };
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
                ((RelayCommand)AddUserCommand).RaiseCanExecuteChanged();
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
                ((RelayCommand)AddUserCommand).RaiseCanExecuteChanged();
            }
        }

        public string Role
        {
            get => _role;
            set
            {
                _role = value;
                OnPropertyChanged();
                ((RelayCommand)AddUserCommand).RaiseCanExecuteChanged();
            }
        }

        public ObservableCollection<string> RoleOptions { get; set; }

        public ICommand AddUserCommand { get; private set; }

        private bool CanAddUser()
        {
            return !string.IsNullOrWhiteSpace(Name) &&
                   !string.IsNullOrWhiteSpace(Email) &&
                   !string.IsNullOrWhiteSpace(Role) &&
                   IsValidEmail(Email);
        }

        private void ExecuteAddUser()
        {
            try
            {
                // Create new user object
                var newUser = new User
                {
                    Name = Name.Trim(),
                    Email = Email.Trim().ToLower(),
                    Role = Role,
                    CreatedDate = DateTime.Now
                };

                // Here you would typically:
                // 1. Save to database
                // 2. Add to the users collection
                // 3. Show success message
                // 4. Close the form

                // For now, just simulate success
                System.Windows.MessageBox.Show("User added successfully!", "Success",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);

                // Close the window
                CloseWindow();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error adding user: {ex.Message}", "Error",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void CloseWindow()
        {
            // This would typically be handled by the view or a window service
            // For now, we'll rely on the view to handle window closing
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}