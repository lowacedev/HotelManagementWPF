using HotelManagementWPF.Models;

namespace HotelManagementWPF.Services.Interfaces
{
    public interface IRoomService
    {
        Task<IEnumerable<Room>> GetAllRoomsAsync();
        Task<Room?> GetRoomByIdAsync(int id);
        Task<Room> CreateRoomAsync(Room room);
        Task<Room> UpdateRoomAsync(Room room);
        Task<bool> DeleteRoomAsync(int id);
        Task<IEnumerable<Room>> GetRoomsByStatusAsync(RoomStatus status);
        Task<int> GetRoomCountByStatusAsync(RoomStatus status);
        Task<int> GetTotalRoomCountAsync();
    }
}
