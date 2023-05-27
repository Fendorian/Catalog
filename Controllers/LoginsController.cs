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
using System.Net.Mail;
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

        [System.Web.Http.HttpPost]
        public IHttpActionResult ForgotPassword(Login login)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                using (SqlCommand command = new SqlCommand("spCheckUserEmail", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Email", login.Email);

                    connection.Open();

                    int exists = (int)command.ExecuteScalar();

                    if (exists == 1)
                    {
                        // Email exists in the user table, proceed with forgot password process.

                        // Generate a unique token.
                        var token = Guid.NewGuid().ToString();

                        // Save the token in your database tied to the user. 
                        SaveResetToken(login, token);

                        // Send an email to the user with a URL that includes the unique token.
                        // This is outside the scope of this function and depends on your SMTP configuration.

                        return Ok();
                    }
                    else
                    {
                        // Email does not exist in the user table.
                        return NotFound();
                    }
                }
            }
        }


        public IHttpActionResult SaveResetToken(Login login, string resetToken)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                using (SqlCommand command = new SqlCommand("spSaveUserResetToken", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Email", login.Email);
                    command.Parameters.AddWithValue("@ResetToken", resetToken);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }

            // Consider what you want to return here. It could be an Ok() status,
            // or you might want to return a more specific message or status.
            return Ok();
        }

        private void SendResetEmail(string email, string resetToken)
        {
            // The email address that this email will be sent from
            var fromAddress = new MailAddress("your-email@example.com", "Your Name");

            // The email address that this email will be sent to
            var toAddress = new MailAddress(email);

            // The password for the email address that this email will be sent from
            const string fromPassword = "your-email-password";

            // The subject of the email
            const string subject = "Password Reset Request";

            // The body of the email
            string body = $"Please reset your password by clicking here: https://your-app-url.com/reset-password?token={resetToken}";

            // SMTP server configuration
            var smtp = new SmtpClient
            {
                Host = "smtp.example.com", // replace with your SMTP server
                Port = 587, // replace with your SMTP port
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            // Create the email
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })

                // Send the email
                smtp.Send(message);
        }





    }
}
