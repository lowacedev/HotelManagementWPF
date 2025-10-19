using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DatabaseProject
{
    class DbConnections : IDisposable
    {
        private SqlConnection connection;
        private SqlCommand command;
        private SqlDataReader DbReader;
        private SqlDataAdapter adapter;
        public SqlTransaction DbTran;

        private readonly string strConnString =
            "Data Source=DESKTOP-8TM8KGG\\SQLEXPRESS;Initial Catalog=DB_HotelM;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";
        private readonly string strOnlineConnString =
            "Server=db28480.public.databaseasp.net; Database=db28480; User Id=db28480; Password=aZ+8=4Kk3Pf%; Encrypt=True; TrustServerCertificate=True;";

        private bool useOnlineDb = false;

        public DbConnections(bool connectToOnlineDb = false)
        {
            useOnlineDb = connectToOnlineDb;
            connection = new SqlConnection(useOnlineDb ? strOnlineConnString : strConnString);
            adapter = new SqlDataAdapter();
            Console.WriteLine($"Initialized DbConnections with {(useOnlineDb ? "Online" : "Local")} database");
        }

        public void SetConnectionToOnline()
        {
            if (connection != null && connection.State == ConnectionState.Open)
                connection.Close();

            connection = new SqlConnection(strOnlineConnString);
            Console.WriteLine("Switched connection to ONLINE database");
        }

        public void SetConnectionToLocal()
        {
            if (connection != null && connection.State == ConnectionState.Open)
                connection.Close();

            connection = new SqlConnection(strConnString);
            Console.WriteLine("Switched connection to LOCAL database");
        }

        public void createConn()
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
                Console.WriteLine("Opened connection to: " + connection.ConnectionString);
            }
        }

        public void closeConn()
        {
            if (connection.State != ConnectionState.Closed)
            {
                connection.Close();
                Console.WriteLine("Closed connection");
            }
        }

        public int executeDataAdapter(DataTable tblName, string strSelectSql)
        {
            try
            {
                createConn();
                using (var cmd = new SqlCommand(strSelectSql, connection))
                {
                    adapter.SelectCommand = cmd;
                    SqlCommandBuilder DbCommandBuilder = new SqlCommandBuilder(adapter);
                    return adapter.Update(tblName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in executeDataAdapter: " + ex.Message);
                throw;
            }
        }

        public void readDatathroughAdapter(string query, DataTable tblName)
        {
            try
            {
                createConn();
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    adapter.SelectCommand = cmd;
                    adapter.Fill(tblName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in readDatathroughAdapter: " + ex.Message);
                throw;
            }
        }

        public int ExecuteNonQuery(string query, Dictionary<string, object> parameters)
        {
            int affectedRows = 0;
            try
            {
                createConn();
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                        }
                    }
                    affectedRows = cmd.ExecuteNonQuery();
                }
                Console.WriteLine("Executed NonQuery: " + query);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in ExecuteNonQuery: " + ex.Message);
                throw;
            }
            finally
            {
                closeConn();
            }
            return affectedRows;
        }

        public void readDataWithParameters(string query, DataTable tblName, Dictionary<string, object> parameters)
        {
            try
            {
                createConn();
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                        }
                    }
                    adapter.SelectCommand = cmd;
                    adapter.Fill(tblName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in readDataWithParameters: " + ex.Message);
                throw;
            }
        }

        public async Task<object> ExecuteScalarAsync(string query, Dictionary<string, object> parameters = null)
        {
            return await Task.Run(() =>
            {
                object result;
                createConn();
                using (var cmd = new SqlCommand(query, connection))
                {
                    if (parameters != null)
                        foreach (var param in parameters)
                            cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                    result = cmd.ExecuteScalar();
                }
                closeConn();
                return result;
            });
        }

        public async Task<int> ExecuteNonQueryAsync(string query, Dictionary<string, object> parameters = null)
        {
            return await Task.Run(() =>
            {
                int affectedRows;
                createConn();
                using (var cmd = new SqlCommand(query, connection))
                {
                    if (parameters != null)
                        foreach (var param in parameters)
                            cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                    affectedRows = cmd.ExecuteNonQuery();
                }
                closeConn();
                return affectedRows;
            });
        }

        public async Task<DataTable> readDataWithParametersAsync(string query, Dictionary<string, object> parameters)
        {
            var dt = new DataTable();
            using (var conn = new SqlConnection(strConnString))
            {
                await conn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (parameters != null)
                        foreach (var param in parameters)
                            cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                    using (var adapter = new SqlDataAdapter(cmd))
                        adapter.Fill(dt);
                }
            }
            return dt;
        }

        // Method to insert data, with online fallback
        public void InsertDataWithSync(string tableName, Dictionary<string, object> data)
        {
            string columns = string.Join(", ", data.Keys);
            string paramNames = string.Join(", ", data.Keys.Select(k => "@" + k));
            string insertSql = $"INSERT INTO {tableName} ({columns}) VALUES ({paramNames})";

            // Insert into local DB
            ExecuteNonQuery(insertSql, data);

            if (useOnlineDb)
            {
                try
                {
                    // Insert into online DB
                    SetConnectionToOnline();
                    createConn();
                    using (var onlineCmd = new SqlCommand(insertSql, connection))
                    {
                        foreach (var kvp in data)
                        {
                            onlineCmd.Parameters.AddWithValue("@" + kvp.Key, kvp.Value ?? DBNull.Value);
                        }
                        onlineCmd.ExecuteNonQuery();
                        Console.WriteLine("Inserted data into online database");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error inserting into online DB: " + ex.Message);
                    // On failure, store for sync
                    string jsonData = JsonConvert.SerializeObject(data);
                    string pendingInsertSql = "INSERT INTO PendingSync (TableName, Data) VALUES (@TableName, @Data)";
                    ExecuteNonQuery(pendingInsertSql, new Dictionary<string, object>
                    {
                        { "@TableName", tableName },
                        { "@Data", jsonData }
                    });
                }
            }
        }

        // Sync all pending data
        public void SyncPendingData()
        {
            try
            {
                // Load all pending records
                DataTable pendingRecords = new DataTable();
                string selectPendingSql = "SELECT * FROM PendingSync";

                using (var cmd = new SqlCommand(selectPendingSql, new SqlConnection(strConnString)))
                {
                    using (var adapter = new SqlDataAdapter(cmd))
                        adapter.Fill(pendingRecords);
                }

                // Process each record
                SetConnectionToOnline();
                createConn();

                foreach (DataRow row in pendingRecords.Rows)
                {
                    string tableName = row["TableName"].ToString();
                    string jsonData = row["Data"].ToString();

                    var dataDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonData);
                    string columns = string.Join(", ", dataDict.Keys);
                    string paramNames = string.Join(", ", dataDict.Keys.Select(k => "@" + k));
                    string insertSql = $"INSERT INTO {tableName} ({columns}) VALUES ({paramNames})";

                    using (var cmd = new SqlCommand(insertSql, connection))
                    {
                        foreach (var kvp in dataDict)
                        {
                            cmd.Parameters.AddWithValue("@" + kvp.Key, kvp.Value ?? DBNull.Value);
                        }
                        cmd.ExecuteNonQuery();
                        Console.WriteLine($"Synced pending data for table {tableName}");
                    }
                }

                // Clear pending records after success
                string deleteSql = "DELETE FROM PendingSync";
                using (var deleteCmd = new SqlCommand(deleteSql, connection))
                {
                    deleteCmd.ExecuteNonQuery();
                }
                Console.WriteLine("Cleared pending sync records");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in SyncPendingData: " + ex.Message);
                // Keep pending records if error
            }
        }
        public async Task readDatathroughAdapterAsync(string query, DataTable tblName)
        {
            try
            {
                using (var conn = new SqlConnection(useOnlineDb ? strOnlineConnString : strConnString))
                {
                    await conn.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            await Task.Run(() => adapter.Fill(tblName));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in readDatathroughAdapterAsync: " + ex.Message);
                throw;
            }
        }
        public void InsertUserActivity(int userId, string activityDescription)
        {
            try
            {
                createConn();

                // First, verify the user exists
                string checkUserQuery = "SELECT COUNT(1) FROM tbl_User WHERE user_id = @UserId";
                using (SqlCommand checkCmd = new SqlCommand(checkUserQuery, connection))
                {
                    checkCmd.Parameters.AddWithValue("@UserId", userId);
                    var userExists = (int)checkCmd.ExecuteScalar();

                    if (userExists == 0)
                    {
                        throw new Exception($"User with ID {userId} does not exist in tbl_User.");
                    }
                }

                // Now insert the activity
                string query = "INSERT INTO tbl_UserActivityLog (user_id, ActivityDescription, ActivityTimestamp) VALUES (@UserId, @ActivityDescription, GETDATE())";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@ActivityDescription", activityDescription);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in InsertUserActivity: " + ex.Message);
                throw; // Re-throw so caller can handle it
            }
            finally
            {
                closeConn();
            }
        }
        public void Dispose()
        {
            if (connection != null)
            {
                if (connection.State != ConnectionState.Closed)
                    connection.Close();
                connection.Dispose();
            }
        }
    }
}