using APIConsole.Supplier.Models;
using APIConsole.Supplier.Models.HYGST;
using System.Xml.Linq;

namespace APIConsole.Supplier.Helpers
{
    public static class HYGSTHelper
    {
        public static HYGSTCredentials ReadCredential(this string custId, int supplId, short callType)
        {
            HYGSTCredentials _credential = null;
            try
            {
                if (!string.IsNullOrEmpty(custId))
                {
                    XElement data = supplier_Cred.getsupplier_credentials(custId, supplId.ToString());
                    _credential = new HYGSTCredentials
                    {
                        CustomerId = data.Attribute("customerid").Value,
                        SupplierId = data.Attribute("supplierid").GetValueOrDefault(0),
                        Token = data.Element("token").Value,
                        Currency = data.Element("currency").Value,
                        Culture = data.Element("culture").Value,
                        Email = data.Element("email").Value,
                        CustomerName = data.Element("customerName").Value,
                        FirstName = data.Element("firstName").Value,
                        LastName = data.Element("lastName").Value,
                        Phone = data.Element("phone").Value,
                        IpAddress = data.Element("ipAddress").Value,
                    };

                    if (callType == (short)ApiAction.Search)
                    {_credential.BaseUrl = data.Element("search").Value;
                    }
                    else if (callType == (short)ApiAction.RoomSearch)
                    {
                        _credential.BaseUrl = data.Element("search").Value;
                    }
                    else if (callType == (short)ApiAction.PreBook)
                    {
                        _credential.BaseUrl = data.Element("prebook").Value;
                    }
                    else if (callType == (short)ApiAction.Book)
                    {
                        _credential.BaseUrl = data.Element("book").Value;
                    }
                    else if (callType == (short)ApiAction.Cancel)
                    {
                        _credential.BaseUrl = data.Element("cancel").Value;
                    }
                    else if (callType == (short)ApiAction.BookDetail)
                    {
                        _credential.BaseUrl = data.Element("book").Value;
                    }
                }
                return _credential;
            }
            catch
            {
                return null;
            }
        }

    }
}