using System.Configuration;

namespace APIConsole.Supplier.DataAccess
{ 
    public static class TravayooConnection
    {
        static string _dbConnectionString = string.Empty;
        static TravayooConnection()
        {
            //
            // TODO: Add constructor logic here
            //
            _dbConnectionString = ConfigurationManager.ConnectionStrings["INGMContext"].ToString();
        }
        public static string ServiceConnection
        {
            get { return _dbConnectionString; }

        }
    }
}