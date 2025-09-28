using DatabaseProject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementWPF.Helper
{
    public class UserRepository
    {
        public bool UserExists(int userId)
        {
            using (var db = new DbConnections())
            {
                string query = "SELECT COUNT(*) FROM tbl_User WHERE user_id = @UserId";
                var parameters = new Dictionary<string, object>
                {
                    { "@UserId", userId }
                };
                int count = (int)db.ExecuteScalarAsync(query, parameters).Result;
                return count > 0;
            }
        }

        public int InsertUser(string fullName, string role, string email, string username, string password)
        {
            using (var db = new DbConnections())
            {
                string insertQuery = @"
                    INSERT INTO tbl_User (name, role, email, username, password, createddate)
                    VALUES (@Name, @Role, @Email, @Username, @Password, GETDATE());
                    SELECT SCOPE_IDENTITY();";

                var parameters = new Dictionary<string, object>
                {
                    { "@Name", fullName },
                    { "@Role", role },
                    { "@Email", email },
                    { "@Username", username },
                    { "@Password", password }
                };

                var result = db.ExecuteScalarAsync(insertQuery, parameters).Result;
                int newUserId = Convert.ToInt32(result);
                return newUserId;
            }
        }
    }
}