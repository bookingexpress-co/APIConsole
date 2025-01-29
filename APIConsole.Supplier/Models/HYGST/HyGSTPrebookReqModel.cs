using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIConsole.Supplier.Models.HYGST
{
    public class HyGSTPrebookReqModel
    {
        public Search search { get; set; }
        public List<Room> rooms { get; set; }
    }

    public class Search
    {
        public Dates dates { get; set; }
        public int propertyId { get; set; }
        public string nationality { get; set; }
        public List<Pax> pax { get; set; }
    }
    public class Dates
    {
        public string from { get; set; }
        public string to { get; set; }
    }

    public class ExpectedPrice
    {
        public double amount { get; set; }
        public string currency { get; set; }
    }

    public class Pax
    {
        public int adults { get; set; }
        public List<int> children { get; set; }
    }

    public class Room
    {
        public string roomCode { get; set; }
        //public int roomId { get; set; }
        public string rateCode { get; set; }
        //public int ratePlanId { get; set; }
        public ExpectedPrice expectedPrice { get; set; }
    }
}