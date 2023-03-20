using BitshopWebApi.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace BitshopWebApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ProductsController : ApiController
    {

        private SqlConnection connection;

        public ProductsController()
        {
            this.connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        }

        public HttpResponseMessage GetAllItems()
        {
            DataSet ds = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter("spGetAllItems", this.connection);
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;

            this.connection.Open();

            adapter.Fill(ds);

            this.connection.Close();

            return Request.CreateResponse(HttpStatusCode.OK, ds);
        }

        [HttpGet]
        public IHttpActionResult GetPagedProducts(int pageNumber, int pageSize)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                SqlCommand command = new SqlCommand("spPagination", this.connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@pageNumber", pageNumber);
                command.Parameters.AddWithValue("@pageSize", pageSize);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable table = new DataTable();
                adapter.Fill(table);

                return Ok(table);
            }
        }

        public HttpResponseMessage GetItemsByCategory(int id)
        {
            DataSet ds = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter("spGetItemsByCategory", this.connection);
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.SelectCommand.Parameters.Add("@CategoryID", SqlDbType.Int);
            adapter.SelectCommand.Parameters[0].Value = id;

            this.connection.Open();

            adapter.Fill(ds);

            this.connection.Close();


            return Request.CreateResponse(HttpStatusCode.OK, ds);
        }
        [HttpDelete]
        public HttpResponseMessage DeleteItem(int id)
        {
            using (var connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                // Create the SQL command object
                SqlCommand command = new SqlCommand("spDeleteItem", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Add the parameter to the command
                command.Parameters.Add("@ItemID", SqlDbType.Int).Value = id;

                // Open the connection
                connection.Open();

                // Execute the command
                int rowsAffected = command.ExecuteNonQuery();

                // Check if any rows were deleted
                if (rowsAffected == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                // Return a success response
                return Request.CreateResponse(HttpStatusCode.OK);
            }
        }


        [HttpGet]
        public HttpResponseMessage GetItemById(int id)
        {
            DataSet ds = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter("spGetItemById", this.connection);
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.SelectCommand.Parameters.Add("@ItemID", SqlDbType.Int);
            adapter.SelectCommand.Parameters[0].Value = id;

            this.connection.Open();

            adapter.Fill(ds);

            this.connection.Close();


            return Request.CreateResponse(HttpStatusCode.OK, ds);
        }

        [HttpPost]
        public IHttpActionResult CreateItem(Item item)
        {
            using (var connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                using (var command = new SqlCommand("spCreateItem", connection))
                {
                    connection.Open();

                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("@Name", SqlDbType.NVarChar, 50).Value = item.Name;
                    command.Parameters.Add("@Abstract", SqlDbType.NVarChar, 255).Value = item.Abstract;
                    command.Parameters.Add("@Desc", SqlDbType.NVarChar, 255).Value = item.Desc;
                    command.Parameters.Add("@Price", SqlDbType.Int).Value = item.Price;
                    command.Parameters.Add("@ImageUrl", SqlDbType.NVarChar, 255).Value = item.ImageUrl;
                    command.Parameters.Add("@CategoryID", SqlDbType.Int).Value = item.CategoryID;

                   
                    command.ExecuteNonQuery();
                    
                }
                connection.Close();
            }
            return Ok(item);
        }
    }
    }
