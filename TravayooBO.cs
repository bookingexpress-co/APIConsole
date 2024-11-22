using APIConsole.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
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

        string constr = ConfigurationManager.ConnectionStrings["INGMContext"].ConnectionString;
        public async Task<List<APILogModel>> GetSearchLogAsync(string trackNo, int supplier)
        {
           var lstLog = new List<APILogModel>();
            string sql = @"select * from tblSimulationResult where resultid in (1,2)";
            using (var conn = new SqlConnection(constr))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = sql;
                //cmd.Parameters.AddWithValue("@id", supplier);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (reader.Read())
                    {
                        var obj = new APILogModel();
                        obj.Supplier = supplier;
                        obj.TrackNo = trackNo;
                        obj.Request = reader.GetString(reader.GetOrdinal("ResultXML"));
                        lstLog.Add(obj);
                    }
                }
            }
            return lstLog.OrderBy(x=>x.LogId).ToList();
        }










    }























}
