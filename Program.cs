using APIConsole.Helpers;
using System;
using System.IO;

namespace APIConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string trackNo = "a2b4ac4a-6274-4ef2-aab1-6c36a2d5f76d";
            int suplId = 21;

            TravayooBO obj = new TravayooBO();
            obj.HotelSearch(trackNo, suplId);
            obj.RoomSearch(trackNo, suplId);
            obj.Prebook(trackNo, suplId);
            obj.Book(trackNo, suplId);



        }








    }
}
