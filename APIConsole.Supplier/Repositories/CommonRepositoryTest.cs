﻿using APIConsole.Supplier.Models.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace APIConsole.Supplier.Repositories
{
    public class CommonRepositoryTest
    {
        string _path;
        public CommonRepositoryTest(string _path)
        {
            this._path = _path;
        }

        #region Get static hotel data from giata mapping
        public List<StaticHotelData> GetStaticHotelsForSearch(int SupplierId, string HotelCode, string HotelName, string CityId, string CountryId, string MinRating, string MaxRating)
        {
            List<StaticHotelData> hotelList = new List<StaticHotelData>();

            try
            {


         
                string xmlPath = Path.Combine(this._path, "GiataStaticData.xml");

                DataSet ds = new DataSet();
                ds.ReadXml(xmlPath);
                DataTable hoteldt = ds.Tables[0];
        
                if (hoteldt.Rows.Count > 0)
                {
                    hotelList = hoteldt.AsEnumerable().Select(row => new StaticHotelData
                    {
                        HotelCode = row["hotelid"].ToString(),
                        HotelName = row["hotelname"].ToString(),
                        Rating = row["rating"].ToString(),
                        Latitude = row["latitude"].ToString(),
                        Longitude = row["longitude"].ToString(),
                        Address = row["address"].ToString(),
                        CityName = row["cityname"].ToString(),
                        GiataId = row["giataid"].ToString(),
                        SupplierID = SupplierId.ToString(),
                        HotelImage = getHotelImage(row["giataid"].ToString())
                    }).ToList();
                }


            }
            catch (Exception ex)
            {
                hotelList = null;

            }
            return hotelList;

        }
        #endregion
        #region Get static hotel image
        private string getHotelImage(string GiataId)
        {
            return "https://images.bookingexpress.co/HotelPrimary/" + GiataId + "/PrimaryImage.webp";
        }
        #endregion


        #region bind supplier hotel response data
        public XElement BindHotelData(StaticHotelData hotelData, XElement req, string RequestID, decimal minrate, string dmc, string xmlouttype, string customerid, string sales_environment)
        {
            XElement htlData = null;
            try
            {
                htlData = new XElement("Hotel",
                        new XElement("HotelID", hotelData.HotelCode),
                        new XElement("HotelName", hotelData.HotelName),
                        new XElement("PropertyTypeName", ""),
                        new XElement("CountryID", req.Descendants("CountryID").FirstOrDefault().Value),
                        new XElement("CountryName", req.Descendants("CountryName").FirstOrDefault().Value),
                        new XElement("CountryCode", req.Descendants("CountryCode").FirstOrDefault().Value),
                        new XElement("CityId", req.Descendants("CityID").FirstOrDefault().Value),
                        new XElement("CityCode", req.Descendants("CityCode").FirstOrDefault().Value),
                        new XElement("CityName", req.Descendants("CityName").FirstOrDefault().Value),
                        new XElement("AreaId"),
                        new XElement("AreaName", hotelData.Area),
                        new XElement("RequestID", RequestID),
                        new XElement("Address", hotelData.Address),
                        new XElement("Location", hotelData.Location),
                        new XElement("Description"),
                        new XElement("StarRating", hotelData.Rating),
                        new XElement("MinRate", minrate),
                        new XElement("HotelImgSmall", hotelData.HotelImage),
                        new XElement("HotelImgLarge", hotelData.HotelImage),
                        new XElement("MapLink"),
                        new XElement("Longitude", hotelData.Longitude),
                        new XElement("Latitude", hotelData.Latitude),
                        new XElement("xmloutcustid", customerid),
                        new XElement("xmlouttype", xmlouttype),
                        new XElement("DMC", dmc), new XElement("SupplierID", hotelData.SupplierID),
                        new XElement("Currency", hotelData.Currency),
                        new XElement("Offers", ""), new XElement("Facilities", null),
                        new XElement("Rooms", ""),
                        new XElement("searchType", sales_environment),
                        new XElement("GiataID", hotelData.GiataId));

            }
            catch { }
            return htlData;
        }
        public XElement BindHotelData_WithUnderScore(StaticHotelData hotelData, XElement req, string RequestID, decimal minrate, string dmc, string xmlouttype, string customerid, string sales_environment)
        {
            XElement htlData = null;
            try
            {
                htlData = new XElement("Hotel",
                        new XElement("HotelID", hotelData.HotelCode.Replace('-', '_')),
                        new XElement("HotelName", hotelData.HotelName),
                        new XElement("PropertyTypeName", ""),
                        new XElement("CountryID", req.Descendants("CountryID").FirstOrDefault().Value),
                        new XElement("CountryName", req.Descendants("CountryName").FirstOrDefault().Value),
                        new XElement("CountryCode", req.Descendants("CountryCode").FirstOrDefault().Value),
                        new XElement("CityId", req.Descendants("CityID").FirstOrDefault().Value),
                        new XElement("CityCode", req.Descendants("CityCode").FirstOrDefault().Value),
                        new XElement("CityName", req.Descendants("CityName").FirstOrDefault().Value),
                        new XElement("AreaId"),
                        new XElement("AreaName", hotelData.Area),
                        new XElement("RequestID", RequestID),
                        new XElement("Address", hotelData.Address),
                        new XElement("Location", hotelData.Location),
                        new XElement("Description"),
                        new XElement("StarRating", hotelData.Rating),
                        new XElement("MinRate", minrate),
                        new XElement("HotelImgSmall", hotelData.HotelImage),
                        new XElement("HotelImgLarge", hotelData.HotelImage),
                        new XElement("MapLink"),
                        new XElement("Longitude", hotelData.Longitude),
                        new XElement("Latitude", hotelData.Latitude),
                        new XElement("xmloutcustid", customerid),
                        new XElement("xmlouttype", xmlouttype),
                        new XElement("DMC", dmc), new XElement("SupplierID", hotelData.SupplierID),
                        new XElement("Currency", hotelData.Currency),
                        new XElement("Offers", ""), new XElement("Facilities", null),
                        new XElement("Rooms", ""),
                        new XElement("searchType", sales_environment),
                        new XElement("GiataID", hotelData.GiataId));

            }
            catch { }
            return htlData;
        }

        #endregion


        #region split hotel list into chunks
        public List<List<T>> SplitPropertyList<T>(List<T> htlList, int size)
        {
            var list = new List<List<T>>();
            for (int i = 0; i < htlList.Count; i += size)
                list.Add(htlList.GetRange(i, Math.Min(size, htlList.Count - i)));
            return list;
        }
        #endregion

    }
}