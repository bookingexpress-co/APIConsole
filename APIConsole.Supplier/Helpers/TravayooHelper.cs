using APIConsole.Supplier.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.Linq;


namespace APIConsole.Supplier.Helpers
{
    public static class TravayooHelper
    {
        public static string GetValueOrDefault(this XElement item, string defaultValue = null)
        {
            if (item == null)
                return defaultValue;
            else
                return item.Value;
        }

        public static int GetValueOrDefault(this XElement item, int defaultValue = 0)
        {
            if (item == null)
                return defaultValue;
            else
                return Convert.ToInt32(item.Value);
        }

        public static double GetValueOrDefault(this XElement item, double defaultValue = 0.0d)
        {
            if (item == null)
                return defaultValue;
            else
                return Convert.ToDouble(item.Value);
        }
        public static bool GetValueOrDefault(this XAttribute item, bool defaultValue = false)
        {
            if (item == null)
                return defaultValue;
            else
                return Convert.ToBoolean(item.Value);
        }
        public static long GetValueOrDefault(this XAttribute item, long defaultValue = 0)
        {
            if (item == null)
                return defaultValue;
            else
                return Convert.ToInt64(item.Value);
        }
        public static int GetValueOrDefault(this XAttribute item, int defaultValue = 0)
        {
            if (item == null)
                return defaultValue;
            else
                return Convert.ToInt32(item.Value);
        }

        public static int ToINT(this XAttribute item, int defaultValue = 0)
        {
            if (item == null)
                return defaultValue;
            else
                return Convert.ToInt32(item.Value);
        }

        public static long ToLong(this string item)
        {
            if (item != null)
            {
                return Convert.ToInt64(item);
            }
            else
            {
                return 0;
            }
        }

        public static string GetValueOrDefault(this XAttribute attribute, string defaultValue = null)
        {
            if (attribute == null)
                return defaultValue;
            else
                return attribute.Value;
        }

        public static decimal GetValueOrDefault(this XAttribute attribute, decimal defaultValue = 0.0m)
        {
            if (attribute == null)
                return defaultValue;
            else
                return Convert.ToDecimal(attribute.Value);
        }
        public static double GetValueOrDefault(this XAttribute attribute, double defaultValue = 0.0d)
        {
            if (attribute == null)
                return defaultValue;
            else
                return Convert.ToDouble(attribute.Value);
        }
        public static IEnumerable<XElement> DescendantsOrEmpty(this XElement item, XName name)
        {
            IEnumerable<XElement> result;
            if (item != null)
                result = item.Descendants(name).Count() != 0 ? item.Descendants(name) : Enumerable.Empty<XElement>();
            else
                result = Enumerable.Empty<XElement>();
            return result;
        }
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            if (source != null)
            {
                foreach (T obj in source)
                {
                    return false;
                }
            }
            return true;
        }




        public static DateTime GetCheckinDate(this XAttribute item, string dateTimeFormate, string defaultValue = null)
        {
            string date = string.Empty;
            if (item != null)
            {
                date = item.Value;
            }
            else
            {
                date = defaultValue;
            }
            DateTime dte = DateTime.ParseExact(date.Trim(), dateTimeFormate, CultureInfo.InvariantCulture);
            return dte;
        }



        public static string AlterFormat(this string strDate, string oldFormat, string newFormat)
        {
            DateTime dte = DateTime.ParseExact(strDate.Trim(), oldFormat, CultureInfo.InvariantCulture);
            return dte.ToString(newFormat);
        }

        public static string TotalStayEncode(this string Data)
        {
            Data = HttpUtility.UrlEncode(Data);
            Data = "data=" + Data;
            return Data;
        }


        public static XElement RemoveXmlns(this XElement doc)
        {
            doc.Descendants().Attributes().Where(x => x.IsNamespaceDeclaration).Remove();
            foreach (var elem in doc.Descendants())
                elem.Name = elem.Name.LocalName;

            return doc;
        }


        public static int Days(this string startDate, string endDate, string dateFormat)
        {
            DateTime start = DateTime.ParseExact(startDate.Trim(), dateFormat, CultureInfo.InvariantCulture);
            DateTime end = DateTime.ParseExact(endDate.Trim(), dateFormat, CultureInfo.InvariantCulture);
            TimeSpan difference = end - start;
            int daysBetween = difference.Days;
            return daysBetween;
        }


        public static List<List<T>> SplitHotelLists<T>(this List<T> htlList, int size)
        {
            var list = new List<List<T>>();
            for (int i = 0; i < htlList.Count; i += size)
                list.Add(htlList.GetRange(i, Math.Min(size, htlList.Count - i)));
            return list;
        }

        public static ApiAction GetActions(this string ElementName)
        {
            if (ElementName == "searchRequest")
            {
                return ApiAction.Search;
            }
            else if (ElementName == "HotelPreBookingRequest")
            {
                return ApiAction.PreBook;
            }
            else if (ElementName == "HotelBookingRequest")
            {
                return ApiAction.Book;
            }
            else if (ElementName == "hotelcancelpolicyrequest")
            {
                return ApiAction.CXLPolicy;
            }
            else if (ElementName == "HotelCancellationRequest")
            {
                return ApiAction.Cancel;
            }


            else
            {
                return ApiAction.Cancel;
            }

        }

        public static DateTime TravayooDateTime(this string strDate)
        {
            DateTime dte = DateTime.ParseExact(strDate.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
            return dte;
        }

        public static DateTime GetDateTime(this string strDateTime, string dateTimeFormate)
        {
            DateTime dte = DateTime.ParseExact(strDateTime.Trim(), dateTimeFormate, CultureInfo.InvariantCulture);
            return dte;
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


        public static XElement RemoveAllNamespaces(this XElement xmlDocument)
        {
            XElement xmlDocumentWithoutNs = removeAllNamespaces(xmlDocument);
            return xmlDocumentWithoutNs;
        }
        private static XElement removeAllNamespaces(XElement xmlDocument)
        {
            var stripped = new XElement(xmlDocument.Name.LocalName);
            foreach (var attribute in
                    xmlDocument.Attributes().Where(
                    attribute =>
                        !attribute.IsNamespaceDeclaration &&
                        String.IsNullOrEmpty(attribute.Name.NamespaceName)))
            {
                stripped.Add(new XAttribute(attribute.Name.LocalName, attribute.Value));
            }
            if (!xmlDocument.HasElements)
            {
                stripped.Value = xmlDocument.Value;
                return stripped;
            }
            stripped.Add(xmlDocument.Elements().Select(
                el =>
                    RemoveAllNamespaces(el)));
            return stripped;
        }



        public static string JsonEscape(this string input)
        {
            char[] toEscape = "\0\x1\x2\x3\x4\x5\x6\a\b\t\n\v\f\r\xe\xf\x10\x11\x12\x13\x14\x15\x16\x17\x18\x19\x1a\x1b\x1c\x1d\x1e\x1f\x2C\"\\".ToCharArray();
            string[] literals = @"\0,\x0001,\x0002,\x0003,\x0004,\x0005,\x0006,\a,\b,\t,\n,\v,\f,\r,\x000e,\x000f,\x0010,\x0011,\x0012,\x0013,\x0014,\x0015,\x0016,\x0017,\x0018,\x0019,\x001a,\x001b,\x001c,\x001d,\x001e,\x001f\x002C".Split(new char[] { ',' });

            int i = input.IndexOfAny(toEscape);
            if (i < 0) return input;

            var sb = new System.Text.StringBuilder(input.Length + 5);
            int j = 0;
            do
            {
                sb.Append(input, j, i - j);
                var c = input[i];
                if (c < 0x20) sb.Append(literals[c]); else sb.Append(@"\").Append(c);
            } while ((i = input.IndexOfAny(toEscape, j = ++i)) > 0);

            return sb.Append(input, j, input.Length - j).ToString();
        }

        public static string Escape(this string input)
        {
            StringBuilder literal = new StringBuilder(input.Length + 2);
            literal.Append("\"");
            foreach (var c in input)
            {
                switch (c)
                {
                    case '\"': literal.Append("\\\""); break;
                    case '\\': literal.Append(@"\\"); break;
                    case '\0': literal.Append(@"\0"); break;
                    case '\a': literal.Append(@"\a"); break;
                    case '\b': literal.Append(@"\b"); break;
                    case '\f': literal.Append(@"\f"); break;
                    case '\n': literal.Append(@"\n"); break;
                    case '\r': literal.Append(@"\r"); break;
                    case '\t': literal.Append(@"\t"); break;
                    case '\v': literal.Append(@"\v"); break;
                    default:
                        // ASCII printable character
                        if (c >= 0x20 && c <= 0x7e)
                        {
                            literal.Append(c);
                            // As UTF16 escaped character
                        }
                        else
                        {
                            literal.Append(@"\u");
                            literal.Append(((int)c).ToString("x4"));
                        }
                        break;
                }
            }
            literal.Append("\"");
            return literal.ToString();
        }

        public static string BasePath()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var basePath = currentDirectory.Split(new string[] { "\\bin" }, StringSplitOptions.None)[0];
            return basePath;
        }

        public static string ActualPath(string Key)
        {
            string _filePath = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory);
            var keyPath = ConfigurationManager.AppSettings[Key].ToString();
            var basePath = Path.Combine(_filePath, keyPath);
            return basePath;
        }

        public static int ChildrenCount(this string strArray)
        {
            int count = 0;
            if (!string.IsNullOrEmpty(strArray))
            {
                count = strArray.Split(',').Count();
            }
            return count;
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
    (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (!seenKeys.Contains(keySelector(element)))
                {
                    seenKeys.Add(keySelector(element));
                    yield return element;
                }
            }
        }

        public static IEnumerable<IGrouping<int, TSource>> GroupBy<TSource>(this IEnumerable<TSource> source, int itemsPerGroup)
        {
            return source.Zip(Enumerable.Range(0, source.Count()),
                              (s, r) => new { Group = r / itemsPerGroup, Item = s })
                         .GroupBy(i => i.Group, g => g.Item)
                         .ToList();
        }

        public static IEnumerable<IEnumerable<T>> Batch<T>(IEnumerable<T> collection, int batchSize)
        {
            List<T> nextbatch = new List<T>(batchSize);
            foreach (T item in collection)
            {
                nextbatch.Add(item);
                if (nextbatch.Count == batchSize)
                {
                    yield return nextbatch;
                    nextbatch = new List<T>(batchSize);
                }
            }
            if (nextbatch.Count > 0)
                yield return nextbatch;
        }




        public static List<T>[] Partition<T>(List<T> list, int totalPartitions)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            if (totalPartitions < 1)
                throw new ArgumentOutOfRangeException("totalPartitions");

            List<T>[] partitions = new List<T>[totalPartitions];

            int maxSize = (int)Math.Ceiling(list.Count / (double)totalPartitions);
            int k = 0;

            for (int i = 0; i < partitions.Length; i++)
            {
                partitions[i] = new List<T>();
                for (int j = k; j < k + maxSize; j++)
                {
                    if (j >= list.Count)
                        break;
                    partitions[i].Add(list[j]);
                }
                k += maxSize;
            }

            return partitions;
        }
        public static List<List<T>> BreakIntoSlots<T>(List<T> list, int slotSize)
        {
            if (slotSize <= 0)
            {
                throw new ArgumentException("Slot Size must be greater than 0.");
            }
            List<List<T>> retVal = new List<List<T>>();
            while (list.Count > 0)
            {
                int count = list.Count > slotSize ? slotSize : list.Count;
                retVal.Add(list.GetRange(0, count));
                list.RemoveRange(0, count);
            }

            return retVal;
        }


        public static IEnumerable<IEnumerable<T>> GroupAt<T>(this IEnumerable<T> source, int itemsPerGroup)
        {
            for (int i = 0; i < (int)Math.Ceiling((double)source.Count() / itemsPerGroup); i++)
                yield return source.Skip(itemsPerGroup * i).Take(itemsPerGroup);
        }
        public static IEnumerable<IGrouping<int, T>> GroupAtt<T>(this IEnumerable<T> source, int itemsPerGroup)
        {
            for (int i = 0; i < (int)Math.Ceiling((double)source.Count() / itemsPerGroup); i++)
            {
                var currentGroup = new Grouping<int, T> { Key = i };
                currentGroup.AddRange(source.Skip(itemsPerGroup * i).Take(itemsPerGroup));
                yield return currentGroup;
            }
        }
        private class Grouping<TKey, TElement> : List<TElement>, IGrouping<TKey, TElement>
        {
            public TKey Key { get; set; }
        }



        public static IEnumerable<KeyValuePair<int, T[]>> Partition<T>(this IEnumerable<T> source, int partitionSize)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (partitionSize < 1) throw new ArgumentOutOfRangeException("partitionSize");

            int i = 0;
            List<T> partition = new List<T>(partitionSize);

            foreach (T item in source)
            {
                partition.Add(item);
                if (partition.Count == partitionSize)
                {
                    yield return new KeyValuePair<int, T[]>(++i, partition.ToArray());
                    partition.Clear();
                }
            }

        }



        public static List<List<T>> Combinations<T>(this List<T> list, int combinationSize, Func<List<T>, bool> condition)
        {
            List<List<T>> result = new List<List<T>>();
            GetCombinations(list, combinationSize, 0, new List<T>(), result, condition);
            return result;
        }

        private static void GetCombinations<T>(List<T> list, int combinationSize, int start, List<T> current, List<List<T>> result, Func<List<T>, bool> condition)
        {
            if (current.Count == combinationSize)
            {
                if (condition(current))
                {
                    result.Add(new List<T>(current));
                }
                return;
            }
            for (int i = start; i < list.Count; i++)
            {
                current.Add(list[i]);
                GetCombinations(list, combinationSize, i + 1, current, result, condition);
                current.RemoveAt(current.Count - 1);
            }
        }



        public static string GetJsonFromXml(this string xmlString)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlString);
            string resp = doc.InnerText;
            // JsonConvert.DeserializeObject<string>(doc.InnerText);
            return resp;
        }


        public static string cleanFormJSON(this string str)
        {
            if (str == null || str.Length == 0)
            {
                return "";
            }
            str = Regex.Replace(str, @"\\u(?<Value>[a-zA-Z0-9]{4})",
         m =>
         {
             return ((char)int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString();
         });

            str = str.Replace("\\n", "\n");
            str = str.Replace("\\b", "\b");
            str = str.Replace("\\f", "\f");
            str = str.Replace("\\t", "\t");
            str = str.Replace("\\r", "\r");
            str = str.Replace("\\", "/");
            return str.ToString();

        }



    
    }
}