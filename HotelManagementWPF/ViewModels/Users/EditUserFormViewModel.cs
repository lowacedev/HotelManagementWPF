using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using HotelManagementWPF.ViewModels.Base;
using HotelManagementWPF.Models;

namespace HotelManagementWPF.ViewModels.Users
{
    public class EditUserFormViewModel : INotifyPropertyChanged
    {
        public ICommand SaveChangesCommand { get; }
        public ICommand CancelCommand { get; }

        private User _user;
        private string _name;
        private string _email;
        private string _role;

        // Properties for binding to UI
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        public string Role
        {
            get => _role;
            set
            {
                _role = value;
                OnPropertyChanged(nameof(Role));
            }
        }

        // Role options for the dropdown
        public ObservableCollection<string> RoleOptions { get; }

        public User User
        {
            get => _user;
            set => _user = value;
        }

        public EditUserFormViewModel(User user)
        {
            _user = user;
            
            // Initialize role options
            RoleOptions = new ObservableCollection<string>
            {
                "Admin",
                "Manager", 
                "Receptionist",
                "Staff",
                "Guest"
            };

            // Pre-fill form with existing user data
            if (user != null)
            {
                Name = user.Name;
                Email = user.Email;
                Role = user.Role;
            }

            SaveChangesCommand = new RelayCommand(ExecuteSaveChanges);
            CancelCommand = new RelayCommand(ExecuteCancel);
        }

        private void ExecuteSaveChanges()
        {
            if (_user != null)
            {
                // Update user properties
                _user.Name = Name;
                _user.Email = Email;
                _user.Role = Role;
            }

            // Close the window with DialogResult = true
            var window = Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.DataContext == this);
            
            if (window != null)
            {
                window.DialogResult = true;
                window.Close();
            }
        }

        private void ExecuteCancel()
        {
            // Close the window with DialogResult = false
            var window = Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.DataContext == this);
            
            if (window != null)
            {
                window.DialogResult = false;
                window.Close();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}