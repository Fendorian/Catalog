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
        public HttpResponseMessage GetPagedProducts(int pageNumber, int pageSize)
        {
            DataSet ds = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter("spPagination", this.connection);
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.SelectCommand.Parameters.Add("@pageNumber", SqlDbType.Int);
            adapter.SelectCommand.Parameters[0].Value = pageNumber;
            adapter.SelectCommand.Parameters.Add("@pageSize", SqlDbType.Int);
            adapter.SelectCommand.Parameters[1].Value = pageSize;

            this.connection.Open();

            adapter.Fill(ds);

            this.connection.Close();

                return Request.CreateResponse(HttpStatusCode.OK, ds);
        }
        [HttpGet]
        public IHttpActionResult GetItemsByCategory(int id)
        {
            DataSet ds = new DataSet();
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                SqlCommand command = new SqlCommand("spGetItemsByCategory", this.connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CategoryID", SqlDbType.Int);
                command.Parameters[0].Value = id;

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable table = new DataTable();
                adapter.Fill(table);

                return Ok(table);
            }

            
        }

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
        public HttpResponseMessage GetItemByCategory(int categoryId)
        {
            DataSet ds = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter("spGetItemsByCategory", this.connection);
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.SelectCommand.Parameters.Add("@CategoryID", SqlDbType.Int);
            adapter.SelectCommand.Parameters[0].Value = categoryId;

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
                using (var command = new SqlCommand("spCreateItem2", connection))
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
        [HttpPost]
        public IHttpActionResult CreateItem2(Item item)
        {
            using (var connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                using (var command = new SqlCommand("spCreateItem", connection))
                {
                    connection.Open();

                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("@Name", SqlDbType.NVarChar, 50).Value = item.Name;
                    command.Parameters.Add("@Abstract", SqlDbType.NVarChar, 255).Value = item.Abstract;
                    command.Parameters.Add("@Desc", SqlDbType.NVarChar, -1).Value = item.Desc; // Use -1 for nvarchar(MAX)
                    command.Parameters.Add("@Price", SqlDbType.Int).Value = item.Price;
                    command.Parameters.Add("@ImageUrl", SqlDbType.NVarChar, 255).Value = item.ImageUrl;
                    command.Parameters.Add("@CategoryID", SqlDbType.Int).Value = item.CategoryID;

                    // Prepare the Specs table
                    var specsTable = new DataTable();
                    specsTable.Columns.Add("SpecID", typeof(int));
                    specsTable.Columns.Add("Value", typeof(string));
                    foreach (var spec in item.Specs)
                    {
                        specsTable.Rows.Add(spec.SpecID, spec.Value);
                    }

                    // Add the Specs table as a parameter
                    var specsParameter = command.Parameters.AddWithValue("@Specs", specsTable);
                    specsParameter.SqlDbType = SqlDbType.Structured;
                    specsParameter.TypeName = "dbo.ItemSpecs";

                    command.ExecuteNonQuery();

                }
                connection.Close();
            }
            return Ok(item);
        }



        [HttpGet]
        public IHttpActionResult GetTotalItemsCount()
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                SqlCommand command = new SqlCommand("spCountItems", connection);
                command.CommandType = CommandType.StoredProcedure;

                connection.Open();
                int totalItemsCount = (int)command.ExecuteScalar();
                connection.Close();

                return Ok(totalItemsCount);
            }
        }

        [HttpGet]
        [Route("api/Products/GetSpecForCategory/{categoryId}")]
        public IHttpActionResult GetSpecForCategory(int categoryId)
        {
            List<SpecForCategory> specs = new List<SpecForCategory>();

            using (var connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                using (var command = new SqlCommand("spGetSpecialCategoriesByCategory", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Set CategoryID parameter
                    command.Parameters.Add("@CategoryID", SqlDbType.Int).Value = categoryId;

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            specs.Add(new SpecForCategory()
                            {
                                CategoryName = reader["CategoryName"].ToString(),
                                SpecCatID = Convert.ToInt32(reader["SpecCatID"]),
                                SpecialCategoryName = reader["SpecialCategoryName"].ToString()
                            });

                        }
                    }
                }
            }
            connection.Close();
            if (specs.Count > 0)
                
                return Ok(specs);

            return NotFound();
        }

    }
}
