using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using DatabaseProject;
namespace DatabaseProject
{
    class DbConnections
    {
        private static SqlConnection connection = new SqlConnection();
        private static SqlCommand command = new SqlCommand();
        private static SqlDataReader DbReader;
        private static SqlDataAdapter adapter = new SqlDataAdapter();
        public SqlTransaction DbTran;
        private static string strConnString = "Data Source=DESKTOP-8TM8KGG\\SQLEXPRESS;Initial Catalog=DB_HotelM;Integrated Security=True;Trust Server Certificate=True";


        public void createConn()
        {
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.ConnectionString = strConnString;
                    connection.Open();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void closeConn()
        {
            connection.Close();
        }
        public int executeDataAdapter(DataTable tblName, string strSelectSql)
        {
            try
            {
                if (connection.State == 0)
                {
                    createConn();
                }
                adapter.SelectCommand.CommandText = strSelectSql;
                adapter.SelectCommand.CommandType = CommandType.Text;
                SqlCommandBuilder DbCommandBuilder = new SqlCommandBuilder(adapter);
                string insert = DbCommandBuilder.GetInsertCommand().CommandText.ToString();
                string update = DbCommandBuilder.GetUpdateCommand().CommandText.ToString();
                string delete = DbCommandBuilder.GetDeleteCommand().CommandText.ToString();
                return adapter.Update(tblName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void readDatathroughAdapter(string query, DataTable tblName)
        {
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    createConn();
                }
                command.Connection = connection;
                command.CommandText = query;
                command.CommandType = CommandType.Text;
                adapter = new SqlDataAdapter(command);
                adapter.Fill(tblName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void ExecuteNonQuery(string query, Dictionary<string, object> parameters)
        {
            try
            {
                createConn();
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    foreach (var param in parameters)
                    {
                        cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                    }
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                // Handle exception (log or throw)
                throw ex;
            }
            finally
            {
                closeConn();
            }
        }
    }
}