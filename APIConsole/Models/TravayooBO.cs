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
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace APIConsole.Models
{
    public class APILogModel
    {
        public long LogId { get; set; }
        public int Supplier { get; set; }
        public string Customer { get; set; }
        public string CallType { get; set; }
        public string TrackNo { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int ResponseTime { get; set; }
        public string FilePath { get; set; }

    }

    public class ItemType
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
    }

    public class TravayooBO
    {
        string BasePath;
        string constr;
        public TravayooBO(string connectionString, string basePath)
        {
            constr = connectionString;
            BasePath = basePath;
        }
        public string CreateIfMissing(string path)
        {
            string logPath = BasePath + path;
            bool folderExists = Directory.Exists(logPath);
            if (!folderExists)
                Directory.CreateDirectory(logPath);
            return logPath;
        }

        public void CleanDirectory(string path)
        {
            var folder = new DirectoryInfo(path);
            foreach (FileInfo file in folder.GetFiles())
            {
                file.Delete();
            }

        }
        public async Task<List<ItemType>> SupplierList()
        {
            var resultList = new List<ItemType>();
            try
            {
                string sqlQuery = @"Select x.suplId,y.suplLclName,x.suplCurrency,x.mrkupTypid,x.mrkupvalue,x.cxlmrkupTyp,x.canclmrkup 
From tblSupplierMaster x inner join tblSupplierMaster_Locale y on x.suplId=y.suplId 
Where x.srvId=1 and x.suplStatus=1 and x.supCatId=1";

                using (var conn = new SqlConnection(constr))
                using (var cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandText = sqlQuery;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            var item = new ItemType
                            {
                                ItemId = reader.GetInt32(reader.GetOrdinal("suplId")),
                                ItemName = reader.GetString(reader.GetOrdinal("suplLclName"))
                            };
                            resultList.Add(item);
                        }
                    }
                }
                return resultList.OrderBy(x => x.ItemId).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<List<APILogModel>> RhineSystemLog(string trackNo, string supplierId, int? logTypeId, string filePath)
        {
            var resultList = new List<APILogModel>();
            try
            {
                string sqlQuery = @"
declare @SupplierId int=null, @logTypeId int=null
declare @TrackNo nvarchar(1000)=null;
Select  x.logID,x.TrackNumber,x.SupplierID,x.logTypeID,x.customerID,x.logrequestXML,x.logresponseXML,x.logcreatedOn,x.logmodifyOn ,DATEDIFF(MILLISECOND, x.logmodifyOn, x.logcreatedOn) as responseTime
from tblapilog_search x WITH (NOLOCK) where x.logTypeID=1 and x.SupplierID=@SupplierId and x.TrackNumber=@TrackNo and 
x.logTypeID=(CASE WHEN  (@logTypeId IS NOT NULL)  THEN @logTypeId ELSE x.logTypeID END)
union
Select  x.logID,x.TrackNumber,x.SupplierID,x.logTypeID,x.customerID,x.logrequestXML,
CONVERT(nvarchar(max),x.logresponseXML) as logresponseXML,x.logcreatedOn,x.logmodifyOn ,DATEDIFF(MILLISECOND, x.logmodifyOn, x.logcreatedOn) as responseTime
from tblapilog_room x WITH (NOLOCK) where x.logTypeID=2 and x.SupplierID=@SupplierId and x.TrackNumber=@TrackNo
and x.logTypeID=(CASE WHEN  (@logTypeId IS NOT NULL)  THEN @logTypeId ELSE x.logTypeID END)
union
Select  x.logID,x.TrackNumber,x.SupplierID,x.logTypeID,x.customerID,x.logrequestXML,
CONVERT(nvarchar(max),x.logresponseXML) as logresponseXML,
x.logcreatedOn,x.logmodifyOn,DATEDIFF(MILLISECOND, x.logmodifyOn, x.logcreatedOn) as responseTime
from tblapilog  x WITH (NOLOCK) where  x.SupplierID=@SupplierId and x.TrackNumber=@TrackNo
and x.logTypeID=(CASE WHEN  (@logTypeId IS NOT NULL)  THEN @logTypeId ELSE x.logTypeID END)
";

                using (var conn = new SqlConnection(constr))
                using (var cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandText = sqlQuery;
                    cmd.Parameters.AddWithValue("@SupplierId", supplierId);
                    cmd.Parameters.AddWithValue("@TrackNo", trackNo);
                    cmd.Parameters.AddWithValue("@logTypeId", logTypeId);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            var item = new APILogModel
                            {
                                LogId = reader.GetInt64(reader.GetOrdinal("logID")),
                                TrackNo = reader.GetString(reader.GetOrdinal("TrackNumber")),
                                Request = reader.GetString(reader.GetOrdinal("logrequestXML")),
                                Response = reader.GetString(reader.GetOrdinal("logresponseXML")),
                                CallType = reader.GetString(reader.GetOrdinal("logTypeID")),
                                Customer = reader.GetString(reader.GetOrdinal("customerID")),
                                CreatedOn = reader.GetDateTime(reader.GetOrdinal("logcreatedOn")),
                                ModifiedOn = reader.GetDateTime(reader.GetOrdinal("logmodifyOn")),
                                ResponseTime = reader.GetInt32(reader.GetOrdinal("responseTime")),
                                FilePath = filePath
                            };
                            item.FilePath = this.SaveFile(item);
                            resultList.Add(item);

                        }
                    }
                }
                return resultList.OrderBy(x => new { x.LogId, x.CallType }).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public string SaveFile(APILogModel item)
        {
            var sb = new StringBuilder();
            sb.AppendLine("                                                                              ");
            sb.AppendLine("                                                                              ");
            sb.AppendLine("------------------------------------------------------------------------------");
            sb.AppendLine("------------------------------------------------------------------------------");
            sb.AppendLine("---------------Log Detail-----------------------------------------------------");
            sb.AppendLine("Track No :     " + item.TrackNo + "    ---------------------------------------");
            sb.AppendLine("Log Id :       " + item.LogId + "    -----------------------------------------");
            sb.AppendLine("Customer :     " + item.Customer + "    --------------------------------------");
            sb.AppendLine("Log Type :     " + item.CallType + "    --------------------------------------");
            sb.AppendLine("Supplier :     " + item.Supplier + "    --------------------------------------");
            sb.AppendLine("------------------------------------------------------------------------------");
            sb.AppendLine("------------------------------------------------------------------------------");
            sb.AppendLine("                                                                              ");
            sb.AppendLine("                                                                              ");
            if (!string.IsNullOrEmpty(item.Request))
            {
                sb.AppendLine("------------------------------------------------------------------------------");
                sb.AppendLine("------------------------------------------------------------------------------");
                sb.AppendLine("--------------------------Request---------------------------------------------");
                sb.AppendLine("------------------------------------------------------------------------------");
                sb.AppendLine("------------------------------------------------------------------------------");
                sb.AppendLine("                                                                              ");
                sb.AppendLine("                                                                              ");
                sb.AppendLine(item.Request);
                sb.AppendLine("                                                                              ");
                sb.AppendLine("                                                                              ");
                sb.AppendLine("                                                                              ");
                sb.AppendLine("                                                                              ");
            }
            if (!string.IsNullOrEmpty(item.Response))
            {
                if (item.Supplier == 21)
                {
                    item.Response = item.Response.GetJsonFromXml().cleanFormJSON();
                }
                sb.AppendLine("------------------------------------------------------------------------------");
                sb.AppendLine("------------------------------------------------------------------------------");
                sb.AppendLine("--------------------------Response--------------------------------------------");
                sb.AppendLine("------------------------------------------------------------------------------");
                sb.AppendLine("------------------------------------------------------------------------------");
                sb.AppendLine("                                                                              ");
                sb.AppendLine("                                                                              ");
                sb.AppendLine(item.Response);
                sb.AppendLine("                                                                              ");
                sb.AppendLine("                                                                              ");
                sb.AppendLine("                                                                              ");
                sb.AppendLine("                                                                              ");
            }
            string fileName = string.Format("LogFile{0}-{1}_.{2}", item.LogId, item.CallType, "text");
            string filePath = Path.Combine(item.FilePath, fileName);
            File.WriteAllText(filePath, sb.ToString());
            return filePath;
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
                    string filePath = string.Format("{0}_Req_{1}.{2}", logpath, index, _type);
                    File.WriteAllText(filePath, item.Request);

                }
                if (!string.IsNullOrEmpty(item.Response))
                {
                    string filePath = string.Format("{0}_Resp_{1}.{2}", logpath, index, _type);
                    if (item.Supplier == 21)
                    {
                        item.Response = item.Response.GetJsonFromXml().cleanFormJSON();
                    }
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


        public void APILog(string trackNo, string logPath, int suplId, string _type)
        {
            var lstResult = new List<APILogModel>();
            string path = string.Empty;

            string htlSql = @"Select * from tblapilog_search x where x.logTypeID = 1 and x.SupplierID=@SupplierId and x.TrackNumber=@TrackNumber";
            var htlData = this.GetLogAsync(trackNo, suplId, htlSql);
            path = Path.Combine(logPath, "search_sup_" + suplId);
            lstResult.AddRange(htlData.Result);
            this.SaveFile(htlData.Result, path, _type);

            string rmSql = @"Select * from tblapilog_room x where x.SupplierID=@SupplierId and x.TrackNumber=@TrackNumber";
            var rmData = this.GetLogAsync(trackNo, suplId, rmSql);
            path = Path.Combine(logPath, "room_sup_" + suplId);
            lstResult.AddRange(rmData.Result);
            this.SaveFile(rmData.Result, path, _type);

            string prSql = @"Select * from tblapilog x where x.SupplierID=@SupplierId and x.logTypeID = 4 and x.TrackNumber=@TrackNumber";
            var prData = this.GetLogAsync(trackNo, suplId, prSql);
            path = Path.Combine(logPath, "preBook_sup_" + suplId);
            lstResult.AddRange(prData.Result);
            this.SaveFile(prData.Result, path, _type);

            string sql = @"Select * from tblapilog x where x.SupplierID=@SupplierId and x.logTypeID = 5 and x.TrackNumber=@TrackNumber";
            var data = this.GetLogAsync(trackNo, suplId, sql);
            path = Path.Combine(logPath, "book_sup_" + suplId);
            lstResult.AddRange(data.Result);
            this.SaveFile(data.Result, path, _type);

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
