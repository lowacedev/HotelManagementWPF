using DatabaseProject;
using HotelManagementWPF.Models;
using System.Collections.ObjectModel;
using System.Data;
using System.Threading.Tasks;

public class HousekeepingViewModel
{
    public ObservableCollection<HousekeepingTask> Tasks { get; set; } = new ObservableCollection<HousekeepingTask>();

    public async Task LoadDataAsync()
    {
        var db = new DbConnections();
        string query = @"
        SELECT 
             r.roomNumber, 
            s.name AS StaffName, 
            h.taskDate, 
            h.taskType, 
            h.status, 
            h.notes
        FROM tbl_HouseKeeping h
        JOIN tbl_Room r ON h.room_id = r.room_id
        JOIN tbl_Staff s ON h.staff_id = s.staff_id";

        var dt = await db.readDataWithParametersAsync(query, null);

        Tasks.Clear();
        foreach (DataRow row in dt.Rows)
        {
            Tasks.Add(new HousekeepingTask
            {
                RoomNumber = Convert.ToInt32(row["roomNumber"]),
                StaffName = row["StaffName"].ToString(),
                TaskDate = Convert.ToDateTime(row["taskDate"]),
                TaskType = row["taskType"].ToString(),
                Status = row["status"].ToString(),
                Notes = row["notes"] == DBNull.Value ? null : row["notes"].ToString()
            });
        }
    }
}