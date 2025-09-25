using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using HotelManagementWPF.ViewModels.Base;

namespace HotelManagementWPF.ViewModels.Booking
{
    public class BookingFormViewModel : INotifyPropertyChanged
    {
        // Guest Information
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int? Age { get; set; }
        public string Gender { get; set; } = "Male";
        public ObservableCollection<string> GenderOptions { get; } = new() { "Male", "Female", "Other" };

        // Payment Details
        public string PaymentMethod { get; set; } = "Cash";
        public ObservableCollection<string> PaymentMethodOptions { get; } = new() { "Cash", "Credit Card", "Bank Transfer", "Online Payment" };
        public decimal AdvancedPayment { get; set; } = 0;
        public decimal TotalAmount { get; set; } = 0;
        public string PaymentStatus { get; set; } = "Pending";
        public ObservableCollection<string> PaymentStatusOptions { get; } = new() { "Pending", "Partial", "Completed" };

        // Stay Details
        public string RoomNumber { get; set; } = string.Empty;
        public string RoomType { get; set; } = string.Empty;
        public ObservableCollection<string> RoomTypeOptions { get; } = new() { "Deluxe Suite", "Standard Room", "Executive Room" };
        public DateTime CheckInDate { get; set; } = DateTime.Today;
        public DateTime CheckOutDate { get; set; } = DateTime.Today.AddDays(1);
        public int NumberOfNights => (CheckOutDate > CheckInDate) ? (CheckOutDate - CheckInDate).Days : 0;

        public ICommand CancelCommand { get; set; }
        public ICommand BookRoomCommand { get; set; }

        public BookingFormViewModel()
        {
            CancelCommand = new RelayCommand(() => OnCancel());
            BookRoomCommand = new RelayCommand(() => OnBookRoom());
        }

        public void PreFill(string roomNumber, string roomType, DateTime checkInDate)
        {
            RoomNumber = roomNumber;
            RoomType = roomType;
            CheckInDate = checkInDate;
            CheckOutDate = checkInDate.AddDays(1);
            OnPropertyChanged(nameof(RoomNumber));
            OnPropertyChanged(nameof(RoomType));
            OnPropertyChanged(nameof(CheckInDate));
            OnPropertyChanged(nameof(CheckOutDate));
            OnPropertyChanged(nameof(NumberOfNights));
        }

        private void OnCancel()
        {
            // Close dialog logic (handled in view)
        }

        private void OnBookRoom()
        {
            // Booking logic (validation, save, close dialog)
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
