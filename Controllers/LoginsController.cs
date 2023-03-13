using BitshopWebApi;
using BitshopWebApi.Models;
using Catalog.Controllers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Mvc;

namespace Catalog.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]


    public class LoginsController : ApiController
    {
        private SqlConnection connection;

        public LoginsController()
        {
            this.connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        }
        public HttpResponseMessage GetAllUsers()
        {
            DataSet ds = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter("spGetAllUsers", this.connection);
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;

            this.connection.Open();

            adapter.Fill(ds);

            this.connection.Close();

            return Request.CreateResponse(HttpStatusCode.OK, ds);
        }
        [System.Web.Http.HttpPost]
        public IHttpActionResult CreateUser(Login login)
        {

            using (var connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                connection.Open();

                using (var command = new SqlCommand("spCreateUser", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@username", login.Username);
                    command.Parameters.AddWithValue("@email", login.Email);
                    command.Parameters.AddWithValue("@password", login.Password);
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }

            return Ok(login);
        }
        //server=FENDORIAN\\SQLEXPRESS; Integrated Security = True; database=Catalog;
        [System.Web.Http.HttpPost]
        public IHttpActionResult CheckLogin([FromBody] Login login)
        {
            //string connectionString = ConfigurationManager.AppSettings["ConnectionString"];

            using (var connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                connection.Open();

                SqlCommand command = new SqlCommand("spCheckValidation", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@username", login.Username);
                command.Parameters.AddWithValue("@password", login.Password);
                SqlParameter resultParameter = new SqlParameter("@p_result", System.Data.SqlDbType.Int);
                resultParameter.Direction = System.Data.ParameterDirection.Output;
                command.Parameters.Add(resultParameter);

                command.ExecuteNonQuery();

                int result = (int)resultParameter.Value;

                if (result == 1)
                    return Ok("Login Success");
                else
                    return BadRequest("Login Failed");
            }
        }
    }
    }
