using HotelManagementWPF.Models;
using HotelManagementWPF.Services.Interfaces;

namespace HotelManagementWPF.Services
{
    public class RoomService : IRoomService
    {
        private readonly List<Room> _rooms = new();
        private int _nextId = 1;

        public RoomService()
        {
            InitializeSampleData();
        }

        public async Task<IEnumerable<Room>> GetAllRoomsAsync()
        {
            await Task.Delay(100); // Simulate async operation
            return _rooms.ToList();
        }

        public async Task<Room?> GetRoomByIdAsync(int id)
        {
            await Task.Delay(50);
            return _rooms.FirstOrDefault(r => r.Id == id);
        }

        public async Task<Room> CreateRoomAsync(Room room)
        {
            await Task.Delay(100);
            room.Id = _nextId++;
            room.CreatedDate = DateTime.Now;
            _rooms.Add(room);
            return room;
        }

        public async Task<Room> UpdateRoomAsync(Room room)
        {
            await Task.Delay(100);
            var existingRoom = _rooms.FirstOrDefault(r => r.Id == room.Id);
            if (existingRoom != null)
            {
                room.ModifiedDate = DateTime.Now;
                var index = _rooms.IndexOf(existingRoom);
                _rooms[index] = room;
            }
            return room;
        }

        public async Task<bool> DeleteRoomAsync(int id)
        {
            await Task.Delay(50);
            var room = _rooms.FirstOrDefault(r => r.Id == id);
            if (room != null)
            {
                _rooms.Remove(room);
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<Room>> GetRoomsByStatusAsync(RoomStatus status)
        {
            await Task.Delay(50);
            return _rooms.Where(r => r.Status == status).ToList();
        }

        public async Task<int> GetRoomCountByStatusAsync(RoomStatus status)
        {
            await Task.Delay(50);
            return _rooms.Count(r => r.Status == status);
        }

        public async Task<int> GetTotalRoomCountAsync()
        {
            await Task.Delay(50);
            return _rooms.Count;
        }

        private void InitializeSampleData()
        {
            var bedTypes = new[] { "Single", "Double", "Queen", "King", "Twin" };
            var statuses = new[] { RoomStatus.Available, RoomStatus.Booked, RoomStatus.Reserved, RoomStatus.Waitlist, RoomStatus.Blocked };
            var random = new Random();

            for (int i = 1; i <= 50; i++)
            {
                var floor = (i - 1) / 10 + 1;
                var roomNumber = $"{floor}{i % 10:D2}";
                
                _rooms.Add(new Room
                {
                    Id = i,
                    RoomNumber = roomNumber,
                    BedType = bedTypes[random.Next(bedTypes.Length)],
                    RoomFloor = floor,
                    Status = statuses[random.Next(statuses.Length)],
                    Description = $"Comfortable room on floor {floor}",
                    PricePerNight = 100 + random.Next(200),
                    MaxOccupancy = 2 + random.Next(3),
                    Amenities = "WiFi, TV, Mini Bar, Air Conditioning",
                    CreatedDate = DateTime.Now.AddDays(-random.Next(365))
                });
            }
        }
    }
}
