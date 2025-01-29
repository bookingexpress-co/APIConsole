using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIConsole.Supplier.Models
{
    public enum ApiAction
    {
        //[Description("Searching")]
        Search = 1,
        RoomSearch = 2,
        //[Description("Cancellation Policy")]
        CXLPolicy = 3,
        //[Description("PreBooking")]
        PreBook = 4,
        //[Description("Booking")]
        Book = 5,
        //[Description("Cancellation")]
        Cancel = 6,
        BookDetail = 7,
        Hotels = 8,
        HotelInfo = 9,

    }
}
