using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace BitshopWebApi.Models
{
    public class DAL
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        SqlDataAdapter da = null;
        DataTable dt = new DataTable();

        public string GetLogin(Login login)
        {
            da = new SqlDataAdapter("SELECT * FROM Users WHERE Username = '"+login.Username +"' AND Password = '" + login.Password + "' ", con);
            dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)

                return "Logged in";
            else
                return "Login failed";
        }
    }
}