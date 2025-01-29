using APIConsole.Supplier.DataAccess;
using APIConsole.Supplier.Helpers;
using APIConsole.Supplier.Models;
using APIConsole.Supplier.Models.Common;
using APIConsole.Supplier.Models.HYGST;
using APIConsole.Supplier.Repositories;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Xml.Linq;


namespace APIConsole.Supplier.Services
{
    public class HYGSTServices : IDisposable
    {

        #region Global vars
        HYGSTCredentials model;
        HYGSTRepository repo;
        SaveAPILog _saveEx;
        CustomException exLog;
        string customerId = string.Empty;
        string trackNo = string.Empty;
        string dmc = "HYPERGUEST";
        const int supplierId = 28;
        int chunksize = 20;
        int sup_cutime = 20, threadCount = 2;
        XNamespace soapenv = "http://schemas.xmlsoap.org/soap/envelope/";
        string sales_environment = "hotel_package";
        #endregion

        public HYGSTServices(string customerId, ApiAction callType)
        {
            this.customerId = customerId;
            model = customerId.ReadCredential(supplierId, (short)callType);
            repo = new HYGSTRepository(model);
            _saveEx = new SaveAPILog();
        }

        #region Common

        HotelSearchModel BindSearchModel(XElement req, ApiAction callType)
        {
            var _hreq = new HotelSearchModel()
            {
                SupplierId = supplierId,
                CustomerId = customerId.ToLong(),
                CityCode = req.Element("CityID").Value,
                CountryCode = req.Element("CountryID").Value,
                HotelId = req.Element("HotelID").Value,
                HotelName = req.Element("HotelName").Value,
                MinRating = req.Element("MinStarRating").GetValueOrDefault(0),
                MaxRating = req.Element("MaxStarRating").GetValueOrDefault(0),
                CheckIn = req.Element("FromDate").Value.AlterFormat("dd/MM/yyyy", "yyyy-MM-dd"),
                CheckOut = req.Element("ToDate").Value.AlterFormat("dd/MM/yyyy", "yyyy-MM-dd"),
                TrackNo = req.Element("TransID").Value,
                Nights = req.Element("FromDate").Value.Days(req.Element("ToDate").Value, "dd/MM/yyyy"),
                Nationality = req.Element("PaxNationality_CountryCode").Value.ToLower(),
                RoomList = req.Element("Rooms").Descendants("RoomPax").Select((x, i) => new RoomPax
                {
                    Index = i + 1,
                    Guests = new Occupancy
                    {
                        Adults = x.Element("Adult").GetValueOrDefault(1),
                        Children = x.Descendants("ChildAge") != null ? x.Descendants("ChildAge").Select(y => y.GetValueOrDefault(0)).ToList() : new List<int>(),
                    }
                }).ToList(),
            };

            if (callType == ApiAction.Search)
            {
                var hItem = req.Descendants("SupplierID").Where(x => x.Attribute("custID").Value == customerId && x.Value == supplierId.ToString()).FirstOrDefault();
                _hreq.XmlOut = hItem.Attribute("xmlout").GetValueOrDefault(false);
            }
            else if (callType == ApiAction.RoomSearch)
            {
                var hItem = req.Descendants("GiataHotelList").Where(x => x.Attribute("custID").Value == customerId && x.Attribute("GSupID").Value == supplierId.ToString()).FirstOrDefault();
                _hreq.XmlOut = hItem.Attribute("xmlout").GetValueOrDefault(false);
                _hreq.HotelId = hItem.Attribute("GHtlID").GetValueOrDefault("");
            }
            return _hreq;
        }

        public string GetGuestString(List<RoomPax> roomList)
        {
            var _guests = new List<string>();
            foreach (var itm in roomList)
            {
                string gStr = string.Empty;
                if (itm.Guests.Children.Count > 0)
                {
                    string childs = string.Join(",", itm.Guests.Children);
                    gStr = itm.Guests.Adults + "-" + childs;
                }
                else
                {
                    gStr = itm.Guests.Adults.ToString();
                }
                _guests.Add(gStr);
            }
            return string.Join(".", _guests);
        }

        public XElement GetPriceBreakup(List<double> priceList)
        {
            XElement pricebrk = new XElement("PriceBreakups");

            int i = 1;
            foreach (var price in priceList)
            {
                pricebrk.Add(new XElement("Price", new XAttribute("Night", i), new XAttribute("PriceValue", price)));
                i++;
            }
            return pricebrk;
        }

        public List<RoomPolicy> RoomCxlTag(JToken _policy, JToken _dayRates, string checkIn, double roomPrice)
        {
            List<RoomPolicy> cxlList = new List<RoomPolicy>();
            try
            {
                List<double> nightPrice = new List<double>();
                foreach (JToken item in _dayRates)
                {
                    double nightAmt = (double)item.SelectToken("prices.sell.price");

                    nightPrice.Add(nightAmt);
                }
                if (_policy != null)
                {
                    foreach (JToken item in _policy)
                    {
                        var _days = (int)item["daysBefore"];

                        string _time = (string)item["cancellationDeadlineHour"];

                        if (string.IsNullOrEmpty(_time))
                        {
                            _time = "00:00";
                        }
                        string _dateTime = checkIn.Trim() + " " + _time.Trim();
                        var cxlDate = _dateTime.GetDateTime("yyyy-MM-dd HH:mm").AddDays(-_days);
                        double amount = 0;
                        double _amount = (double)item["amount"];
                        string penaltyType = (string)item["penaltyType"];
                        if (penaltyType == "fixed")
                        {
                            amount = _amount;
                        }
                        else if (penaltyType == "percent")
                        {
                            amount = nightPrice.Sum() * _amount / 100;
                        }
                        else if (penaltyType == "nights")
                        {
                            amount = nightPrice.Select((v, i) => new { v, i }).
                                Where(x => x.i < (int)_amount).Sum(x => x.v);
                        }
                        cxlList.Add(new RoomPolicy
                        {
                            StartDate = cxlDate,
                            Amount = amount,
                            NoShow = 0
                        });
                    }
                }
                else
                {
                    cxlList.Add(new RoomPolicy
                    {
                        StartDate = checkIn.GetDateTime("yyyy-MM-dd HH:mm").AddDays(-1),
                        Amount = nightPrice.Sum(),
                        NoShow = 0
                    });

                }

            }
            catch
            {
                cxlList.Add(new RoomPolicy
                {
                    StartDate = checkIn.GetDateTime("yyyy-MM-dd HH:mm").AddDays(-1),
                    Amount = roomPrice,
                    NoShow = 0
                });

            }
            return cxlList;
        }



        public RhinePolicyModel roomTypePolicyTag(List<List<RoomPolicy>> _policy, double _totalPrice, int bufferDay)
        {
            var model = new RhinePolicyModel();
            var _cxlPolicy = new XElement("CancellationPolicies");

            try
            {

                var cxlList = _policy.SelectMany(x => x).ToList();
                var _tempPolicy = cxlList.GroupBy(z => new { z.StartDate, z.NoShow }).
                   Select(y => new RhineCancelPolicy
                   {
                       StartDate = y.Key.StartDate,
                       Amount = y.Sum(x => x.Amount),
                       NoShow = y.Key.NoShow,
                   }).OrderBy(x => x.StartDate).ToList();


                var fItem = _tempPolicy.FirstOrDefault();
                var lastItem = new RhineCancelPolicy
                {
                    StartDate = fItem.StartDate.AddDays(-1),
                    Amount = 0.0d,
                    NoShow = 0,
                };
                _tempPolicy.Insert(0, lastItem);
                if (fItem.StartDate.AddDays(-bufferDay).Date > DateTime.Now.Date)
                {
                    var _cxlPolicyList = _tempPolicy.Select(y => new XElement("CancellationPolicy",
                          new XAttribute("LastCancellationDate", y.StartDate.ToString("yyyy-MM-dd")),
                          new XAttribute("ApplicableAmount", y.Amount),
                          new XAttribute("NoShowPolicy", y.NoShow))).OrderBy(p => p.Attribute("LastCancellationDate").Value).ToList();
                    _cxlPolicy.Add(_cxlPolicyList);
                    model = new RhinePolicyModel { Amount = 0, LastCancelDate = lastItem.StartDate, IsRefundable = true, policy = _cxlPolicy };

                }
                else
                {
                    _cxlPolicy.Add(new XElement("CancellationPolicy",
                           new XAttribute("LastCancellationDate", lastItem.StartDate.AddDays(-1).ToString("yyyy-MM-dd")),
                           new XAttribute("ApplicableAmount", 0),
                           new XAttribute("NoShowPolicy", 0)));

                    _cxlPolicy.Add(new XElement("CancellationPolicy",
                                         new XAttribute("LastCancellationDate", lastItem.StartDate.ToString("yyyy-MM-dd")),
                                         new XAttribute("ApplicableAmount", _totalPrice),
                                         new XAttribute("NoShowPolicy", 0)));
                    _cxlPolicy.Add(new XElement("CancellationPolicy",
                                         new XAttribute("LastCancellationDate", lastItem.StartDate.ToString("yyyy-MM-dd")),
                                         new XAttribute("ApplicableAmount", _totalPrice),
                                         new XAttribute("NoShowPolicy", 1)));


                    model = new RhinePolicyModel { Amount = _totalPrice, LastCancelDate = lastItem.StartDate, IsRefundable = false, policy = _cxlPolicy };

                }


            }
            catch
            {
                _cxlPolicy.Add(new XElement("CancellationPolicy",
                                      new XAttribute("LastCancellationDate", DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd")),
                                      new XAttribute("ApplicableAmount", 0),
                                      new XAttribute("NoShowPolicy", 0)));
                _cxlPolicy.Add(new XElement("CancellationPolicy",
                                     new XAttribute("LastCancellationDate", DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd")),
                                     new XAttribute("ApplicableAmount", _totalPrice),
                                     new XAttribute("NoShowPolicy", 0)));
                _cxlPolicy.Add(new XElement("CancellationPolicy",
                                     new XAttribute("LastCancellationDate", DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd")),
                                     new XAttribute("ApplicableAmount", _totalPrice),
                                     new XAttribute("NoShowPolicy", 1)));



                model = new RhinePolicyModel { Amount = _totalPrice, LastCancelDate = DateTime.Now, IsRefundable = false, policy = _cxlPolicy };
            }
            return model;
        }
        public void WriteException(CustomException _exLog)
        {
            _exLog.PageName = "HYGSTServices";
            _exLog.CustomerID = this.customerId;
            _exLog.TranID = this.trackNo;
            _saveEx.SendCustomExcepToDB(_exLog);
        }












        #endregion

        #region HotelSearch
        public List<XElement> HotelAvailability(XElement req)
        {
            var rhReq = req.Descendants("searchRequest").First();
            List<XElement> HotelsList = new List<XElement>();
            var _hreq = this.BindSearchModel(rhReq, ApiAction.Search);
            try
            {
                #region get cut off time
                try
                {
                    sup_cutime = supplier_Cred.secondcutoff_time(req.Descendants("HotelID").FirstOrDefault().Value);
                }
                catch { }
                int timeOut = sup_cutime;


                System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
                timer.Start();

                #endregion

                var filePath = HttpRuntime.AppDomainAppPath + @"\App_Data\Hotel";
                var commRepo = new CommonRepositoryTest(filePath);

                //var commRepo = new CommonRepository();
                var hotelData = commRepo.GetStaticHotelsForSearch(supplierId, _hreq.HotelId, _hreq.HotelName, _hreq.CityCode, _hreq.CountryCode, _hreq.MinRating.ToString(), _hreq.MaxRating.ToString());

                if (hotelData == null || hotelData.Count == 0)
                {
                    #region No hotel found exception
                    exLog = new CustomException("There is no hotel available in database");
                    exLog.MethodName = "HotelAvailability";
                    exLog.PageName = "RTHWKServices";
                    exLog.CustomerID = _hreq.CustomerId.ToString();
                    exLog.TranID = req.Descendants("TransID").First().Value;
                    SaveAPILog saveex = new SaveAPILog();
                    saveex.SendCustomExcepToDB(exLog);
                    #endregion
                    return null;
                }
                var statiProperties = hotelData.Select(r => r.HotelCode).ToList();
                List<List<string>> splitList = commRepo.SplitPropertyList(statiProperties, chunksize);

                var threadResults = new JArray();
                var threadlist = new List<Thread>();
                //for (int i = 0; i < splitList.Count(); i++)
                //{
                //    List<string> htList = splitList[i];
                //    Thread th = new Thread(() =>
                //    {
                //        JArray thResult = GetSearchResult(_hreq, splitList[i], timeOut);
                //        lock (threadResults) // Synchronize access to the shared list
                //        {
                //            if (thResult.Count > 0)
                //            {
                //                threadResults.Merge(thResult);
                //            }
                //        }
                //    });
                //    threadlist.Add(th);
                //}

                List<string> htList = splitList[0];
                Thread th = new Thread(() =>
                {
                    JArray thResult = GetSearchResult(_hreq, splitList[0], timeOut);
                    if (thResult != null)
                    {
                        if (thResult.Count > 0)
                        {
                            threadResults.Merge(thResult);
                        }
                    }
                });
                threadlist.Add(th);
                threadlist.ForEach(t => t.Start());
                threadlist.ForEach(t => t.Join(timeOut));
                threadlist.ForEach(t => t.Abort());
                foreach (JObject hotel in threadResults.Where(x => x["rooms"].Count() > 0))
                {

                    string _currency;

                    var roomList = new List<RoomPrice>();
                    _currency = (string)hotel["rooms"][0]["ratePlans"][0].SelectToken("prices.bar.currency");
                    foreach (JToken room in hotel["rooms"])
                    {
                        //var ratePlans = room["ratePlans"].Where(x => (bool)x["isImmediate"] == true).Select(m => new { price = (decimal)m.SelectToken("prices.bar.price"), currency = (string)m.SelectToken("prices.bar.currency") }).ToList();
                        //var minPrice = ratePlans.Min(x => x.price);
                        foreach (var rate in room["ratePlans"].Where(x => (bool)x["isImmediate"] == true))
                        {
                            var childAgeArray = room.SelectToken("searchedPax.children").Select(x => (int)x).ToArray();
                            var roomItem = new RoomPrice
                            {
                                roomCode = (string)room["roomTypeCode"],
                                roomId = (int)room["roomId"],
                                rateCode = (string)rate["ratePlanCode"],
                                ratePlanId = (int)rate["ratePlanId"],
                                currency = (string)rate.SelectToken("prices.bar.currency"),
                                amount = (decimal)rate.SelectToken("prices.bar.price"),
                                adults = (int)room.SelectToken("searchedPax.adults"),
                                mealPlan = (string)rate["board"],
                                roomKey = (string)room.SelectToken("searchedPax.adults") + "$" + string.Join("-", childAgeArray),
                            };
                            roomList.Add(roomItem);
                        }
                    }
                    var roomIndex = _hreq.RoomList;
                    decimal minrate = 0;
                    foreach (var itm in roomIndex)
                    {
                        minrate += roomList.Where(x => x.roomKey == itm.RoomKey).OrderBy(x => x.amount).First().amount;
                    }
                    string htlCode = (string)hotel["propertyId"];
                    var staticData = hotelData.Where(x => x.HotelCode == htlCode).FirstOrDefault();
                    if (staticData != null)
                    {
                        staticData.Area = "";
                        staticData.Currency = _currency;
                        #region bind hotel data
                        HotelsList.Add(commRepo.BindHotelData(staticData, req, "", minrate, dmc, _hreq.XmlOut.ToString(), _hreq.CustomerId.ToString(), sales_environment));
                        #endregion
                    }

                }
            }
            catch (Exception ex)
            {
                #region Exception

                exLog = new CustomException(ex);
                exLog.MethodName = "HotelAvailability";
                this.WriteException(exLog);

                #endregion


            }
            return HotelsList;
        }

        public JArray GetSearchResult(HotelSearchModel req, List<string> htlCodes, int timeout)
        {
            JArray result = null;
            try
            {
                var reqObj = new RequestModel();
                reqObj.TimeOut = timeout;
                reqObj.StartTime = DateTime.Now;
                reqObj.CustomerId = req.CustomerId;
                reqObj.TrackNo = req.TrackNo;
                reqObj.CallType = ApiAction.Search;
                reqObj.Method = "GET";
                reqObj.RequestStr = $"checkIn={req.CheckIn}&nights={req.Nights}&guests={this.GetGuestString(req.RoomList)}&hotelIds={string.Join(",", htlCodes)}&customerNationality={req.Nationality}";
                reqObj.ResponseStr = repo.GetClientResponse(reqObj);
                if (!string.IsNullOrEmpty(reqObj.ResponseStr))
                {
                    if (reqObj.ResponseStr.StartsWith("{") && reqObj.ResponseStr.EndsWith("}"))
                    {
                        dynamic data = JsonConvert.DeserializeObject(reqObj.ResponseStr);
                        if (data.error == null)
                        {
                            result = (JArray)data.results;
                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                #region Exception

                exLog = new CustomException(ex);
                exLog.MethodName = "GetSearchResult";
                this.WriteException(exLog);

                #endregion
            }
            return result;
        }
        #endregion

        #region Hotel Details
        public XElement HotelDetails(XElement req)
        {
            XElement hotelDesc = new XElement("Hotels");
            XElement HotelDescReq = req.Descendants("hoteldescRequest").FirstOrDefault();
            XElement hotelDescResdoc = new XElement(soapenv + "Envelope", new XAttribute(XNamespace.Xmlns + "soapenv", soapenv),
                new XElement(soapenv + "Header", new XAttribute(XNamespace.Xmlns + "soapenv", soapenv),
                new XElement("Authentication", new XElement("AgentID", req.Descendants("AgentID").Single().Value),
                new XElement("UserName", req.Descendants("UserName").Single().Value),
                new XElement("Password", req.Descendants("Password").Single().Value),
                new XElement("ServiceType", req.Descendants("ServiceType").Single().Value),
                new XElement("ServiceVersion", req.Descendants("ServiceVersion").Single().Value))));

            try
            {
                dynamic hotelObj = null;
                //var hotelObj = htlRepo.GetHotelDetail(req.Descendants("HotelID").FirstOrDefault().Value);
                if (hotelObj != null)
                {
                    StringBuilder sb = new StringBuilder("<h5>" + hotelObj.name + "</h6>");
                    sb.Append("<p>" + hotelObj.address + "</p>");
                    foreach (var item in hotelObj.description_struct)
                    {
                        sb.Append("<h6><b>" + item.title + "</b></h6>");
                        foreach (var text in item.paragraphs)
                        {
                            sb.Append("<p>" + text + "</p>");
                        }
                    }
                    hotelDescResdoc.Add(new XElement(soapenv + "Body", HotelDescReq, new XElement("hoteldescResponse",
                        new XElement("Hotels", new XElement("Hotel",
                        new XElement("HotelID", req.Descendants("HotelID").FirstOrDefault().Value),
                        new XElement("DMC", dmc),
                                        new XElement("Description", sb.ToString()),
                                        HotelImageTag(hotelObj.images),
                                        new XElement("ContactDetails", new XElement("Phone", hotelObj.phone),
                                        new XElement("Fax", "")),
                                        new XElement("CheckinTime", hotelObj.check_in_time), new XElement("CheckoutTime", hotelObj.check_out_time)
                                        )))));
                }
                return hotelDescResdoc;
            }
            catch (Exception ex)
            {
                #region Exception

                exLog = new CustomException(ex);
                exLog.MethodName = "HotelDetails";
                this.WriteException(exLog);

                #endregion

                hotelDescResdoc.Add(new XElement(soapenv + "Body", HotelDescReq,
                    new XElement("hoteldescResponse", new XElement("ErrorTxt", "Hotel detail is not available"))));
                return hotelDescResdoc;
            }
        }
        public XElement HotelImageTag(List<string> imgList)
        {
            XElement mgItem;
            if (!imgList.IsNullOrEmpty())
            {
                var result = from itm in imgList
                             select new XElement("Image", new XAttribute("Path", itm.Replace("{size}", "240x240")));
                mgItem = new XElement("Images", result);
            }
            else
            {
                mgItem = new XElement("Images", null);
            }
            return mgItem;

        }
        #endregion

        #region RoomSearch

        public XElement RoomAvailability(XElement rhReq)
        {
            List<XElement> roomavailabilityresponse = new List<XElement>();
            XElement getrm = null;
            try
            {
                #region changed
                string dmc = string.Empty;
                List<XElement> htlele = rhReq.Descendants("GiataHotelList").Where(x => x.Attribute("GSupID").Value == supplierId.ToString()).ToList();
                for (int i = 0; i < htlele.Count(); i++)
                {
                    string custID = string.Empty;
                    string custName = string.Empty;
                    string htlid = htlele[i].Attribute("GHtlID").Value;
                    string xmlout = string.Empty;
                    try
                    {
                        xmlout = htlele[i].Attribute("xmlout").Value;
                    }
                    catch { xmlout = "false"; }
                    if (xmlout == "true")
                    {
                        try
                        {
                            customerId = htlele[i].Attribute("custID").Value;
                            dmc = htlele[i].Attribute("custName").Value;
                        }
                        catch { custName = "HA"; }
                    }
                    else
                    {
                        try
                        {
                            customerId = htlele[i].Attribute("custID").Value;
                        }
                        catch { }
                    }
                    roomavailabilityresponse.Add(this.GetRoomAvailability(rhReq, htlid));
                }
                #endregion
                getrm = new XElement("TotalRooms", roomavailabilityresponse);
                return getrm;
            }
            catch { return null; }
        }


        public XElement GetRoomAvailability(XElement roomReq, string htlid)
        {
            XElement searchReq = roomReq.Descendants("searchRequest").FirstOrDefault();
            XElement RoomDetails = new XElement(soapenv + "Envelope", new XAttribute(XNamespace.Xmlns + "soapenv", soapenv),
                new XElement(soapenv + "Header", new XAttribute(XNamespace.Xmlns + "soapenv", soapenv),
                new XElement("Authentication", new XElement("AgentID", roomReq.Descendants("AgentID").FirstOrDefault().Value),
                new XElement("UserName", roomReq.Descendants("UserName").FirstOrDefault().Value),
                new XElement("Password", roomReq.Descendants("Password").FirstOrDefault().Value),
                new XElement("ServiceType", roomReq.Descendants("ServiceType").FirstOrDefault().Value),
                new XElement("ServiceVersion", roomReq.Descendants("ServiceVersion").FirstOrDefault().Value))));
            var _hreq = this.BindSearchModel(searchReq, ApiAction.RoomSearch);
            try
            {
                var reqObj = new RequestModel();
                reqObj.StartTime = DateTime.Now;
                reqObj.CustomerId = _hreq.CustomerId;
                reqObj.TrackNo = _hreq.TrackNo;
                reqObj.CallType = ApiAction.RoomSearch;
                reqObj.Method = "GET";
                reqObj.RequestStr = $"checkIn={_hreq.CheckIn}&nights={_hreq.Nights}&guests={this.GetGuestString(_hreq.RoomList)}&hotelIds={htlid}&customerNationality={_hreq.Nationality}";
                reqObj.ResponseStr = repo.GetClientResponse(reqObj);
                if (!string.IsNullOrEmpty(reqObj.ResponseStr))
                {
                    if (reqObj.ResponseStr.StartsWith("{") && reqObj.ResponseStr.EndsWith("}"))
                    {
                        dynamic data = JsonConvert.DeserializeObject(reqObj.ResponseStr);
                        if (data.error == null && data["results"].Count > 0)
                        {
                            List<XElement> roomsResult = new List<XElement>();
                            var hotel = data["results"][0];
                            string _currency;
                            if (hotel["rooms"].Count > 0)
                            {
                                var roomList = new List<RoomPrice>();
                                _currency = (string)hotel["rooms"][0]["ratePlans"][0].SelectToken("prices.bar.currency");
                                foreach (JToken room in hotel["rooms"])
                                {
                                    foreach (var rate in room["ratePlans"].Where(x => (bool)x["isImmediate"] == true))
                                    {
                                        var childAgeArray = room.SelectToken("searchedPax.children").Select(x => (int)x).ToArray();
                                        var roomItem = new RoomPrice
                                        {
                                            roomCode = (string)room["roomTypeCode"],
                                            roomId = (int)room["roomId"],
                                            rateCode = (string)rate["ratePlanCode"],
                                            ratePlanId = (int)rate["ratePlanId"],
                                            currency = (string)rate.SelectToken("prices.bar.currency"),
                                            amount = (decimal)rate.SelectToken("prices.bar.price"),
                                            adults = (int)room.SelectToken("searchedPax.adults"),
                                            mealPlan = (string)rate["board"],
                                            roomKey = (string)room.SelectToken("searchedPax.adults") + "$" + string.Join("-", childAgeArray),
                                            cancelPolicies = this.RoomCxlTag(rate["cancellationPolicies"], rate["nightlyBreakdown"], _hreq.CheckIn, (double)rate.SelectToken("prices.bar.price")),
                                            nightPrice = rate.SelectToken("nightlyBreakdown").Select(x => (double)x.SelectToken("prices.sell.price")).ToList()
                                        };
                                        roomList.Add(roomItem);
                                    }
                                }
                                var roomIndex = _hreq.RoomList;
                                var roomTypeList = roomList.Combinations(roomIndex.Count, combination =>
                                {
                                    int i = 0;
                                    var IsResult = false;
                                    var IsMeal = combination.All(x => x.mealPlan == combination[i].mealPlan);
                                    foreach (var item in combination)
                                    {
                                        if (roomIndex[i].RoomKey == item.roomKey)
                                        {
                                            IsResult = true;
                                        }
                                        else
                                        {
                                            IsResult = false;
                                        }
                                        i++;
                                    }
                                    return (IsResult && IsMeal);
                                }).Take(50);

                                string htlCode = (string)hotel["propertyId"];
                                int counter = 0;
                                foreach (var rmTypes in roomTypeList)
                                {
                                    double _totalPrice = 0;
                                    List<XElement> roomItems = new List<XElement>();
                                    int rmCounter = 0;
                                    foreach (var rmItem in rmTypes)
                                    {
                                        double _roomPrice = rmItem.nightPrice.Sum();
                                        _totalPrice += _roomPrice;
                                        var _roomItem = new XElement("Room",
                                                                       new XAttribute("ID", rmItem.roomId),
                                                                       new XAttribute("SuppliersID", supplierId),
                                                                       new XAttribute("RoomSeq", ++rmCounter),
                                                                       new XAttribute("SessionID", ""),
                                                                       new XAttribute("RoomType", rmItem.roomCode),
                                                                       new XAttribute("OccupancyID", rmItem.ratePlanId),
                                                                       new XAttribute("OccupancyName", ""),
                                                                       new XAttribute("MealPlanID", rmItem.rateCode),
                                                                       new XAttribute("MealPlanName", rmItem.mealPlan),
                                                                       new XAttribute("MealPlanCode", rmItem.mealPlan),
                                                                       new XAttribute("MealPlanPrice", ""),
                                                                       new XAttribute("PerNightRoomRate", rmItem.nightPrice.Average()),
                                                                       new XAttribute("TotalRoomRate", _roomPrice),
                                                                       new XAttribute("CancellationDate", ""),
                                                                       new XAttribute("CancellationAmount", ""),
                                                                       new XAttribute("isAvailable", true),
                                                                       new XElement("RequestID", rmItem.ratePlanId),
                                                                       new XElement("Offers", ""),
                                                                       new XElement("Amenities", new XElement("Amenity", null)),
                                                                       new XElement("Images", null),
                                                                       new XElement("Supplements", null),
                                                                       new XElement("PromotionList", new XElement("Promotions", null)),
                                                                       GetPriceBreakup(rmItem.nightPrice),
                                                                       new XElement("AdultNum", rmItem.adults),
                                                                       new XElement("ChildNum", rmItem.ChildAgeSum)
                                                                       );

                                        roomItems.Add(_roomItem);
                                    }


                                    var cxlList = rmTypes.Select(x => x.cancelPolicies).ToList();
                                    var _b2bPolicy = this.roomTypePolicyTag(cxlList, _totalPrice, _hreq.BufferDays);
                                    string cxlString = EncryptionHelper.Encrypt(_b2bPolicy.policy.ToString(), "hyperGuest");

                                    roomItems.First().Element("RequestID").Value = cxlString;
                                    var roomType = new XElement("RoomTypes",
                                                                    new XAttribute("Index", ++counter),
                                                                    new XAttribute("TotalRate", _totalPrice),
                                                                    new XAttribute("HtlCode", htlCode),
                                                                    new XAttribute("CrncyCode", _currency),
                                                                    new XAttribute("DMCType", dmc),
                                                                    new XAttribute("policy", _b2bPolicy.IsRefundable ? "Refundable" : "Non Refundable"),
                                                                    roomItems);
                                    roomsResult.Add(roomType);
                                }

                                XElement hoteldata = new XElement("Hotels", new XElement("Hotel", new XElement("HotelID"), new XElement("HotelName"), new XElement("PropertyTypeName"),
                                    new XElement("CountryID"), new XElement("CountryName"), new XElement("CityCode"), new XElement("CityName"),
                                    new XElement("AreaId"), new XElement("AreaName"), new XElement("RequestID"), new XElement("Address"), new XElement("Location"),
                                    new XElement("Description"), new XElement("StarRating"), new XElement("MinRate"), new XElement("HotelImgSmall"),
                                    new XElement("HotelImgLarge"), new XElement("MapLink"), new XElement("Longitude"), new XElement("Latitude"), new XElement("DMC", dmc),
                                    new XElement("SupplierID"), new XElement("Currency", _currency), new XElement("Offers"),
                                    new XElement("Rooms", roomsResult)));
                                RoomDetails.Add(new XElement(soapenv + "Body", searchReq, new XElement("searchResponse", hoteldata)));

                            }
                            else
                            {

                                RoomDetails.Add(new XElement(soapenv + "Body", searchReq,
                             new XElement("searchResponse", new XElement("ErrorTxt", data.error))));

                            }
                        }
                    }
                    else
                    {
                        RoomDetails.Add(new XElement(soapenv + "Body", searchReq,
                                      new XElement("searchResponse", new XElement("ErrorTxt", reqObj.ResponseStr))));
                    }
                }
                else
                {
                    RoomDetails.Add(new XElement(soapenv + "Body", searchReq,
                                  new XElement("searchResponse", new XElement("ErrorTxt", "No Response from supplier"))));
                }
            }
            catch (Exception ex)
            {
                #region Exception
                exLog = new CustomException(ex);
                exLog.MethodName = "RoomAvailability";
                this.WriteException(exLog);
                RoomDetails.Add(new XElement(soapenv + "Body", searchReq,
                    new XElement("searchResponse", new XElement("ErrorTxt", "Room is not available"))));
                #endregion

            }
            return RoomDetails;
        }

        #endregion

        #region Cancellation Policy

        public XElement CancellationPolicy(XElement cxlPolicyReq)
        {
            XElement CxlPolicyReqest = cxlPolicyReq.Descendants("hotelcancelpolicyrequest").FirstOrDefault();
            XElement CxlPolicyResponse = new XElement(soapenv + "Envelope", new XAttribute(XNamespace.Xmlns + "soapenv", soapenv), new XElement(soapenv + "Header", new XAttribute(XNamespace.Xmlns + "soapenv", soapenv),
                                       new XElement("Authentication", new XElement("AgentID", cxlPolicyReq.Descendants("AgentID").FirstOrDefault().Value), new XElement("UserName", cxlPolicyReq.Descendants("UserName").FirstOrDefault().Value),
                                       new XElement("Password", cxlPolicyReq.Descendants("Password").FirstOrDefault().Value), new XElement("ServiceType", cxlPolicyReq.Descendants("ServiceType").FirstOrDefault().Value),
                                       new XElement("ServiceVersion", cxlPolicyReq.Descendants("ServiceVersion").FirstOrDefault().Value))));

            try
            {
                //string requestCode = CxlPolicyReqest.Descendants("RequestID").First().Value.ToString();

                var checkIn = CxlPolicyReqest.Element("FromDate").Value.TravayooDateTime();
                var totalRate = CxlPolicyReqest.Descendants("RoomTypes").FirstOrDefault().Attribute("TotalRate").GetValueOrDefault(0m);
                var Refundable = CxlPolicyReqest.Descendants("RoomTypes").FirstOrDefault().Attribute("policy").GetValueOrDefault("");
                bool IsRefundable = (Refundable == "Refundable") ? true : true;


                List<double> roomPrice = CxlPolicyReqest.Descendants("Room").Select(x => Convert.ToDouble(x.Attribute("TotalRoomRate").Value)).ToList();
                var _policy = CxlPolicyReqest.Descendants("Room").First().Element("RequestID").Value;
                bool IsNoPolicy = false;

                if (!string.IsNullOrEmpty(_policy))
                {
                    var _cxlString = EncryptionHelper.Decrypt(_policy, "hyperGuest");
                    XElement _cxlPolicy = XElement.Parse(_cxlString);
                    CxlPolicyResponse.Add(new XElement(soapenv + "Body", cxlPolicyReq,
                         new XElement("HotelDetailwithcancellationResponse",
              new XElement("Hotels", new XElement("Hotel",
              new XElement("HotelID", cxlPolicyReq.Descendants("HotelID").FirstOrDefault().Value),
              new XElement("HotelName"), new XElement("HotelImgSmall"),
              new XElement("HotelImgLarge"), new XElement("MapLink"),
              new XElement("DMC", dmc),
              new XElement("Currency"), new XElement("Offers"),
              new XElement("Rooms",
              new XElement("Room", new XAttribute("ID", cxlPolicyReq.Descendants("Room").FirstOrDefault().Attribute("ID").Value),
              new XAttribute("RoomType", ""),
              new XAttribute("PerNightRoomRate", cxlPolicyReq.Descendants("PerNightRoomRate").FirstOrDefault().Value),
              new XAttribute("TotalRoomRate", cxlPolicyReq.Descendants("TotalRoomRate").FirstOrDefault().Value),
              new XAttribute("LastCancellationDate", ""),
              _cxlPolicy
              )))))));

                }
                else
                {
                    IsNoPolicy = true;

                }
                if (IsNoPolicy)
                {
                    CxlPolicyResponse.Add(new XElement(soapenv + "Body",
                                      CxlPolicyReqest,
                                      new XElement("HotelDetailwithcancellationResponse",
                                      new XElement("ErrorTxt", "No cancellation policy found"))));
                }
                return CxlPolicyResponse;
            }
            catch (Exception ex)
            {
                CxlPolicyResponse.Add(new XElement(soapenv + "Body", CxlPolicyReqest,
                    new XElement("HotelDetailwithcancellationResponse", new XElement("ErrorTxt", ex.Message))));
                #region Exception
                exLog = new CustomException(ex);
                exLog.MethodName = "CancellationPolicy";
                this.WriteException(exLog);
                #endregion
                return CxlPolicyResponse;
            }
        }
        #endregion

        #region PreBooking
        HyGSTPrebookReqModel BindPreBookRequest(XElement req)
        {
            try
            {
                var model = new HyGSTPrebookReqModel
                {
                    search = new Search
                    {
                        dates = new Dates
                        {
                            from = req.Element("FromDate").Value.AlterFormat("dd/MM/yyyy", "yyyy-MM-dd"),
                            to = req.Element("ToDate").Value.AlterFormat("dd/MM/yyyy", "yyyy-MM-dd")
                        },
                        propertyId = req.Element("HotelID").GetValueOrDefault(0),
                        nationality = req.Element("PaxNationality_CountryCode").Value,
                        pax = req.Element("Rooms").Descendants("RoomPax").Select(x => new Pax
                        {
                            adults = x.Element("Adult").GetValueOrDefault(1),
                            children = x.Descendants("ChildAge") != null ? x.Descendants("ChildAge").Select(y => y.GetValueOrDefault(0)).ToList() : new List<int>(),
                        }).ToList(),
                    },
                    rooms = req.Descendants("Room").Select(x => new Room
                    {
                        // roomId = x.Attribute("ID").GetValueOrDefault(0),
                        roomCode = x.Attribute("RoomType").GetValueOrDefault(""),
                        //ratePlanId = x.Attribute("OccupancyID").GetValueOrDefault(0),
                        rateCode = x.Attribute("MealPlanID").GetValueOrDefault(""),
                        expectedPrice = new ExpectedPrice
                        {
                            amount = x.Attribute("TotalRoomRate").GetValueOrDefault(0.0),
                            currency = req.Element("CurrencyName").GetValueOrDefault(""),
                        }
                    }).ToList()
                };
                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public XElement PreBooking(XElement rhReq)
        {

            XElement preBookReq = rhReq.Descendants("HotelPreBookingRequest").FirstOrDefault();
            XElement PreBookResponse = new XElement(soapenv + "Envelope", new XAttribute(XNamespace.Xmlns + "soapenv", soapenv), new XElement(soapenv + "Header", new XAttribute(XNamespace.Xmlns + "soapenv", soapenv),
                                       new XElement("Authentication", new XElement("AgentID", rhReq.Descendants("AgentID").FirstOrDefault().Value), new XElement("UserName", rhReq.Descendants("UserName").FirstOrDefault().Value),
                                       new XElement("Password", rhReq.Descendants("Password").FirstOrDefault().Value), new XElement("ServiceType", rhReq.Descendants("ServiceType").FirstOrDefault().Value),
                                       new XElement("ServiceVersion", rhReq.Descendants("ServiceVersion").FirstOrDefault().Value))));
            var checkIn = preBookReq.Element("FromDate").Value.TravayooDateTime();
            var _hreq = this.BindPreBookRequest(preBookReq);

            try
            {
                var reqObj = new RequestModel();
                reqObj.StartTime = DateTime.Now;
                reqObj.CustomerId = Convert.ToInt64(preBookReq.Element("CustomerID").Value);
                reqObj.TrackNo = preBookReq.Element("TransID").Value;
                reqObj.CallType = ApiAction.PreBook;
                reqObj.Method = "POST";
                reqObj.RequestStr = JsonConvert.SerializeObject(_hreq);
                reqObj.ResponseStr = repo.PostClientResponse(reqObj);
                if (!string.IsNullOrEmpty(reqObj.ResponseStr))
                {
                    if (reqObj.ResponseStr.StartsWith("{") && reqObj.ResponseStr.EndsWith("}"))
                    {
                        dynamic data = JsonConvert.DeserializeObject(reqObj.ResponseStr);

                        if (data.error == null && data["content"]["rooms"].Count > 0)
                        {
                            string _currency;
                            XElement roomType = null;
                            var roomList = new List<RoomPrice>();
                            foreach (JToken room in data["content"]["rooms"])
                            {
                                var childAgeArray = room.SelectToken("searchedPax.children").Select(x => (int)x).ToArray();
                                var roomItem = new RoomPrice
                                {
                                    roomCode = (string)room["roomTypeCode"],
                                    roomId = (int)room["roomId"],
                                    rateCode = (string)room["ratePlanCode"],
                                    ratePlanId = (int)room["ratePlanId"],

                                    currency = (string)room.SelectToken("prices.bar.currency"),
                                    amount = (decimal)room.SelectToken("prices.bar.price"),
                                    adults = (int)room.SelectToken("searchedPax.adults"),
                                    //mealPlan = (string)rate["board"],
                                    roomKey = (string)room.SelectToken("searchedPax.adults") + "$" + string.Join("-", childAgeArray),
                                    cancelPolicies = this.RoomCxlTag(room["cancellationPolicies"], room["nightlyRates"], checkIn.ToString(), (double)room.SelectToken("prices.bar.price")),
                                    nightPrice = room.SelectToken("nightlyRates").Select(x => (double)x.SelectToken("prices.sell.price")).ToList()
                                };
                                roomList.Add(roomItem);
                            }

                            double _totalPrice = 0;
                            List<XElement> roomItems = new List<XElement>();
                            int rmCounter = 0;
                            foreach (var rmItem in roomList)
                            {
                                double _roomPrice = rmItem.nightPrice.Sum();
                                _totalPrice += _roomPrice;
                                var _roomItem = new XElement("Room",
                                                               new XAttribute("ID", rmItem.roomId),
                                                               new XElement("RequestID", rmItem.ratePlanId),
                                                               new XAttribute("SuppliersID", supplierId),
                                                               new XAttribute("RoomSeq", ++rmCounter),
                                                               new XAttribute("SessionID", ""),
                                                               new XAttribute("RoomType", rmItem.roomCode),
                                                               new XAttribute("OccupancyID", rmItem.rateCode),
                                                               new XAttribute("OccupancyName", ""),
                                                               new XAttribute("MealPlanID", ""),
                                                               new XAttribute("MealPlanName", rmItem.mealPlan),
                                                               new XAttribute("MealPlanCode", rmItem.mealPlan),
                                                               new XAttribute("MealPlanPrice", ""),
                                                               new XAttribute("PerNightRoomRate", rmItem.nightPrice.Average()),
                                                               new XAttribute("TotalRoomRate", _roomPrice),
                                                               new XAttribute("CancellationDate", ""),
                                                               new XAttribute("CancellationAmount", ""),
                                                               new XAttribute("isAvailable", true),
                                                               new XElement("Offers", ""),
                                                               new XElement("Amenities", new XElement("Amenity", null)),
                                                               new XElement("Images", null),
                                                               new XElement("Supplements", null),
                                                               //RoomPromotion(room["RoomPromotion"]),
                                                               GetPriceBreakup(rmItem.nightPrice),
                                                               new XElement("AdultNum", rmItem.adults),
                                                               new XElement("ChildNum", rmItem.ChildAgeSum)
                                                               );
                                roomItems.Add(_roomItem);
                            }


                            var cxlList = roomList.Select(x => x.cancelPolicies).ToList();
                            var _b2bPolicy = this.roomTypePolicyTag(cxlList, _totalPrice, 3);

                            roomType = new XElement("RoomTypes",
                                                               new XAttribute("Index", 1),
                                                                new XAttribute("TotalRate", _totalPrice),
                                                            new XAttribute("HtlCode", ""),
                                                            new XAttribute("CrncyCode", ""),
                                                               new XAttribute("DMCType", dmc),
                                                               new XAttribute("policy", _b2bPolicy.IsRefundable ? "Refundable" : "Non Refundable"),
                                                               roomItems,
                                                               _b2bPolicy.policy);

                            XElement hoteldata = new XElement("Hotels", new XElement("Hotel", new XElement("HotelID", rhReq.Descendants("HotelID").FirstOrDefault().Value),
                                                 new XElement("HotelName", rhReq.Descendants("HotelName").FirstOrDefault().Value), new XElement("Status", true),
                                                 new XElement("TermCondition", ""), new XElement("HotelImgSmall"), new XElement("HotelImgLarge"),
                                                 new XElement("MapLink"), new XElement("DMC", dmc),
                                                 new XElement("RequestID", ""),
                                                 new XElement("Currency", ""),
                                                 new XElement("Offers"), new XElement("Rooms", roomType)));
                            double oldprice = preBookReq.Descendants("RoomTypes").First().Attribute("TotalRate").GetValueOrDefault(0);


                            if (oldprice == _totalPrice)
                            {
                                PreBookResponse.Add(new XElement(soapenv + "Body", preBookReq,
                      new XElement("HotelPreBookingResponse",
                      new XElement("NewPrice", ""), hoteldata)));
                            }
                            else
                            {
                                PreBookResponse.Add(new XElement(soapenv + "Body", preBookReq,
                                  new XElement("HotelPreBookingResponse",
                                  new XElement("ErrorTxt", "Hotel Price has been changed"),
                                  new XElement("NewPrice", _totalPrice), hoteldata)));
                            }

                        }
                        else
                        {
                            PreBookResponse.Add(new XElement(soapenv + "Body", preBookReq, new XElement("HotelPreBookingResponse",
                                new XElement("ErrorTxt", data.error))));
                        }
                    }
                    else
                    {
                        PreBookResponse.Add(new XElement(soapenv + "Body", preBookReq, new XElement("HotelPreBookingResponse",
                            new XElement("ErrorTxt", "Room is not available"))));
                    }
                    return PreBookResponse;
                }
                else
                {
                    PreBookResponse.Add(new XElement(soapenv + "Body", preBookReq, new XElement("HotelPreBookingResponse",
                           new XElement("ErrorTxt", "No response from supplier has been recorded"))));

                }
            }
            catch (Exception ex)
            {
                #region Exception
                exLog = new CustomException(ex);
                exLog.MethodName = "PreBooking";
                this.WriteException(exLog);
                PreBookResponse.Add(new XElement(soapenv + "Body", preBookReq, new XElement("HotelPreBookingResponse",
                           new XElement("ErrorTxt", ex.Message))));
                #endregion

            }
            return PreBookResponse;
        }

        #endregion

        #region Booking

        HyGSTBookReqModel BindOrderBookingReq(XElement req)
        {
            var _guest = req.Descendants("PaxInfo").Where(x => x.Element("IsLead").Value == "true").FirstOrDefault();

            string clientRef = DateTime.UtcNow.ToString("ddMMyyHHmmssfff") + "#" + req.Descendants("TransID").FirstOrDefault().Value + "#" + req.Descendants("CustomerID").FirstOrDefault().Value;
            var model = new HyGSTBookReqModel()
            {
                dates = new Dates
                {
                    from = req.Element("FromDate").Value,
                    to = req.Element("ToDate").Value
                },
                propertyId = req.Element("HotelID").GetValueOrDefault(0),
                leadGuest = new LeadGuest
                {
                    birthDate = null,
                    contact = new Contact { email = _guest.Element("Email").Value.Trim(), phone = _guest.Element("Phone").Value.Trim() },
                    name = new Name { first = _guest.Element("FirstName").Value.Trim(), last = _guest.Element("LastName").Value.Trim() },
                    title = _guest.Element("LastName").Value.Trim()
                },
                reference = new Reference { agency = req.Element("TransID").Value },
                rooms = (from rm in req.Descendants("Room")
                         select new RoomsItem
                         {
                             roomCode = rm.Attribute("RoomType").GetValueOrDefault(""),
                             rateCode = rm.Attribute("MealPlanID").GetValueOrDefault(""),
                             expectedPrice = new ExpectedPrice
                             {
                                 amount = rm.Attribute("TotalRoomRate").GetValueOrDefault(0),
                                 currency = req.Element("CurrencyCode").GetValueOrDefault(""),
                             },
                             specialRequests = null,
                             guests = rm.Descendants("PaxInfo").Select(x => new GuestsItem
                             {
                                 birthDate = null,
                                 contact = new Contact { email = x.Element("Email").Value.Trim(), phone = x.Element("Phone").Value.Trim() },
                                 title = x.Element("Title").Value.Trim(),
                                 name = new Name { first = x.Element("FirstName").Value.Trim(), last = x.Element("LastName").Value.Trim() }

                             }).ToList(),
                         }).ToList(),
            };
            return model;
        }
        public XElement Booking(XElement BookingReq)
        {
            XElement BookReq = BookingReq.Descendants("HotelBookingRequest").FirstOrDefault();
            XElement HotelBookingResp = new XElement(soapenv + "Envelope", new XAttribute(XNamespace.Xmlns + "soapenv", soapenv), new XElement(soapenv + "Header", new XAttribute(XNamespace.Xmlns + "soapenv", soapenv),
                                       new XElement("Authentication", new XElement("AgentID", BookingReq.Descendants("AgentID").FirstOrDefault().Value), new XElement("UserName", BookingReq.Descendants("UserName").FirstOrDefault().Value), new XElement("Password", BookingReq.Descendants("Password").FirstOrDefault().Value),
                                       new XElement("ServiceType", BookingReq.Descendants("ServiceType").FirstOrDefault().Value), new XElement("ServiceVersion", BookingReq.Descendants("ServiceVersion").FirstOrDefault().Value))));

            try
            {
                string _status = "Success";
                var _req = BindOrderBookingReq(BookReq);
                var reqObj = new RequestModel();
                reqObj.StartTime = DateTime.Now;
                reqObj.CustomerId = Convert.ToInt64(BookReq.Element("CustomerID").Value);
                reqObj.TrackNo = BookReq.Element("TransactionID").Value;
                reqObj.ActionId = (int)BookReq.Name.LocalName.GetActions();
                reqObj.Action = BookReq.Name.LocalName.GetActions().ToString();
                reqObj.TimeOut = 300000;
                reqObj.RequestStr = JsonConvert.SerializeObject(_req);
                reqObj.ResponseStr = repo.PostClientResponse(reqObj);
                if (!string.IsNullOrEmpty(reqObj.ResponseStr))
                {
                    if (reqObj.ResponseStr.StartsWith("{") && reqObj.ResponseStr.EndsWith("}"))
                    {
                        dynamic data = JsonConvert.DeserializeObject(reqObj.ResponseStr);
                        if (data.Status.Code == 200 && data.Status.Description == "Successful")
                        {
                            JObject result = (JObject)data;
                            XElement BookingRes = new XElement("HotelBookingResponse",
                                 new XElement("Hotels", new XElement("HotelID", BookingReq.Descendants("HotelID").FirstOrDefault().Value),
                                 new XElement("HotelName", BookingReq.Descendants("HotelName").FirstOrDefault().Value),
                                 new XElement("FromDate", BookingReq.Descendants("FromDate").FirstOrDefault().Value),
                                 new XElement("ToDate", BookingReq.Descendants("ToDate").FirstOrDefault().Value),
                                 new XElement("AdultPax", BookingReq.Descendants("Rooms").Descendants("RoomPax").Descendants("Adult").FirstOrDefault().Value),
                                 new XElement("ChildPax", BookingReq.Descendants("Rooms").Descendants("RoomPax").Descendants("Child").FirstOrDefault().Value),
                                 new XElement("TotalPrice", BookingReq.Descendants("TotalAmount").FirstOrDefault().Value),
                                 new XElement("CurrencyID"),
                                 new XElement("DMC", dmc),
                                 new XElement("CurrencyCode", BookingReq.Descendants("CurrencyCode").FirstOrDefault().Value),
                                 new XElement("MarketID"), new XElement("MarketName"), new XElement("HotelImgSmall"),
                                 new XElement("HotelImgLarge"), new XElement("MapLink"), new XElement("VoucherRemark"),
                                 new XElement("TransID", BookingReq.Descendants("TransID").FirstOrDefault().Value),
                                 new XElement("ConfirmationNumber", (string)result["ConfirmationNumber"]),
                                 new XElement("Status", _status),
                                 new XElement("PassengersDetail", new XElement("GuestDetails",
                                 from room in BookingReq.Descendants("Room")
                                 select new XElement("Room", new XAttribute("ID", room.Attribute("RoomTypeID").Value), new XAttribute("RoomType", room.Attribute("RoomType").Value), new XAttribute("ServiceID", ""),
                                 new XAttribute("RefNo", string.Empty),
                                 new XAttribute("MealPlanID", ""), new XAttribute("MealPlanName", ""),
                                 new XAttribute("MealPlanCode", ""), new XAttribute("MealPlanPrice", ""), new XAttribute("PerNightRoomRate", ""),
                                 new XAttribute("RoomStatus", "true"), new XAttribute("TotalRoomRate", ""),
                                 new XElement("RoomGuest", new XElement("GuestType", "Adult"), new XElement("Title"), new XElement("FirstName", room.Descendants("PaxInfo").FirstOrDefault().Descendants("FirstName").FirstOrDefault().Value),
                                 new XElement("MiddleName"), new XElement("LastName", room.Descendants("PaxInfo").FirstOrDefault().Descendants("LastName").FirstOrDefault().Value), new XElement("IsLead", "true"), new XElement("Age")),
                                 new XElement("Supplements"))
                                 ))));

                            HotelBookingResp.Add(new XElement(soapenv + "Body", BookReq, BookingRes));

                            string Confirmation = result["ConfirmationNumber"] != null ? (string)result["ConfirmationNumber"] : string.Empty;


                        }
                        else
                        {
                            _status = "Fail";
                            HotelBookingResp.Add(new XElement(soapenv + "Body", BookReq,
                                new XElement("HotelBookingResponse",
                                new XElement("ErrorTxt", data.Status.Description))));
                        }
                    }
                    else
                    {
                        _status = "Fail";
                        HotelBookingResp.Add(new XElement(soapenv + "Body", BookReq,
                            new XElement("HotelBookingResponse",
                            new XElement("ErrorTxt", reqObj.ResponseStr))));
                    }
                }
                else
                {
                    _status = "Fail";
                    HotelBookingResp.Add(new XElement(soapenv + "Body", BookReq,
                        new XElement("HotelBookingResponse",
                        new XElement("ErrorTxt", "There is some technical error, Please check the exception log!"))));
                }

                return HotelBookingResp;
            }
            catch (Exception ex)
            {
                exLog = new CustomException(ex);
                exLog.MethodName = "HotelBookingConfirm";
                this.WriteException(exLog);
                HotelBookingResp.Add(new XElement(soapenv + "Body", BookReq,
                    new XElement("HotelBookingResponse",
                    new XElement("ErrorTxt", exLog.MsgName))));
                return HotelBookingResp;

            }
        }

        #endregion

        #region Cancellation Booking
        public XElement Cancelllation(XElement cancelReq)
        {
            string cancelStatus = "";
            XElement CxlReq = cancelReq.Descendants("HotelCancellationRequest").FirstOrDefault();
            XElement BookCXlRes = new XElement(soapenv + "Envelope", new XAttribute(XNamespace.Xmlns + "soapenv", soapenv), new XElement(soapenv + "Header", new XAttribute(XNamespace.Xmlns + "soapenv", soapenv),
                                  new XElement("Authentication", new XElement("AgentID", cancelReq.Descendants("AgentID").FirstOrDefault().Value), new XElement("UserName", cancelReq.Descendants("UserName").FirstOrDefault().Value),
                                  new XElement("Password", cancelReq.Descendants("Password").FirstOrDefault().Value), new XElement("ServiceType", cancelReq.Descendants("ServiceType").FirstOrDefault().Value),
                                  new XElement("ServiceVersion", cancelReq.Descendants("ServiceVersion").FirstOrDefault().Value))));

            try
            {
                var _req = new
                {
                    bookingId = cancelReq.Descendants("ConfirmationNumber").FirstOrDefault().Value,
                    reason = "Customer want to cancel the booking at supplier.",
                    simulation = false
                };
                var reqObj = new RequestModel();
                reqObj.StartTime = DateTime.Now;
                reqObj.CustomerId = Convert.ToInt64(CxlReq.Element("CustomerID").Value);
                reqObj.TrackNo = CxlReq.Element("TransID").Value;
                reqObj.ActionId = (int)CxlReq.Name.LocalName.GetActions();
                reqObj.Action = CxlReq.Name.LocalName.GetActions().ToString();
                reqObj.RequestStr = JsonConvert.SerializeObject(_req);
                reqObj.ResponseStr = repo.PostClientResponse(reqObj);
                if (!string.IsNullOrEmpty(reqObj.ResponseStr))
                {
                    if (reqObj.ResponseStr.StartsWith("{") && reqObj.ResponseStr.EndsWith("}"))
                    {
                        dynamic data = JsonConvert.DeserializeObject(reqObj.ResponseStr);
                        if (data.Status.Code == 200 && data.Status.Description == "Cancelled")
                        {
                            cancelStatus = "Success";
                        }
                        else
                        {
                            cancelStatus = "Fail";

                        }
                        BookCXlRes.Add(new XElement(soapenv + "Body", CxlReq, new XElement("HotelCancellationResponse",
           new XElement("Rooms",
           new XElement("Room",
           new XElement("Cancellation",
       new XElement("Amount", "0.00"),
           new XElement("Status", cancelStatus)))))));

                    }
                    else
                    {
                        BookCXlRes.Add(new XElement(soapenv + "Body", CxlReq, new XElement("HotelCancellationResponse", new XElement("ErrorTxt", reqObj.ResponseStr))));
                    }
                }
                else
                {
                    BookCXlRes.Add(new XElement(soapenv + "Body", CxlReq, new XElement("HotelCancellationResponse", new XElement("ErrorTxt", "There is some technical error, Please check the exception log!"))));

                }
                return BookCXlRes;

            }
            catch (Exception ex)
            {
                exLog = new CustomException(ex);
                exLog.MethodName = "BookingCancellation";
                this.WriteException(exLog);
                BookCXlRes.Add(new XElement(soapenv + "Body", CxlReq, new XElement("HotelCancellationResponse", new XElement("ErrorTxt", exLog.MsgName))));
                return BookCXlRes;
            }
        }

        #endregion



        public void GetAllHotels()
        {

            var reqObj = new RequestModel();
            reqObj.StartTime = DateTime.Now;
            //reqObj.StartTime = DateTime.Now;
            //reqObj.CustomerId = req.CustomerId;
            //reqObj.TrackNo = req.TrackNo;
            reqObj.CallType = ApiAction.Hotels;
            reqObj.Method = "GET";
            reqObj.RequestStr = "hotels.json";
            
            reqObj.ResponseStr = repo.GetStaticResponse(reqObj);

            if (!string.IsNullOrEmpty(reqObj.ResponseStr))
            {
                if (reqObj.ResponseStr.StartsWith("{") && reqObj.ResponseStr.EndsWith("}"))
                {
                }
            }

        }



        #region Dispose
        /// <summary>
        /// Dispose all used resources.
        /// </summary>

        private bool _disposed = false;

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                model = null;
                repo = null;
                _saveEx = null;

                // Free any other managed objects here.
                //
            }
            // Free any unmanaged objects here.
            //
            _disposed = true;
        }
        ~HYGSTServices()
        {
            Dispose(false);
        }
        #endregion







    }
}
