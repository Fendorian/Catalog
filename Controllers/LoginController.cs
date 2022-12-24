using BitshopWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.UI.WebControls;
using Login = BitshopWebApi.Models.Login;

namespace BitshopWebApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class LoginController : ApiController
    {
        [HttpPost]
        [Route("api/Login/Getlogin")]
        public string GetLogin(Login login)
        {
            BAL bal = new BAL();
            string response = bal.GetLogin(login);
            return response;
        }
    }
}
