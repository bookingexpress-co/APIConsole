using APIConsole.Helpers;
using System;
using System.IO;

namespace APIConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string trackNo = "a2b4ac4a-6274-4ef2-aab1-6c36a2d5f76d";
            int suplId = 21;
            var basePath = CommonHelper.BasePath() + @"\App_Data\B2B\HotelSearch\";
            var objBL = new TravayooBO();
            var data = objBL.GetSearchLogAsync(trackNo, suplId);
            int index = 0;
            foreach (var item in data.Result)
            {

                if (!string.IsNullOrEmpty(item.Request))
                {
                    File.WriteAllText(basePath + string.Format("HotelSearchReq-{0}.json", index), item.Request);
                }
                if (!string.IsNullOrEmpty(item.Response))
                {
                    File.WriteAllText(basePath + string.Format("HotelSearchResp-{0}.json", index), item.Response);
                }
                ++index;
            }

        }







    }
}
