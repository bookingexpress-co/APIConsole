using APIConsole.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIConsole
{

    public class APILogModel
    {
        public int Supplier { get; set; }
        public string TrackNo { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
        public string CallType { get; set; }
        public int LogId { get; set; }
    }







    public class TravayooBO
    {
        string BasePath = CommonHelper.BasePath() + @"\App_Data\HotelAPI\";
        string constr = ConfigurationManager.ConnectionStrings["INGMContext"].ConnectionString;
        public async Task<List<APILogModel>> GetSearchLogAsync(string trackNo, int supplier)
        {
            try
            {


                var lstLog = new List<APILogModel>();
                string sql = @"Select * from tblapilog_search x where x.SupplierID=@SupplierId and x.TrackNumber=@TrackNumber";
                using (var conn = new SqlConnection(constr))
                using (var cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandText = sql;
                    cmd.Parameters.AddWithValue("@SupplierId", supplier);
                    cmd.Parameters.AddWithValue("@TrackNumber", trackNo);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            var obj = new APILogModel();
                            obj.Supplier = supplier;
                            obj.TrackNo = trackNo;
                            obj.LogId = (int)reader.GetInt64(reader.GetOrdinal("logID"));
                            obj.Request = reader.GetString(reader.GetOrdinal("logrequestXML"));
                            obj.Response = reader.GetString(reader.GetOrdinal("logresponseXML"));
                            lstLog.Add(obj);
                        }
                    }
                }
                return lstLog.OrderBy(x => x.LogId).ToList();


            }
            catch (Exception ex)
            {
                throw ex;
            }



        }





        public async Task<List<APILogModel>> GetLogAsync(string trackNo, int supplier, string sqlQuery)
        {
            try
            {

                var lstLog = new List<APILogModel>();
                using (var conn = new SqlConnection(constr))
                using (var cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandText = sqlQuery;
                    cmd.Parameters.AddWithValue("@SupplierId", supplier);
                    cmd.Parameters.AddWithValue("@TrackNumber", trackNo);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            var obj = new APILogModel();
                            obj.Supplier = supplier;
                            obj.TrackNo = trackNo;
                            obj.LogId = (int)reader.GetInt64(reader.GetOrdinal("logID"));
                            obj.Request = reader.GetString(reader.GetOrdinal("logrequestXML"));
                            obj.Response = reader.GetString(reader.GetOrdinal("logresponseXML"));
                            lstLog.Add(obj);
                        }
                    }
                }
                return lstLog.OrderBy(x => x.LogId).ToList();


            }
            catch (Exception ex)
            {
                throw ex;
            }



        }
        public void SaveFile(List<APILogModel> lstLog, string fileName)
        {
            int index = 0;
            foreach (var item in lstLog)
            {

                if (!string.IsNullOrEmpty(item.Request))
                {
                    string filePath = BasePath + fileName + string.Format("Request-{0}.json", index);
                    File.WriteAllText(filePath, item.Request);
                }
                if (!string.IsNullOrEmpty(item.Response))
                {
                    string filePath = BasePath + fileName + string.Format("Response-{0}.json", index);
                    File.WriteAllText(filePath, item.Response);
                }
                ++index;
            }
        }


        public void APILog(string trackNo, int suplId)
        {
            var folder = new DirectoryInfo(BasePath);
            foreach (FileInfo file in folder.GetFiles())
            {
                file.Delete();
            }

            string htlSql = @"Select * from tblapilog_search x where x.SupplierID=@SupplierId and x.TrackNumber=@TrackNumber";
            var htlData = this.GetLogAsync(trackNo, suplId, htlSql);
            this.SaveFile(htlData.Result, "HotelSearch");

            string rmSql = @"Select * from tblapilog_room x where x.SupplierID=@SupplierId and x.TrackNumber=@TrackNumber";
            var rmData = this.GetLogAsync(trackNo, suplId, rmSql);
            this.SaveFile(rmData.Result, "RoomSearch");

            string prSql = @"Select * from tblapilog x where x.SupplierID=@SupplierId and x.logTypeID = 4 and x.TrackNumber=@TrackNumber";

            var prData = this.GetLogAsync(trackNo, suplId, prSql);
            this.SaveFile(prData.Result, "PreBook");

            string sql = @"Select * from tblapilog x where x.SupplierID=@SupplierId and x.logTypeID = 5 and x.TrackNumber=@TrackNumber";
            var data = this.GetLogAsync(trackNo, suplId, sql);
            this.SaveFile(data.Result, "Booking");

        }














      
    }























}
