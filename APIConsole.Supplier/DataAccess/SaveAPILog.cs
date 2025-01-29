using APIConsole.Supplier.Models.Common;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace APIConsole.Supplier.DataAccess
{
    public class SaveAPILog : IDisposable
    {
        public SqlConnection con;
        SqlConnection conn;
        SqlCommand cmd;
        SqlDataAdapter adap;
        private void connecttion()
        {
            string constr = ConfigurationManager.ConnectionStrings["INGMContext"].ToString();
            con = new SqlConnection(constr);
            if (con.State != ConnectionState.Open)
            {
                con.Open();
            }
        }
        public void SaveAPILogs(APILogDetail apilog)
        {
            try
            {
                using (conn = new SqlConnection(ConfigurationManager.ConnectionStrings["INGMContext"].ToString()))
                {
                    using (cmd = new SqlCommand("SP_InsertAPILog", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@customerID", apilog.customerID);
                        cmd.Parameters.AddWithValue("@TrackNumber", apilog.TrackNumber);
                        cmd.Parameters.AddWithValue("@logTypeID", apilog.LogTypeID);
                        cmd.Parameters.AddWithValue("@logType", apilog.LogType);
                        cmd.Parameters.AddWithValue("@SupplierID", apilog.SupplierID);
                        cmd.Parameters.AddWithValue("@logMsg", "");
                        cmd.Parameters.AddWithValue("@logrequestXML", apilog.logrequestXML);
                        cmd.Parameters.AddWithValue("@logresponseXML", apilog.logresponseXML);
                        cmd.Parameters.AddWithValue("@logStatus", apilog.logStatus);
                        cmd.Parameters.AddWithValue("@StartTime", apilog.StartTime);
                        cmd.Parameters.AddWithValue("@EndTime", apilog.EndTime);
                        cmd.Parameters.Add("@retVal", SqlDbType.Int);
                        cmd.Parameters["@retVal"].Direction = ParameterDirection.Output;
                        if (cmd.Connection.State == System.Data.ConnectionState.Closed)
                        {
                            cmd.Connection.Open();
                        }
                        cmd.ExecuteNonQuery();
                    }

                }
            }
            catch (FormatException ex)
            {
                //throw ex;
            }
        }














        public void SaveAPILogs_search(APILogDetail apilog)
        {
            try
            {
                using (conn = new SqlConnection(ConfigurationManager.ConnectionStrings["INGMContext"].ToString()))
                {
                    using (cmd = new SqlCommand("USP_InsertAPILog_search", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@customerID", apilog.customerID);
                        cmd.Parameters.AddWithValue("@TrackNumber", apilog.TrackNumber);
                        cmd.Parameters.AddWithValue("@logTypeID", apilog.LogTypeID);
                        cmd.Parameters.AddWithValue("@logType", apilog.LogType);
                        cmd.Parameters.AddWithValue("@SupplierID", apilog.SupplierID);
                        cmd.Parameters.AddWithValue("@logMsg", "");
                        cmd.Parameters.AddWithValue("@logrequestXML", apilog.logrequestXML);
                        cmd.Parameters.AddWithValue("@logresponseXML", apilog.logresponseXML);
                        cmd.Parameters.AddWithValue("@logStatus", apilog.logStatus);
                        cmd.Parameters.AddWithValue("@StartTime", apilog.StartTime);
                        cmd.Parameters.AddWithValue("@EndTime", apilog.EndTime);
                        cmd.Parameters.Add("@retVal", SqlDbType.Int);
                        cmd.Parameters["@retVal"].Direction = ParameterDirection.Output;
                        if (cmd.Connection.State == System.Data.ConnectionState.Closed)
                        {
                            cmd.Connection.Open();
                        }
                        cmd.ExecuteNonQuery();
                    }

                }
            }
            catch (FormatException ex)
            {
                //throw ex;
            }
        }
        public void SaveAPILogs_search_iol(APILogDetail apilog)
        {
            try
            {
                using (conn = new SqlConnection(ConfigurationManager.ConnectionStrings["INGMContext"].ToString()))
                {
                    using (cmd = new SqlCommand("USP_InsertAPILog_search_iol", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@customerID", apilog.customerID);
                        cmd.Parameters.AddWithValue("@TrackNumber", apilog.TrackNumber);
                        cmd.Parameters.AddWithValue("@logTypeID", apilog.LogTypeID);
                        cmd.Parameters.AddWithValue("@logType", apilog.LogType);
                        cmd.Parameters.AddWithValue("@SupplierID", apilog.SupplierID);
                        cmd.Parameters.AddWithValue("@logMsg", "");
                        cmd.Parameters.AddWithValue("@logrequestXML", apilog.logrequestXML);
                        cmd.Parameters.AddWithValue("@logresponseXML", apilog.logresponseXML);
                        cmd.Parameters.AddWithValue("@logStatus", apilog.logStatus);
                        cmd.Parameters.AddWithValue("@StartTime", apilog.StartTime);
                        cmd.Parameters.AddWithValue("@EndTime", apilog.EndTime);
                        cmd.Parameters.AddWithValue("@preID", apilog.preID);
                        cmd.Parameters.Add("@retVal", SqlDbType.Int);
                        cmd.Parameters["@retVal"].Direction = ParameterDirection.Output;
                        if (cmd.Connection.State == System.Data.ConnectionState.Closed)
                        {
                            cmd.Connection.Open();
                        }
                        cmd.ExecuteNonQuery();
                    }

                }
            }
            catch (FormatException ex)
            {
                //throw ex;
            }
        }
        public void SaveAPILogs_room(APILogDetail apilog)
        {
            try
            {
                using (conn = new SqlConnection(ConfigurationManager.ConnectionStrings["INGMContext"].ToString()))
                {
                    using (cmd = new SqlCommand("USP_InsertAPILog_room", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@customerID", apilog.customerID);
                        cmd.Parameters.AddWithValue("@TrackNumber", apilog.TrackNumber);
                        cmd.Parameters.AddWithValue("@logTypeID", apilog.LogTypeID);
                        cmd.Parameters.AddWithValue("@logType", apilog.LogType);
                        cmd.Parameters.AddWithValue("@SupplierID", apilog.SupplierID);
                        cmd.Parameters.AddWithValue("@logMsg", "");
                        cmd.Parameters.AddWithValue("@logrequestXML", apilog.logrequestXML);
                        cmd.Parameters.AddWithValue("@logresponseXML", apilog.logresponseXML);
                        cmd.Parameters.AddWithValue("@logStatus", apilog.logStatus);
                        cmd.Parameters.AddWithValue("@StartTime", apilog.StartTime);
                        cmd.Parameters.AddWithValue("@EndTime", apilog.EndTime);
                        cmd.Parameters.AddWithValue("@HotelId", apilog.HotelId);
                        cmd.Parameters.Add("@retVal", SqlDbType.Int);
                        cmd.Parameters["@retVal"].Direction = ParameterDirection.Output;
                        if (cmd.Connection.State == System.Data.ConnectionState.Closed)
                        {
                            cmd.Connection.Open();
                        }
                        cmd.ExecuteNonQuery();
                    }

                }
            }
            catch (FormatException ex)
            {
                //throw ex;
            }
        }
        public void SaveAPILogs_room_iol(APILogDetail apilog)
        {
            try
            {
                using (conn = new SqlConnection(ConfigurationManager.ConnectionStrings["INGMContext"].ToString()))
                {
                    using (cmd = new SqlCommand("USP_InsertAPILog_room_iol", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@customerID", apilog.customerID);
                        cmd.Parameters.AddWithValue("@TrackNumber", apilog.TrackNumber);
                        cmd.Parameters.AddWithValue("@logTypeID", apilog.LogTypeID);
                        cmd.Parameters.AddWithValue("@logType", apilog.LogType);
                        cmd.Parameters.AddWithValue("@SupplierID", apilog.SupplierID);
                        cmd.Parameters.AddWithValue("@logMsg", "");
                        cmd.Parameters.AddWithValue("@logrequestXML", apilog.logrequestXML);
                        cmd.Parameters.AddWithValue("@logresponseXML", apilog.logresponseXML);
                        cmd.Parameters.AddWithValue("@logStatus", apilog.logStatus);
                        cmd.Parameters.AddWithValue("@StartTime", apilog.StartTime);
                        cmd.Parameters.AddWithValue("@EndTime", apilog.EndTime);
                        cmd.Parameters.AddWithValue("@preID", apilog.preID);
                        cmd.Parameters.Add("@retVal", SqlDbType.Int);
                        cmd.Parameters["@retVal"].Direction = ParameterDirection.Output;
                        if (cmd.Connection.State == System.Data.ConnectionState.Closed)
                        {
                            cmd.Connection.Open();
                        }
                        cmd.ExecuteNonQuery();
                    }

                }
            }
            catch (FormatException ex)
            {
                //throw ex;
            }
        }
        public void SaveAPILogsflt(APILogDetail apilog)
        {
            try
            {
                using (conn = new SqlConnection(ConfigurationManager.ConnectionStrings["INGMContext"].ToString()))
                {
                    using (cmd = new SqlCommand("SP_InsertAPILogflt", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@preID", apilog.preID);
                        cmd.Parameters.AddWithValue("@customerID", apilog.customerID);
                        cmd.Parameters.AddWithValue("@TrackNumber", apilog.TrackNumber);
                        cmd.Parameters.AddWithValue("@logTypeID", apilog.LogTypeID);
                        cmd.Parameters.AddWithValue("@logType", apilog.LogType);
                        cmd.Parameters.AddWithValue("@SupplierID", apilog.SupplierID);
                        cmd.Parameters.AddWithValue("@logMsg", "");
                        cmd.Parameters.AddWithValue("@logrequestXML", apilog.logrequestXML);
                        cmd.Parameters.AddWithValue("@logresponseXML", apilog.logresponseXML);
                        cmd.Parameters.AddWithValue("@logStatus", apilog.logStatus);
                        cmd.Parameters.AddWithValue("@StartTime", apilog.StartTime);
                        cmd.Parameters.AddWithValue("@EndTime", apilog.EndTime);
                        cmd.Parameters.Add("@retVal", SqlDbType.Int);
                        cmd.Parameters["@retVal"].Direction = ParameterDirection.Output;
                        if (cmd.Connection.State == System.Data.ConnectionState.Closed)
                        {
                            cmd.Connection.Open();
                        }
                        cmd.ExecuteNonQuery();
                    }

                }
            }
            catch (FormatException ex)
            {
                //throw ex;
            }
        }
        public void SaveAPILogwithResponseError(APILogDetail apilog)
        {
            try
            {
                using (conn = new SqlConnection(ConfigurationManager.ConnectionStrings["INGMContext"].ToString()))
                {
                    using (cmd = new SqlCommand("SP_InsertApilogFail", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@customerID", apilog.customerID);
                        cmd.Parameters.AddWithValue("@TrackNumber", apilog.TrackNumber);
                        cmd.Parameters.AddWithValue("@logTypeID", apilog.LogTypeID);
                        cmd.Parameters.AddWithValue("@logType", apilog.LogType);
                        cmd.Parameters.AddWithValue("@SupplierID", apilog.SupplierID);
                        cmd.Parameters.AddWithValue("@logMsg", apilog.logMsg);
                        cmd.Parameters.AddWithValue("@logrequestXML", apilog.logrequestXML);
                        cmd.Parameters.AddWithValue("@logresponseXML", apilog.logresponseXML);
                        cmd.Parameters.AddWithValue("@logStatus", apilog.logStatus);
                        cmd.Parameters.AddWithValue("@StartTime", apilog.StartTime);
                        cmd.Parameters.AddWithValue("@EndTime", apilog.EndTime);
                        cmd.Parameters.Add("@retVal", SqlDbType.Int);
                        cmd.Parameters["@retVal"].Direction = ParameterDirection.Output;
                        if (cmd.Connection.State == System.Data.ConnectionState.Closed)
                        {
                            cmd.Connection.Open();
                        }
                        cmd.ExecuteNonQuery();
                    }

                }
                //connecttion();
                //string ipaddress = string.Empty;
                //try
                //{
                //    //ipaddress = toGetIpAddress();
                //}
                //catch { }
                //SqlCommand com = new SqlCommand("[SP_InsertApilogFail]", con);
                //com.CommandType = CommandType.StoredProcedure;
                //com.Parameters.AddWithValue("@customerID", apilog.customerID);
                //com.Parameters.AddWithValue("@TrackNumber", apilog.TrackNumber);
                //com.Parameters.AddWithValue("@logTypeID", apilog.LogTypeID);
                //com.Parameters.AddWithValue("@logType", apilog.LogType);
                //com.Parameters.AddWithValue("@SupplierID", apilog.SupplierID);
                //com.Parameters.AddWithValue("@logMsg", apilog.logMsg);
                //com.Parameters.AddWithValue("@logrequestXML", apilog.logrequestXML);
                //com.Parameters.AddWithValue("@logresponseXML", apilog.logresponseXML);
                //com.Parameters.AddWithValue("@logStatus", apilog.logStatus);
                //com.Parameters.AddWithValue("@StartTime", apilog.StartTime);
                //com.Parameters.AddWithValue("@EndTime", apilog.EndTime);
                //com.Parameters.Add("@retVal", SqlDbType.Int);
                //com.Parameters["@retVal"].Direction = ParameterDirection.Output;
                ////connecttion();
                //if (com.Connection.State == ConnectionState.Closed)
                //{ com.Connection.Open(); }
                //com.ExecuteNonQuery();
            }
            catch (FormatException ex)
            {
                //throw ex;
            }
            finally
            {
                //com.Dispose();
                //con.Close();
                //con.Dispose();
            }
        }
        public void SendExcepToDB(Exception ex)
        {
            try
            {
                using (conn = new SqlConnection(ConfigurationManager.ConnectionStrings["INGMContext"].ToString()))
                {
                    using (cmd = new SqlCommand("SP_ExceptionLogging", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ExceptionMsg", ex.Message.ToString());
                        cmd.Parameters.AddWithValue("@ExceptionType", ex.GetType().Name.ToString());
                        cmd.Parameters.AddWithValue("@customerID", "");
                        cmd.Parameters.AddWithValue("@ExceptionSource", ex.StackTrace.ToString());
                        if (cmd.Connection.State == System.Data.ConnectionState.Closed)
                        {
                            cmd.Connection.Open();
                        }
                        cmd.ExecuteNonQuery();
                    }

                }
                //connecttion();
                //SqlCommand com = new SqlCommand("SP_ExceptionLogging", con);

                //com.CommandType = CommandType.StoredProcedure;
                //com.Parameters.AddWithValue("@ExceptionMsg", ex.Message.ToString());
                //com.Parameters.AddWithValue("@ExceptionType", ex.GetType().Name.ToString());
                //com.Parameters.AddWithValue("@customerID", "");
                //com.Parameters.AddWithValue("@ExceptionSource", ex.StackTrace.ToString());
                //connecttion();
                //com.ExecuteNonQuery();
            }
            catch (FormatException ex2)
            {
                //throw ex2;
            }
            finally
            {
                //com.Dispose();
                //con.Close();   
                //con.Dispose();
            }
        }
        public void SendCustomExcepToDB(CustomException ex)
        {
            try
            {
                using (conn = new SqlConnection(ConfigurationManager.ConnectionStrings["INGMContext"].ToString()))
                {
                    using (cmd = new SqlCommand("SP_CustomExceptionLogging", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ExceptionMsg", ex.MsgName);
                        cmd.Parameters.AddWithValue("@ExceptionType", ex.ExcType);
                        cmd.Parameters.AddWithValue("@PageName", ex.PageName);
                        cmd.Parameters.AddWithValue("@MethodName", ex.MethodName);
                        cmd.Parameters.AddWithValue("@customerID", ex.CustomerID);
                        cmd.Parameters.AddWithValue("@TransID", ex.TranID);
                        cmd.Parameters.AddWithValue("@ExceptionSource", ex.ExcSource);
                        if (cmd.Connection.State == System.Data.ConnectionState.Closed)
                        {
                            cmd.Connection.Open();
                        }
                        cmd.ExecuteNonQuery();
                    }

                }

                //connecttion();
                //SqlCommand com = new SqlCommand("SP_CustomExceptionLogging", con);

                //com.CommandType = CommandType.StoredProcedure;
                //com.Parameters.AddWithValue("@ExceptionMsg", ex.MsgName);
                //com.Parameters.AddWithValue("@ExceptionType", ex.ExcType);
                //com.Parameters.AddWithValue("@PageName", ex.PageName);
                //com.Parameters.AddWithValue("@MethodName", ex.MethodName);
                //com.Parameters.AddWithValue("@customerID", ex.CustomerID);
                //com.Parameters.AddWithValue("@TransID", ex.TranID);
                //com.Parameters.AddWithValue("@ExceptionSource", ex.ExcSource);
                //if (com.Connection.State == ConnectionState.Closed)
                //{ com.Connection.Open(); }
                //com.ExecuteNonQuery();
            }
            catch (FormatException ex2)
            {
                //throw ex2;
            }
        }
        private string GetIPAddress()
        {
            try
            {
                //if (System.ServiceModel.OperationContext.Current != null)
                //{
                //    var endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                //    return endpoint.Address;
                //}


                if (System.Web.HttpContext.Current != null)
                {
                    // Check proxied IP address
                    if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
                        return HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] + " via " +
                            HttpContext.Current.Request.UserHostAddress;
                    else
                        return HttpContext.Current.Request.UserHostAddress;

                }
            }
            catch { }
            return "Unknown";
        }

        public string toGetIpAddress()  // Get IP Address
        {
            string ip = "";
            try
            {
                IPHostEntry ipEntry = Dns.GetHostEntry(toGetCompCode());
                IPAddress[] addr = ipEntry.AddressList;
                ip = addr[1].ToString();
                return ip;
            }
            catch { }
            return "Unknown";
        }
        public string toGetCompCode()  // Get Computer Name
        {
            string strHostName = "";
            try
            {
                strHostName = Dns.GetHostName();
                return strHostName;
            }
            catch
            {

            }
            return "Unknown pc";
        }




        #region Transfer





        public void SaveAPILogs_Transfer(APILogDetail apilog)
        {
            try
            {
                using (conn = new SqlConnection(ConfigurationManager.ConnectionStrings["INGMContext"].ToString()))
                {
                    using (cmd = new SqlCommand("SP_InsertAPILog_Transfer", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@customerID", apilog.customerID);
                        cmd.Parameters.AddWithValue("@TrackNumber", apilog.TrackNumber);
                        cmd.Parameters.AddWithValue("@logTypeID", apilog.LogTypeID);
                        cmd.Parameters.AddWithValue("@logType", apilog.LogType);
                        cmd.Parameters.AddWithValue("@SupplierID", apilog.SupplierID);
                        cmd.Parameters.AddWithValue("@logMsg", "");
                        cmd.Parameters.AddWithValue("@logrequestXML", apilog.logrequestXML);
                        cmd.Parameters.AddWithValue("@logresponseXML", apilog.logresponseXML);
                        cmd.Parameters.AddWithValue("@logStatus", apilog.logStatus);
                        cmd.Parameters.AddWithValue("@StartTime", apilog.StartTime);
                        cmd.Parameters.AddWithValue("@EndTime", apilog.EndTime);
                        cmd.Parameters.Add("@retVal", SqlDbType.Int);
                        cmd.Parameters["@retVal"].Direction = ParameterDirection.Output;
                        if (cmd.Connection.State == System.Data.ConnectionState.Closed)
                        {
                            cmd.Connection.Open();
                        }
                        cmd.ExecuteNonQuery();
                    }

                }
            }
            catch (FormatException ex)
            {
                //throw ex;
            }
        }

        public async Task<string> GetLogResponseAsync(RequestModel model)
        {
            try
            {
                model.IsResult = false;
                using (conn = new SqlConnection(ConfigurationManager.ConnectionStrings["INGMContext"].ToString()))
                {
                    using (cmd = new SqlCommand("APIProc", conn))
                    {

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@flag", 6);
                        cmd.Parameters.AddWithValue("@TransID", model.TrackNo);
                        cmd.Parameters.AddWithValue("@customerID", model.CustomerId);
                        cmd.Parameters.AddWithValue("@SuplId", model.SupplierId);
                        cmd.Parameters.AddWithValue("@logTypeID", model.CallType);
                        cmd.Parameters.AddWithValue("@IpAddress", model.IpAddress);

                        if (cmd.Connection.State == System.Data.ConnectionState.Closed)
                        {
                            cmd.Connection.Open();
                        }


                        var result = await cmd.ExecuteScalarAsync();
                        if (result != null)
                        {
                            model.IsResult = true;
                            model.ResponseStr = result.ToString();
                        }


                    }
                    conn.Close();
                    return model.ResponseStr;
                }
            }
            catch (Exception ex)
            {
                throw ex;

            }

        }


        #endregion






        public long SaveRoomAPILogs(APILogDetail apilog)
        {
            long _LogId = -1;
            try
            {
                using (conn = new SqlConnection(ConfigurationManager.ConnectionStrings["INGMContext"].ToString()))
                {
                    using (cmd = new SqlCommand("AddRoomLogProc", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@customerID", apilog.customerID);
                        cmd.Parameters.AddWithValue("@TrackNumber", apilog.TrackNumber);
                        cmd.Parameters.AddWithValue("@logTypeID", apilog.LogTypeID);
                        cmd.Parameters.AddWithValue("@logType", apilog.LogType);
                        cmd.Parameters.AddWithValue("@SupplierID", apilog.SupplierID);
                        cmd.Parameters.AddWithValue("@logMsg", "");
                        cmd.Parameters.AddWithValue("@logrequestXML", apilog.logrequestXML);
                        cmd.Parameters.AddWithValue("@logresponseXML", apilog.logresponseXML);
                        cmd.Parameters.AddWithValue("@logStatus", apilog.logStatus);
                        cmd.Parameters.AddWithValue("@StartTime", apilog.StartTime);
                        cmd.Parameters.AddWithValue("@EndTime", apilog.EndTime);
                        cmd.Parameters.AddWithValue("@HotelId", apilog.HotelId);
                        cmd.Parameters.Add("@LogId", SqlDbType.BigInt);
                        cmd.Parameters["@LogId"].Direction = ParameterDirection.Output;
                        if (cmd.Connection.State == System.Data.ConnectionState.Closed)
                        {
                            cmd.Connection.Open();
                        }
                        cmd.ExecuteNonQuery();
                        _LogId = (long)cmd.Parameters["@LogId"].Value;
                        cmd.Connection.Close();
                    }
                }
                return _LogId;
            }
            catch (FormatException ex)
            {
                //throw ex;
            }
            return _LogId;
        }















        #region Dispose
        /// <summary>
        /// Dispose all used resources.
        /// </summary> 




        private bool _isDisposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!_isDisposed)
                {
                    //Do your unmanaged disposing here
                    _isDisposed = true;
                }
            }
        }











        #endregion
    }
}