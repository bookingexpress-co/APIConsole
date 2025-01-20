using APIConsole.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

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
        string BasePath = CommonHelper.BasePath() + @"\" + ConfigurationManager.AppSettings["ClientsFilePath"];
        string constr = ConfigurationManager.ConnectionStrings["INGMContext.Live"].ConnectionString;
        public string CreateIfMissing(string path)
        {
            string logPath = BasePath + path;
            bool folderExists = Directory.Exists(logPath);
            if (!folderExists)
                Directory.CreateDirectory(logPath);
            return logPath;
        }

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
        public void SaveFile(List<APILogModel> lstLog, string logpath, string _type)
        {

            int index = 0;
            foreach (var item in lstLog)
            {
                if (!string.IsNullOrEmpty(item.Request))
                {
                    string filePath = Path.Combine(logpath, string.Format("_Req_{0}.{1}", index, _type));
                    File.WriteAllText(filePath, item.Request);
                }
                if (!string.IsNullOrEmpty(item.Response))
                {
                    var rspString = item.Response.GetJsonFromXml();
                    string filePath = Path.Combine(logpath, string.Format("_Resp_{0}.{1}", index, _type));
                    File.WriteAllText(filePath, item.Response);
                }
                ++index;
            }
        }

        public void Search(string trackNo, string path, int suplId, string _type)
        {
            var logPath = Path.Combine(path, "search_sup_" + suplId);

            string htlSql = @"Select * from tblapilog_search x where x.SupplierID=@SupplierId and x.TrackNumber=@TrackNumber";
            var htlData = this.GetLogAsync(trackNo, suplId, htlSql);
            this.SaveFile(htlData.Result, logPath, _type);
        }
        public void Room(string trackNo, string path, int suplId, string _type)
        {
            var logPath = Path.Combine(path, "room");
            string rmSql = @"Select * from tblapilog_room x where x.SupplierID=@SupplierId and x.TrackNumber=@TrackNumber";
            var rmData = this.GetLogAsync(trackNo, suplId, rmSql);
            this.SaveFile(rmData.Result, logPath, _type);

        }
        public void CxlPolicy(string trackNo, string path, int suplId, string _type)
        {
            var logPath = Path.Combine(path, "cxlPolicy");
            var folder = new DirectoryInfo(logPath);
            foreach (FileInfo file in folder.GetFiles())
            {
                file.Delete();
            }
            string prSql = @"Select * from tblapilog x where x.SupplierID=@SupplierId and x.logTypeID = 3 and x.TrackNumber=@TrackNumber";
            var prData = this.GetLogAsync(trackNo, suplId, prSql);
            this.SaveFile(prData.Result, logPath, _type);

        }

        public void PreBook(string trackNo, string path, int suplId, string _type)
        {
            var logPath = Path.Combine(path, "prebook");
            var folder = new DirectoryInfo(logPath);
            foreach (FileInfo file in folder.GetFiles())
            {
                file.Delete();
            }


            string prSql = @"Select * from tblapilog x where x.SupplierID=@SupplierId and x.logTypeID = 4 and x.TrackNumber=@TrackNumber";
            var prData = this.GetLogAsync(trackNo, suplId, prSql);
            this.SaveFile(prData.Result, logPath, _type);


        }

        public void Book(string trackNo, string path, int suplId, string _type)
        {
            var logPath = Path.Combine(path, "book");

            string sql = @"Select * from tblapilog x where x.SupplierID=@SupplierId and x.logTypeID = 5 and x.TrackNumber=@TrackNumber";
            var data = this.GetLogAsync(trackNo, suplId, sql);
            this.SaveFile(data.Result, logPath, _type);
        }


        public void APILog(string trackNo, string path, int suplId, string logPath, string _type)
        {
            var folder = new DirectoryInfo(logPath);
            foreach (FileInfo file in folder.GetFiles())
            {
                file.Delete();
            }

            string htlSql = @"Select * from tblapilog_search x where x.SupplierID=@SupplierId and x.TrackNumber=@TrackNumber";
            var htlData = this.GetLogAsync(trackNo, suplId, htlSql);
            this.SaveFile(htlData.Result, logPath + "HotelSearch", _type);

            string rmSql = @"Select * from tblapilog_room x where x.SupplierID=@SupplierId and x.TrackNumber=@TrackNumber";
            var rmData = this.GetLogAsync(trackNo, suplId, rmSql);
            this.SaveFile(rmData.Result, logPath + "RoomSearch", _type);

            string prSql = @"Select * from tblapilog x where x.SupplierID=@SupplierId and x.logTypeID = 4 and x.TrackNumber=@TrackNumber";
            var prData = this.GetLogAsync(trackNo, suplId, prSql);
            this.SaveFile(prData.Result, logPath + "PreBook", _type);

            string sql = @"Select * from tblapilog x where x.SupplierID=@SupplierId and x.logTypeID = 5 and x.TrackNumber=@TrackNumber";
            var data = this.GetLogAsync(trackNo, suplId, sql);
            this.SaveFile(data.Result, logPath + "Booking", _type);

        }

        public void SupplierSearchResponse(string trackNo, int suplId, string logpath, string _type)
        {
            string sqlQuery = @"Select * from tblapilog_search x where x.SupplierID=@SupplierId and x.TrackNumber=@TrackNumber";
            var resultData = this.GetLogAsync(trackNo, suplId, sqlQuery).Result;
            string filePath = Path.Combine(logpath, string.Format("Response-{0}.{1}", DateTime.Now.Ticks, _type));

            int countHotel = 0;
            JArray hotelList = new JArray();
            foreach (var item in resultData)
            {
                if (!string.IsNullOrEmpty(item.Response))
                {
                    var rspString = item.Response.GetJsonFromXml();

                    if (!string.IsNullOrEmpty(rspString))
                    {
                        dynamic data = JsonConvert.DeserializeObject(rspString);
                        if (data.Status.Code == 200 && data.Status.Description == "Successful")
                        {
                            var hotels = (JArray)data.HotelResult;
                            hotelList.Merge(hotels);
                            Console.WriteLine("Chunk Hotel Count = {0}", hotels.Count);
                            countHotel += hotels.Count;

                        }
                    }


                }
            }
            Console.WriteLine("Total Chunk Hotel Count = {0}", countHotel);
            File.WriteAllText(filePath, hotelList.ToString());
        }






        public void duplicateResponse(string trackNo, int suplId, string logpath, string _type)
        {
            string sqlQuery = @"Select * from tblapilog_search x where x.SupplierID=@SupplierId and x.TrackNumber=@TrackNumber";
            var resultData = this.GetLogAsync(trackNo, suplId, sqlQuery).Result;
            string filePath = Path.Combine(logpath, string.Format("Response-{0}.{1}", DateTime.Now.Ticks, _type));

            int countHotel = 0;
            JArray hotelList = new JArray();
            foreach (var item in resultData)
            {
                if (!string.IsNullOrEmpty(item.Response))
                {
                    var rspString = item.Response.GetJsonFromXml();

                    if (!string.IsNullOrEmpty(rspString))
                    {
                        dynamic data = JsonConvert.DeserializeObject(rspString);
                        if (data.Status.Code == 200 && data.Status.Description == "Successful")
                        {
                            var hotels = (JArray)data.HotelResult;
                            hotelList.Merge(hotels);
                            Console.WriteLine("Chunk Hotel Count = {0}", hotels.Count);
                            countHotel += hotels.Count;

                        }
                    }


                }
            }


            Console.WriteLine("Total Chunk Hotel Distinct = {0}", hotelList.Distinct().Count());
            Console.WriteLine("Total Chunk Hotel Count = {0}", countHotel);
            File.WriteAllText(filePath, hotelList.ToString());
        }
        public void RhineResponse(string trackNo, int suplId, string logpath, string _type)
        {
            string sqlQuery = @"select top  1  *  from  tblapilog_search x where x.SupplierID=0 and x.logTypeID=1 and x.TrackNumber=@TrackNumber order by 1 desc";
            var resultData = this.GetLogAsync(trackNo, suplId, sqlQuery).Result;
            string filePath = Path.Combine(logpath, string.Format("Response-{0}.{1}", DateTime.Now.Ticks, _type));
            if (resultData.Count > 0)
            {


                int countHotel = 0;
                var item = resultData[0];
                if (!string.IsNullOrEmpty(item.Response))
                {
                    var xmlData = XElement.Parse(item.Response);

                    var counttt = xmlData.Descendants("GiataHotelList").Where(x => x.Attribute("GSupID").Value == suplId.ToString());
                    var hotels = xmlData.Descendants("Hotel").Where(x => x.Element("SupplierID").Value == suplId.ToString());
                    countHotel = counttt.Count();
                    XElement element = new XElement("Hotels");
                    element.Add(hotels);
                    element.Save(filePath);
                    Console.WriteLine("Total Hotel Giata Count = {0}", hotels.Count());
                }

                Console.WriteLine("Total Hotel Giata Count = {0}", countHotel);
            }
        }

    }
}
