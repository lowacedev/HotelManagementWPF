using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using HotelManagementWPF.Models;

namespace HotelManagementWPF.ViewModels.Booking
{
    public class BookingViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<RoomResource> Rooms { get; set; }
        public ObservableCollection<BookingAppointment> Bookings { get; set; }
        public AppointmentMapping AppointmentMapping { get; set; }
        public ResourceMapping ResourceMapping { get; set; }

        public BookingViewModel()
        {
            Rooms = new ObservableCollection<RoomResource>
            {
                new RoomResource { Id = 1, Name = "Room 1", Type = "Deluxe Suite" },
                new RoomResource { Id = 2, Name = "Room 2", Type = "Standard Room" },
                new RoomResource { Id = 3, Name = "Room 3", Type = "Executive Room" },
                new RoomResource { Id = 4, Name = "Room 4", Type = "Deluxe Suite" },
                new RoomResource { Id = 5, Name = "Room 5", Type = "Standard Room" },
                new RoomResource { Id = 6, Name = "Room 6", Type = "Standard Room" },
                new RoomResource { Id = 7, Name = "Room 7", Type = "Executive Room" },
                new RoomResource { Id = 8, Name = "Room 8", Type = "Deluxe Suite" }
            };

            Bookings = new ObservableCollection<BookingAppointment>
            {
                new BookingAppointment {
                    Subject = "Said el hadrry",
                    StartTime = new DateTime(2025,2,9,9,2,0),
                    EndTime = new DateTime(2025,2,9,11,2,0),
                    RoomId = 1,
                    StatusColor = "#FFA726"
                },
                new BookingAppointment {
                    Subject = "Said el hadrry",
                    StartTime = new DateTime(2025,2,10,9,2,0),
                    EndTime = new DateTime(2025,2,19,9,2,0),
                    RoomId = 2,
                    StatusColor = "#2196F3"
                },
                new BookingAppointment {
                    Subject = "Jane cooper",
                    StartTime = new DateTime(2025,2,11,11,2,0),
                    EndTime = new DateTime(2025,2,16,11,2,0),
                    RoomId = 4,
                    StatusColor = "#2196F3"
                },
                new BookingAppointment {
                    Subject = "Said el hadrry",
                    StartTime = new DateTime(2025,2,1,9,2,0),
                    EndTime = new DateTime(2025,2,9,9,2,0),
                    RoomId = 6,
                    StatusColor = "#FF5722"
                }
            };

            AppointmentMapping = new AppointmentMapping
            {
                Subject = nameof(BookingAppointment.Subject),
                StartTime = nameof(BookingAppointment.StartTime),
                EndTime = nameof(BookingAppointment.EndTime),
                ResourceId = nameof(BookingAppointment.RoomId),
                AppointmentBackground = nameof(BookingAppointment.StatusColor)
            };

            ResourceMapping = new ResourceMapping
            {
                Id = nameof(RoomResource.Id),
                Name = nameof(RoomResource.Name)
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class RoomResource
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string Type { get; set; }
    }

    public class BookingAppointment
    {
        public required string Subject { get; set; }
        public required DateTime StartTime { get; set; }
        public required DateTime EndTime { get; set; }
        public required int RoomId { get; set; }
        public required string StatusColor { get; set; }
    }

    public class AppointmentMapping
    {
        public required string Subject { get; set; }
        public required string StartTime { get; set; }
        public required string EndTime { get; set; }
        public required string ResourceId { get; set; }
        public required string AppointmentBackground { get; set; }
    }

    public class ResourceMapping
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
    }
}
