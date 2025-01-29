using APIConsole.Supplier.Helpers;
using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml.Linq;

namespace APIConsole.Supplier.Models
{
    public static class supplier_Cred
    {
         static readonly XElement supcred;  
         static supplier_Cred()
        {
            try
            {
       
                string BasePath = TravayooHelper.BasePath() + @"\" + ConfigurationManager.AppSettings["Credentials"];

                supcred = XElement.Load(BasePath);
            }
            catch { }
        }
         public static XElement getsupplier_credentials(this string customerid,string supplierid)
         {
             XElement suppcred = null;
             try
             {                 
                 if (!string.IsNullOrEmpty(customerid))
                 {
                     suppcred = supcred.Descendants("credential").Where(x => x.Attribute("customerid").Value == customerid && x.Attribute("supplierid").Value == supplierid).FirstOrDefault();
                 }
                 return suppcred;
             }
             catch
             {
                 return null;
             }
        }
        //public static int cutoff_time()
        // {
        //    try
        //    {
        //        return Convert.ToInt32(ConfigurationManager.AppSettings["cutofftime"].ToString());
        //    }
        //    catch { return 90000; }
        // }
        public static int rmcutoff_time()
        {
            try
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["rmcutofftime"].ToString());
            }
            catch { return 90000; }
        }
        //public static int secondcutoff_time()
        //{
        //    try
        //    {
        //        return Convert.ToInt32(ConfigurationManager.AppSettings["secondcutime"].ToString());
        //    }
        //    catch { return 90000; }
        //}
        public static int secondcutoff_time(string HotelID)
        {
            try
            {
                Int32 cutoftime;
                bool hotelwiseSearch = false;
                hotelwiseSearch = !string.IsNullOrEmpty(HotelID) ? true : false;
                if (hotelwiseSearch == true)
                {
                    cutoftime = Convert.ToInt32(ConfigurationManager.AppSettings["secondhotelcutime"].ToString());

                }
                else
                {
                    cutoftime = Convert.ToInt32(ConfigurationManager.AppSettings["secondcutime"].ToString());

                }

                return cutoftime;

            }
            catch { return 90000; }
        }



        public static int cutoff_time(string HotelID)
        {
            try
            {

                Int32 cutoftime;
                bool hotelwiseSearch = false;
                hotelwiseSearch = !string.IsNullOrEmpty(HotelID) ? true : false;


                if (hotelwiseSearch == true)
                {
                    cutoftime = Convert.ToInt32(ConfigurationManager.AppSettings["Hotelcutofftime"].ToString());

                }
                else
                {
                    cutoftime = Convert.ToInt32(ConfigurationManager.AppSettings["cutofftime"].ToString());

                }

                return cutoftime;

            }
            catch { return 90000; }
        }
    }
}