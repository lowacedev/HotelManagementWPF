using DatabaseProject;
using HotelManagementWPF.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;

namespace HotelManagementWPF.Models
{
    public class AuditLogger
    {
        private readonly IWindowService _windowService;
        private readonly int _currentUserId;
        public ObservableCollection<string> RecentActivities { get; private set; }

        public AuditLogger(int userId, IWindowService windowService)
        {
            _currentUserId = userId;
            _windowService = windowService;
            RecentActivities = new ObservableCollection<string>();
        }

        public void LogActivity(string description)
        {
            // Add to recent activities list
            RecentActivities.Insert(0, $"{DateTime.Now:HH:mm:ss} - {description}");

            // Save to database
            try
            {
                using (var db = new DbConnections())
                {
                    db.InsertUserActivity(_currentUserId, description);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error logging activity: {ex.Message}");
            }
        }

        public void LoadRecentActivities()
        {
            try
            {
                using (var db = new DbConnections())
                {
                    string query = "SELECT ActivityDescription, ActivityTimestamp " +
                                   "FROM tbl_UserActivityLog " +
                                   "WHERE user_id = @UserId " +
                                   "ORDER BY ActivityTimestamp DESC";

                    var parameters = new Dictionary<string, object>
                    {
                        { "@UserId", _currentUserId }
                    };

                    var dt = new DataTable();
                    db.readDataWithParameters(query, dt, parameters);

                    RecentActivities.Clear();

                    foreach (DataRow row in dt.Rows)
                    {
                        var desc = row["ActivityDescription"].ToString();
                        var timestamp = Convert.ToDateTime(row["ActivityTimestamp"]);
                        RecentActivities.Add($"{timestamp:HH:mm} - {desc}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading recent activities: {ex.Message}");
            }
        }
    }
}