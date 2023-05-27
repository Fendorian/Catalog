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

                using (var checkCommand = new SqlCommand("spCheckUsername", connection))
                {
                    checkCommand.CommandType = CommandType.StoredProcedure;
                    checkCommand.Parameters.AddWithValue("@username", login.Username);

                    int count = Convert.ToInt32(checkCommand.ExecuteScalar());

                    if (count > 0)
                    {
                        // Username already exists
                        return BadRequest("Username already exists");
                    }
                }

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
        private static string GenerateToken()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var token = new string(
                Enumerable.Repeat(chars, 32)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
            return token;
        }
        //server=FENDORIAN\\SQLEXPRESS; Integrated Security = True; database=Catalog;
        [System.Web.Http.HttpPost]
        public IHttpActionResult Login(Login login)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                using (SqlCommand command = new SqlCommand("spCheckUser2", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@username", login.Username);
                    command.Parameters.AddWithValue("@password", login.Password);

                    connection.Open();

                    int authenticated = (int)command.ExecuteScalar();

                    if (authenticated == 1)
                    {
                        // User is authenticated

                        // Generate a new token
                        string token = GenerateToken();

                        // Update the User table with the token
                        using (SqlCommand updateCommand = new SqlCommand("UPDATE [User] SET token = @token WHERE username = @username", connection))
                        {
                            updateCommand.Parameters.AddWithValue("@token", token);
                            updateCommand.Parameters.AddWithValue("@username", login.Username);
                            updateCommand.ExecuteNonQuery();
                        }

                        // Return the token to the client
                        return Ok(new { token = token });
                    }
                    else
                    {
                        // User is not authenticated
                        return Unauthorized();
                    }
                }
            }
        }
        [System.Web.Http.HttpPost]
        public IHttpActionResult CheckToken(Login login)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                using (SqlCommand command = new SqlCommand("spCheckUserToken", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@username", login.Username);
                    command.Parameters.AddWithValue("@token", login.Token);

                    connection.Open();

                    int authenticated = (int)command.ExecuteScalar();

                    if (authenticated == 1)
                    {
                        // Token is valid
                        return Ok();
                    }
                    else
                    {
                        // Token is not valid
                        return Unauthorized();
                    }
                }
            }
        }





    }
}
