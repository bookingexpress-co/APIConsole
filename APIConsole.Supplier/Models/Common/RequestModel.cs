using System;

namespace APIConsole.Supplier.Models.Common
{
    public class RequestModel
    {
        public RequestModel()
        {
            this.TimeOut = 1200000;
        }


        public bool IsResult
        {
            get;
            set;
        }

        public string HostName
        {
            get;
            set;
        }
        public string Method
        {
            get;
            set;
        }
        public string ContentType
        {
            get;
            set;
        }
        public string RequestStr
        {
            get;
            set;
        }
        public string ResponseStr
        {
            get;
            set;
        }

        public DateTime StartTime
        {
            get;
            set;
        }
        public DateTime EndTime
        {
            get;
            set;
        }
        public int TimeOut
        {
            get;
            set;
        }


        public string Header
        {
            get;
            set;
        }

        public long CustomerId
        {
            get;
            set;
        }

        public string TrackNo
        {
            get;
            set;
        }

        public int SupplierId
        {
            get;
            set;
        }

        public int ActionId
        {
            get;
            set;
        }
        public string Action
        {
            get;
            set;
        }

        public ApiAction CallType
        {
            get;
            set;
        }
        public string IpAddress
        {
            get;
            set;
        }



    }
}