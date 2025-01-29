using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIConsole.Supplier.Models.HYGST
{
    public class HyGSTBookReqModel
    {
        public Dates dates { get; set; }
        public int propertyId { get; set; }
        public LeadGuest leadGuest { get; set; }
        public Reference reference { get; set; }
        public PaymentDetails paymentDetails { get; set; }
        public List<RoomsItem> rooms { get; set; }
        public List<MetaItem> meta { get; set; }
        public string isTest { get; set; }
        public string groupBooking { get; set; }
    }

    public class Contact
    {
        public string address { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
    }

    public class Name
    {
        public string first { get; set; }
        public string last { get; set; }
    }

    public class LeadGuest
    {
        public string birthDate { get; set; }
        public Contact contact { get; set; }
        public Name name { get; set; }
        public string title { get; set; }
    }

    
    public class Reference
    {
        public string agency { get; set; }
    }
    public class Expiry
    {
        public string month { get; set; }
        public string year { get; set; }
    }
    public class Details
    {
        public string number { get; set; }
        public string cvv { get; set; }
        public Expiry expiry { get; set; }
        public Name name { get; set; }
    }

    public class PaymentDetails
    {
        public string type { get; set; }
        public Details details { get; set; }
    }
    public class GuestsItem
    {
        public string birthDate { get; set; }
        public Contact contact { get; set; }
        public Name name { get; set; }
        public string title { get; set; }
    }

    public class RoomsItem
    {
        public string roomCode { get; set; }
        public string rateCode { get; set; }
        public ExpectedPrice expectedPrice { get; set; }
        public List<GuestsItem> guests { get; set; }
        public List<string> specialRequests { get; set; }
    }

    public class MetaItem
    {
        public string key { get; set; }
        public string value { get; set; }
    }

}