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
        // GET api/values
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
        public HttpResponseMessage GetAllCategories()
        {
            DataSet ds = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter("spGetAllItems", this.connection);
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;

            this.connection.Open();

            adapter.Fill(ds);

            this.connection.Close();

            return Request.CreateResponse(HttpStatusCode.OK, ds);
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

        


        public HttpResponseMessage GetItemById(int id)
        {
            DataSet ds = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter("spGetItem", this.connection);
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.SelectCommand.Parameters.Add("@ItemID", SqlDbType.Int);
            adapter.SelectCommand.Parameters[0].Value = id;

            this.connection.Open();

            adapter.Fill(ds);

            this.connection.Close();


            return Request.CreateResponse(HttpStatusCode.OK, ds);
        }

        // POST api/values
        //public void Post([FromBody] string value)
        //{
        //}
        [HttpPost]
        [Route("api/Product/CreateItem")]
        public IHttpActionResult CreateItem(Item item)
        {
            using (var connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                

                using (var command = new SqlCommand("spPostItem", this.connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Add the parameters for the stored procedure
                    command.Parameters.Add("@ID", SqlDbType.Int).Value = item.ItemID;
                    command.Parameters.Add("@Name", SqlDbType.NVarChar, 50).Value = item.Name;
                    command.Parameters.Add("@Abstract", SqlDbType.NVarChar, 100).Value = item.Abstract;
                    command.Parameters.Add("@Desc", SqlDbType.NVarChar, 100).Value = item.Desc;
                    command.Parameters.Add("@Price", SqlDbType.Int).Value = item.Price;
                    command.Parameters.Add("@CategoryId", SqlDbType.Char, 10).Value = item.CategoryID;
                    command.Parameters.Add("@ImageUrl", SqlDbType.NVarChar, 100).Value = item.ImageUrl;

                    // Execute the stored procedure
                    this.connection.Open();
                    command.ExecuteNonQuery();
                    this.connection.Close();
                }
            }

            // Return a success status code
            return Ok();
            
        }

        // PUT api/values/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
