using Newtonsoft.Json;
using System.Xml;

namespace APIConsole.Helpers
{
    public static class XmlHelper
    {

        public static string responseXml(this string respnse)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement element = doc.CreateElement("ResponseXml");
            element.InnerText = respnse.JsonEscape();
            return element.OuterXml;

        }
        public static string GetJsonFromXml(this string xmlString)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlString);
            string resp = doc.InnerText;
           // JsonConvert.DeserializeObject<string>(doc.InnerText);

            return resp;
        }

    }
}
