using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace DatabaseProject
{
    class DbConnections : IDisposable
    {
        private SqlConnection connection;
        private SqlCommand command;
        private SqlDataReader DbReader;
        private SqlDataAdapter adapter;
        public SqlTransaction DbTran;
        private string strConnString = "Data Source=DESKTOP-I6QG41I\\SQLEXPRESS;Initial Catalog=DB_HotelM;Integrated Security=True;Trust Server Certificate=True";

        public DbConnections()
        {
            connection = new SqlConnection(strConnString);
            command = new SqlCommand();
            adapter = new SqlDataAdapter();
        }

        public void createConn()
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
        }

        public void closeConn()
        {
            if (connection.State != ConnectionState.Closed)
            {
                connection.Close();
            }
        }

        public int executeDataAdapter(DataTable tblName, string strSelectSql)
        {
            try
            {
                createConn();
                adapter.SelectCommand = new SqlCommand(strSelectSql, connection);
                SqlCommandBuilder DbCommandBuilder = new SqlCommandBuilder(adapter);
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
                createConn();
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    adapter.SelectCommand = cmd;
                    adapter.Fill(tblName);
                }
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
                throw ex;
            }
            finally
            {
                closeConn();
            }
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
                throw ex;
            }
        }
        public void Dispose()
        {
            if (connection != null)
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
                connection.Dispose();
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
                    {
                        foreach (var param in parameters)
                            cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                    }
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
                    {
                        foreach (var param in parameters)
                            cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                    }
                    affectedRows = cmd.ExecuteNonQuery();
                }
                closeConn();
                return affectedRows;
            });
        }
    }
}