using APIConsole.Supplier.Models.Common;
using System.Collections.Generic;


namespace APIConsole.Supplier.Models.HYGST
{
    public class RoomPrice
    {
        public int roomNo { get; set; }
        public string roomCode { get; set; }
        public int roomId { get; set; }
        public string rateCode { get; set; }
        public int ratePlanId { get; set; }
        public decimal amount { get; set; }
        public string currency { get; set; }
        public int adults { get; set; }
        public int ChildAgeSum { get; set; }
        public string roomKey { get; set; }
        public string mealPlan { get; set; }
        public List<RoomPolicy> cancelPolicies { get; set; }
        public List<double> nightPrice { get; set; }

    }




    }