using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace CallbackURL.Filters
{
    public class ApiSecurity
    {
        public static bool VaidateUser(string username, string password)
        {
            return username.Equals("pisoftware") && password.Equals("Pi*Soft74UBXi");//13-05-2024
        }
    }
}