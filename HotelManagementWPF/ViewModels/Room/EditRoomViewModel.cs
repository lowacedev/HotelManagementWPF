using HotelManagementWPF.Models;
using HotelManagementWPF.ViewModels.Base;
using HotelManagementWPF.Views.Room;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace HotelManagementWPF.ViewModels
{
    public class EditRoomViewModel : INotifyPropertyChanged
    {
        private Models.Room _originalRoom;
        private string _roomNumber;
        private string _bedType;
        private decimal _price;
        private RoomStatus _status;

        public EditRoomViewModel(Models.Room room)
        {
            _originalRoom = room;

            // Pre-populate the form with existing data
            RoomNumber = room.RoomNumber;
            BedType = room.BedType;
            Price = room.Price;
            Status = room.Status;

            SaveChangesCommand = new RelayCommand(SaveChanges);
        }

        // Properties for data binding
        public string RoomNumber
        {
            get => _roomNumber;
            set { _roomNumber = value; OnPropertyChanged(); }
        }

        public string BedType
        {
            get => _bedType;
            set { _bedType = value; OnPropertyChanged(); }
        }

        public decimal Price
        {
            get => _price;
            set { _price = value; OnPropertyChanged(); }
        }

        public RoomStatus Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(); }
        }

        // Options for ComboBoxes
        public List<string> BedTypeOptions => new List<string>
        {
            "Single", "Double", "Queen", "King", "Twin", "Suite", "Deluxe", "Presidential Suite"
        };

        public List<RoomStatus> StatusOptions => new List<RoomStatus>
        {
            RoomStatus.Available, RoomStatus.Booked, RoomStatus.Reserved,
            RoomStatus.Waitlist, RoomStatus.Blocked
        };

        public ICommand SaveChangesCommand { get; }

        private void SaveChanges()
        {
            // Update the original room with new values
            _originalRoom.RoomNumber = RoomNumber;
            _originalRoom.BedType = BedType;
            _originalRoom.Price = Price;
            _originalRoom.Status = Status;
            _originalRoom.ModifiedDate = DateTime.Now;

            // Here you would typically save to database
            // For now, we'll just show a message and close
            System.Windows.MessageBox.Show(
                $"Room {RoomNumber} updated successfully!",
                "Success",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Information);

            // Close the window with success result
            if (System.Windows.Application.Current.MainWindow is System.Windows.Window window)
            {
                foreach (System.Windows.Window w in System.Windows.Application.Current.Windows)
                {
                    if (w is EditRoomFormView editForm && editForm.DataContext == this)
                    {
                        editForm.DialogResult = true;
                        editForm.Close();
                        break;
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}