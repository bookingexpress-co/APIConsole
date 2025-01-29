using APIConsole.Supplier.Models.Common;

namespace APIConsole.Supplier.Models.HYGST
{
    public class HYGSTCredentials : AuthenticationModel
    {
        public string Token { get; set; }
        public string IpAddress { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string CustomerName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public HYGSTCredentials()
        {
            this.BaseUrl = "https://api.tbotechnology.in/TBOHolidays_HotelAPI/";
            this.ClientId = "Rhinetravel";
            this.SecretKey = "Rhi@63663603";
            this.CustomerId = "111025";
            this.SupplierId = 21;
            this.Currency = "USD";
            this.Culture = "en";
            this.Email = "s.hijazin@bookingexpress.co";
            this.CustomerName = "Saleem Hijazin";
            this.FirstName = "Saleem";
            this.LastName = "Hijazin";
            this.Phone = "+962795674029";
        }
    }
}
