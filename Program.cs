using APIConsole.Helpers;
using System;
using System.IO;

namespace APIConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var basePath = CommonHelper.BasePath() + @"\App_Data\HB\";
            var objBL = new TravayooBO();
            var data = objBL.GetSearchLogAsync("Test", 21);
            int index = 0;
            foreach (var item in data.Result)
            {

                if (!string.IsNullOrEmpty(item.Request))
                {
                    File.WriteAllText(basePath + string.Format("HBSRESP-{0}.json", DateTime.Now.Ticks), item.Request);
                }

                ++index;
            }

        }
    }
}
