using System.Windows.Input;
using HotelManagementWPF.ViewModels.Base;
using HotelManagementWPF.Models;


namespace HotelManagementWPF.ViewModels.Users
{
    public class EditUserFormViewModel
    {
        public ICommand SaveChangesCommand { get; }
        public ICommand CancelCommand { get; }
        private User _user;
        public User User
        {
            get => _user;
            set => _user = value;
        }
        public EditUserFormViewModel(User user)
        {
            _user = user;
            SaveChangesCommand = new RelayCommand(ExecuteSaveChanges);
            CancelCommand = new RelayCommand(ExecuteCancel);
        }

        private void ExecuteSaveChanges()
        {
            // your save logic
        }

        private void ExecuteCancel()
        {
            // MVVM-friendly way to close:
            // - send a message/event
            // - or call a service that closes the window
            // - or if you allow minimal code-behind: just Close() the window there
        }
    }
}
