﻿using System;

namespace APIConsole.Supplier.Models.Common
{
    public class APILogDetail
    {
        public long customerID
        {
            get;
            set;
        }
        public long LogTypeID
        {
            get;
            set;
        }
    
        public string LogType
        {
            get;
            set;
        }
        public long SupplierID
        {
            get;
            set;
        }
        public string TrackNumber
        {
            get;
            set;
        }
        public string preID
        {
            get;
            set;
        }
        public string logMsg
        {
            get;
            set;
        }
        public string logrequestXML
        {
            get;
            set;
        }
        public string logresponseXML
        {
            get;
            set;
        }
        public int logStatus
        {
            get;
            set;
        }
        public DateTime? StartTime
        {
            get;
            set;
        }

        public DateTime? EndTime
        {
            get;
            set;
        }
        public string HotelId
        {
            get;
            set;
        }
    }
}