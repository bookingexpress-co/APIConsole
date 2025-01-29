using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIConsole.Supplier.Models.Common
{
    public abstract class AuthenticationModel
    {
        public string BaseUrl
        {
            get;
            set;
        }
        public string ClientId
        {
            get;
            set;
        }
        public string SecretKey
        {
            get;
            set;
        }
        public string CustomerId
        {
            get;
            set;
        }
        public int SupplierId
        {
            get;
            set;
        }
        public string Currency
        {
            get;
            set;
        }
        public string Culture
        {
            get;
            set;
        }
        public string Service
        {
            get;
            set;
        }
    

    }
}
