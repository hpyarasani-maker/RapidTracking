using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CallbackURL.Filters
{
    public class ApiSecurity
    {
        public static bool VaidateUser(string username, string password)
        {
            
            if (true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        internal bool Authenticate(string name, string password)
        {
            var isValid = name == "pisoftware" && password == "Pi*Soft74UBXi";
            if (isValid)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}