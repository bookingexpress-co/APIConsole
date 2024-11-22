using System;
using System.Collections.Generic;

using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace APIConsole.Helpers
{
    public static class CommonHelper
    {
 


        public static string BasePath()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var basePath = currentDirectory.Split(new string[] { "\\bin" }, StringSplitOptions.None)[0];
            return basePath;
        }







        public static string IsNullString(this string str)
        {
            string result = string.Empty;
            if (str != null)
            {
                result = str;
            }
            return result;
        }
        public static decimal IsNullNumber(this decimal? item)
        {
            return item.HasValue ? item.Value : 0.00m; ;
        }





        public static decimal GetValueOrDefault(this XAttribute attribute, decimal defaultValue = 0.0m)
        {
            if (attribute == null)
                return defaultValue;
            else
                return Convert.ToDecimal(attribute.Value);
        }

















    }












}