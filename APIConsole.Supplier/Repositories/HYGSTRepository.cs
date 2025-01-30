using APIConsole.Supplier.DataAccess;
using APIConsole.Supplier.Helpers;
using APIConsole.Supplier.Models;
using APIConsole.Supplier.Models.Common;
using APIConsole.Supplier.Models.HYGST;
using RestSharp;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;


namespace APIConsole.Supplier.Repositories
{
    public class HYGSTRepository : IDisposable
    {
        HYGSTCredentials model;
        SaveAPILog apilog;
        APILogDetail log;
        CustomException custEx;
        string _filePath = @"/App_Data/";
        public HYGSTRepository(HYGSTCredentials _model)
        {
            model = _model;
            apilog = new SaveAPILog();
            log = new APILogDetail();
        }

        public string GetClientResponse(RequestModel reqModel)
        {
            string responseBody = string.Empty;
            try
            {
                string proxyURL = string.Empty;
                proxyURL = model.BaseUrl + "?" + reqModel.RequestStr;
                var client = new RestClient(proxyURL);
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AddHeader("Authorization", "Bearer " + model.Token);
                request.AddHeader("Accept-Encoding", "gzip, deflate");
                IRestResponse response = client.Execute(request);
                responseBody = response.Content;
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    WebResponse webEx = ex.Response;
                    using (StreamReader reader = new StreamReader(webEx.GetResponseStream()))
                    {
                        responseBody = reader.ReadToEnd();
                    }
                    webEx.Close();
                }
                custEx = new CustomException(ex);
                custEx.MethodName = "issue while reading client response";
                custEx.PageName = "HYGSTRepository";
                custEx.CustomerID = reqModel.CustomerId.ToString();
                custEx.TranID = reqModel.TrackNo;
                apilog.SendCustomExcepToDB(custEx);
                log.logMsg = ex.Message.ToString();
            }
            catch (Exception ex)
            {
                custEx = new CustomException(ex);
                custEx.MethodName = "issue while reading client response";
                custEx.PageName = "HYGSTRepository";
                custEx.CustomerID = reqModel.CustomerId.ToString();
                custEx.TranID = reqModel.TrackNo;
                apilog.SendCustomExcepToDB(custEx);
                log.logMsg = ex.Message.ToString();
            }
            finally
            {
                log.customerID = reqModel.CustomerId;
                log.LogTypeID = (short)reqModel.CallType;
                log.LogType = reqModel.CallType.ToString();
                log.SupplierID = model.SupplierId;
                log.TrackNumber = reqModel.TrackNo;
                log.logrequestXML = reqModel.RequestStr;

                log.EndTime = DateTime.Now;

                try
                {
                    log.logresponseXML = responseBody;
                    if (reqModel.CallType == ApiAction.Search)
                    {
                        apilog.SaveAPILogs_search(log);
                    }
                    if (reqModel.CallType == ApiAction.RoomSearch)
                    {
                        apilog.SaveAPILogs_room(log);
                    }
                    else
                    {
                        apilog.SaveAPILogs(log);
                    }

                }
                catch (Exception ex)
                {
                    custEx = new CustomException(ex);
                    custEx.MethodName = "error on saving client response";
                    custEx.PageName = "HYGSTRepository";
                    custEx.CustomerID = log.customerID.ToString();
                    custEx.TranID = log.TrackNumber;
                    apilog.SendCustomExcepToDB(custEx);
                    log.logMsg = ex.Message.ToString();
                    log.logresponseXML = responseBody;
                    apilog.SaveAPILogwithResponseError(log);
                }
            }
            return responseBody;

        }


        public string PostClientResponse(RequestModel reqModel)
        {
            string responseBody = string.Empty;
            try
            {

                var client = new RestClient(model.BaseUrl);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddJsonBody(reqModel.RequestStr);
                request.AddHeader("Authorization", "Bearer " + model.Token);
                request.AddHeader("Accept-Encoding", "gzip, deflate");
                IRestResponse response = client.Execute(request);
                responseBody = response.Content;
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    WebResponse webEx = ex.Response;
                    using (StreamReader reader = new StreamReader(webEx.GetResponseStream()))
                    {
                        responseBody = reader.ReadToEnd();
                    }
                    webEx.Close();
                }
                custEx = new CustomException(ex);
                custEx.MethodName = "issue while reading client response";
                custEx.PageName = "HYGSTRepository";
                custEx.CustomerID = reqModel.CustomerId.ToString();
                custEx.TranID = reqModel.TrackNo;
                apilog.SendCustomExcepToDB(custEx);
                log.logMsg = ex.Message.ToString();
            }
            catch (Exception ex)
            {
                custEx = new CustomException(ex);
                custEx.MethodName = "issue while reading client response";
                custEx.PageName = "HYGSTRepository";
                custEx.CustomerID = reqModel.CustomerId.ToString();
                custEx.TranID = reqModel.TrackNo;
                apilog.SendCustomExcepToDB(custEx);
                log.logMsg = ex.Message.ToString();
            }
            finally
            {
                log.customerID = reqModel.CustomerId;
                log.LogTypeID = (short)reqModel.CallType;
                log.LogType = reqModel.CallType.ToString();
                log.SupplierID = model.SupplierId;
                log.TrackNumber = reqModel.TrackNo;
                log.logrequestXML = reqModel.RequestStr;

                log.EndTime = DateTime.Now;

                try
                {
                    log.logresponseXML = responseBody;
                    if (reqModel.CallType == ApiAction.Search)
                    {
                        apilog.SaveAPILogs_search(log);
                    }
                    if (reqModel.CallType == ApiAction.RoomSearch)
                    {
                        apilog.SaveAPILogs_room(log);
                    }
                    else
                    {
                        apilog.SaveAPILogs(log);
                    }

                }
                catch (Exception ex)
                {
                    custEx = new CustomException(ex);
                    custEx.MethodName = "error on saving client response";
                    custEx.PageName = "HYGSTRepository";
                    custEx.CustomerID = log.customerID.ToString();
                    custEx.TranID = log.TrackNumber;
                    apilog.SendCustomExcepToDB(custEx);
                    log.logMsg = ex.Message.ToString();
                    log.logresponseXML = responseBody;
                    apilog.SaveAPILogwithResponseError(log);
                }
            }
            return responseBody;

        }






        public string GetStaticResponse(RequestModel reqModel)
        {
            string responseBody = string.Empty;
            try
            {
                string proxyURL = model.BaseUrl;

                if (!string.IsNullOrEmpty(reqModel.RequestStr))
                {
                    proxyURL += reqModel.RequestStr;
                }
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)(0xc0 | 0x300 | 0xc00);
                var client = new RestClient(proxyURL);
                var request = new RestRequest(Method.GET);
                request.AddHeader("Authorization", "Bearer " + model.Token);
                request.AddHeader("Accept-Encoding", "gzip, deflate");
                request.AddHeader("KeepAlive", "true");
                IRestResponse response = client.Execute(request);
                responseBody = response.Content;


                string _path = TravayooHelper.CreateIfMissing("supplier_28");
                string filePath = Path.Combine(_path, string.Format("HotelList.{0}", "json"));
                File.WriteAllText(filePath, responseBody.ToString());



            }
            catch (Exception ex)
            {
                throw ex;
            }
            return responseBody;
        }




        public int UploadJson(string _response)
        {


            SqlParameter[] pList = new SqlParameter[2];
            var flag = new SqlParameter();
            flag.ParameterName = "@flag";
            flag.Direction = ParameterDirection.Input;
            flag.SqlDbType = SqlDbType.Int;
            flag.Value = 1;
            pList[0] = flag;
            var json = new SqlParameter();
            json.ParameterName = "@jsonData";
            json.Direction = ParameterDirection.Input;
            json.SqlDbType = SqlDbType.NVarChar;
            json.Value = _response;
            pList[1] = json;

            string _constr = ConfigurationManager.ConnectionStrings["INGMContext.Static"].ConnectionString;
            var _dataAcess = new TravayooDataAcess(_constr);

            int result = _dataAcess.Insert("HYGSTProc", pList);
            return result;
        }







        public string PostClientResponseer(RequestModel reqModel)
        {
            log.StartTime = reqModel.StartTime;
            string responseString = string.Empty;
            try
            {
                string proxyURL = string.Empty;

                if (reqModel.Method == "GET")
                {

                    proxyURL = model.BaseUrl + "?" + reqModel.RequestStr;
                }
                else
                {
                    proxyURL = model.BaseUrl;
                }
                //proxyURL = HttpUtility.UrlDecode(proxyURL);
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                ServicePointManager.SecurityProtocol = (SecurityProtocolType)(0xc0 | 0x300 | 0xc00);
                HttpWebRequest myhttprequest = (HttpWebRequest)HttpWebRequest.Create(proxyURL);

                myhttprequest.Method = reqModel.Method;
                myhttprequest.Headers.Add("Authorization", "Bearer " + model.Token);
                myhttprequest.Headers.Add("Accept-Encoding", "gzip,deflate");
                myhttprequest.ContentType = "application/json";
                myhttprequest.Accept = "application/json";
                using (var response = (HttpWebResponse)myhttprequest.GetResponse())
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        responseString = reader.ReadToEnd();
                        reader.Close();
                    }
                    response.Close();
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    WebResponse webEx = ex.Response;
                    using (StreamReader reader = new StreamReader(webEx.GetResponseStream()))
                    {
                        responseString = reader.ReadToEnd();
                    }
                    webEx.Close();
                }
                custEx = new CustomException(ex);
                custEx.MethodName = "issue while reading client response";
                custEx.PageName = "HYGSTRepository";
                custEx.CustomerID = reqModel.CustomerId.ToString();
                custEx.TranID = reqModel.TrackNo;
                apilog.SendCustomExcepToDB(custEx);
                log.logMsg = ex.Message.ToString();
            }
            catch (Exception ex)
            {
                custEx = new CustomException(ex);
                custEx.MethodName = "issue while reading client response";
                custEx.PageName = "HYGSTRepository";
                custEx.CustomerID = reqModel.CustomerId.ToString();
                custEx.TranID = reqModel.TrackNo;
                apilog.SendCustomExcepToDB(custEx);
                log.logMsg = ex.Message.ToString();
            }
            finally
            {
                log.customerID = reqModel.CustomerId;
                log.LogTypeID = (short)reqModel.CallType;
                log.LogType = reqModel.CallType.ToString();
                log.SupplierID = model.SupplierId;
                log.TrackNumber = reqModel.TrackNo;
                log.logrequestXML = reqModel.RequestStr;

                log.EndTime = DateTime.Now;

                try
                {
                    log.logresponseXML = responseString;
                    if (reqModel.CallType == ApiAction.Search)
                    {
                        apilog.SaveAPILogs_search(log);
                    }
                    if (reqModel.CallType == ApiAction.RoomSearch)
                    {
                        apilog.SaveAPILogs_room(log);
                    }
                    else
                    {
                        apilog.SaveAPILogs(log);
                    }

                }
                catch (Exception ex)
                {
                    custEx = new CustomException(ex);
                    custEx.MethodName = "error on saving client response";
                    custEx.PageName = "HYGSTRepository";
                    custEx.CustomerID = log.customerID.ToString();
                    custEx.TranID = log.TrackNumber;
                    apilog.SendCustomExcepToDB(custEx);
                    log.logMsg = ex.Message.ToString();
                    log.logresponseXML = responseString;
                    apilog.SaveAPILogwithResponseError(log);
                }
            }
            return responseString;
        }

        #region Dispose
        /// <summary>
        /// Dispose all used resources.
        /// </summary>
        bool disposed = false;

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                model = null;
                apilog = null;
                log = null;
                custEx = null;
                // Free any other managed objects here.
                //
            }

            // Free any unmanaged objects here.
            //
            disposed = true;
        }

        ~HYGSTRepository()
        {
            Dispose(false);
        }
        #endregion
    }
}


