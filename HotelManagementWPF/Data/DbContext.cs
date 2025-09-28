using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HotelManagementWPF.Data
{
    public class DbContext
    {
        private readonly string _connectionString;

        public DbContext()
        {
            _connectionString = "Data Source=DESKTOP-8TM8KGG\\SQLEXPRESS;Initial Catalog=DB_HotelM;Integrated Security=True;Encrypt=True;Trust Server Certificate=True"; 
        }

        public SqlConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public async Task<int> ExecuteNonQueryAsync(string query, Dictionary<string, object> parameters)
        {
            using (var connection = CreateConnection())
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                        }
                    }

                    return await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<object> ExecuteScalarAsync(string query, Dictionary<string, object> parameters)
        {
            using (var connection = CreateConnection())
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                        }
                    }

                    return await command.ExecuteScalarAsync();
                }
            }
        }
    }
}