using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIConsole.Supplier.Models.Common
{
    public class HotelSearchModel
    {
        public long CustomerId { get; set; }
        public int SupplierId { get; set; }
        public string TrackNo { get; set; }
        public string HotelId { get; set; }
        public string HotelName { get; set; }
        public int MinRating { get; set; }
        public int MaxRating { get; set; }
        public string CityCode { get; set; }
        public string CountryCode { get; set; }
        public string Nationality { get; set; }
        public string CheckIn { get; set; }
        public string CheckOut { get; set; }
        public int Nights { get; set; }
        public List<RoomPax> RoomList { get; set; }
        public bool XmlOut { get; set; }
        public int BufferDays { get; set; }

    }
    public class RoomPax
    {
        public int Index { get; set; }
        public Occupancy Guests { get; set; }
        public string RoomKey { get { return (this.Guests.Adults + "$" + string.Join("-", this.Guests.Children)); } }



        //roomKey = (string) room.SelectToken("searchedPax.adults") + "$" + string.Join("-", childAgeArray)
    }

    public class Occupancy
    {
        public int Adults { get; set; }
        public List<int> Children { get; set; }
    }

    public class RoomPolicy
    {
        public int Index { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public byte NoShow { get; set; }
        public double Amount { get; set; }
    }
}