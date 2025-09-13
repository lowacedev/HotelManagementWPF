using System.ComponentModel.DataAnnotations;

namespace HotelManagementWPF.Models
{
    public class Room
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(10)]
        public string RoomNumber { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string BedType { get; set; } = string.Empty;
        
        public int RoomFloor { get; set; }
        
        public RoomStatus Status { get; set; }
        
        [StringLength(200)]
        public string? Description { get; set; }
        
        public decimal PricePerNight { get; set; }
        
        public int MaxOccupancy { get; set; }
        
        [StringLength(100)]
        public string? Amenities { get; set; }
        
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    public enum RoomStatus
    {
        Available,
        Booked,
        Reserved,
        Waitlist,
        Blocked
    }
}
