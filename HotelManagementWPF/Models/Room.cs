using System;
using System.ComponentModel.DataAnnotations;

namespace HotelManagementWPF.Models
{

    public enum RoomStatus
    {
        Available,
        Booked,
        Reserved,
        Waitlist,
        Blocked
    }

    public class Room
    {

        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        public string RoomNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string BedType { get; set; } = string.Empty;

        // Kept for compatibility with various UIs that bind to Price
        public decimal PricePerNight { get; set; }

        // Property for setting/getting Price, if needed
        public decimal Price
        {
            get => PricePerNight;
            set => PricePerNight = value;
        }

        public int RoomFloor { get; set; }

        public RoomStatus Status { get; set; }

        [StringLength(200)]
        public string? Description { get; set; }

        public int MaxOccupancy { get; set; }

        [StringLength(100)]
        public string? Amenities { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        // New property for formatted price with Peso sign
        public string FormattedPrice => $"₱{PricePerNight:N2}";

        public int RoomId { get; internal set; }
    }
}