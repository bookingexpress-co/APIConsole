using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace APIConsole.Supplier.DataAccess
{
    public class TravayooDataAcess
    {
        private  string sqlconnectionstring;
        public TravayooDataAcess(string _connection)
        {
            sqlconnectionstring = _connection;

        }
        private  SqlConnection OpenConnection()
        {
            SqlConnection objsqlconnection;
            try
            {
                objsqlconnection = new SqlConnection();
                objsqlconnection =this.GetConnection();
                objsqlconnection.Open();
                return objsqlconnection;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        private  SqlConnection GetConnection()
        {
            try
            {
                return new SqlConnection(sqlconnectionstring);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        private  void CloseConnection(SqlConnection con)
        {
            try
            {
                if ((con != null) && (con.State & ConnectionState.Open) == ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();
                }
            }
            catch
            {
                con = null;
            }
        }

        public  DataTable Get(string pQueryCode, params SqlParameter[] pParameterValues)
        {
            try
            {
                DataTable vDataTable = new DataTable();
                using (SqlConnection vSqlConnection = this.OpenConnection())
                {
                    using (SqlCommand vSqlCommand = new SqlCommand(pQueryCode, vSqlConnection))
                    {
                        vSqlCommand.CommandType = CommandType.StoredProcedure;
                        foreach (SqlParameter vSqlParameter in pParameterValues)
                        {
                            vSqlCommand.Parameters.Add(vSqlParameter);
                        }
                        using (SqlDataAdapter vSqlDataAdapter = new SqlDataAdapter(vSqlCommand))
                        {
                            vSqlDataAdapter.Fill(vDataTable);
                            return vDataTable;
                        }
                    }
                }
            }
            catch (Exception xe)
            {
                throw new Exception(xe.ToString());
            }

        }

        public  int Insert(string pQueryCode, params SqlParameter[] pParameterValues)
        {
            try
            {
                using (SqlConnection vSqlConnection = this.OpenConnection())
                {
                    using (SqlCommand vSqlCommand = new SqlCommand(pQueryCode, vSqlConnection))
                    {
                        vSqlCommand.CommandType = CommandType.StoredProcedure;
                        foreach (SqlParameter vSqlParameter in pParameterValues)
                        {
                            vSqlCommand.Parameters.Add(vSqlParameter);
                        }
                        int row = vSqlCommand.ExecuteNonQuery();
                        CloseConnection(vSqlConnection);
                        return row;
                    }
                }
            }
            catch (Exception xe)
            {
                throw new Exception(xe.ToString());
            }
        }

    }
}
