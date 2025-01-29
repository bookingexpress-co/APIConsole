using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace APIConsole.Supplier.Models.Common
{
    public class RhinePolicyModel
    {
        public DateTime LastCancelDate { get; set; }
        public bool IsRefundable { get; set; }
        public double Amount { get; set; }
        public XElement policy { get; set; }
    }
    public class RhineCancelPolicy
    {
        public int Index { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public byte NoShow { get; set; }
        public double Amount { get; set; }
    }
}