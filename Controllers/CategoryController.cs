using BitshopWebApi.Models;
using Catalog.Models;
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

namespace BitshopWebApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CategoryController : ApiController
    {

        private SqlConnection connection;
        public CategoryController()
        {
            this.connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        }
        public HttpResponseMessage GetAllCategories()
        {
            DataSet ds = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter("spGetAllCategories", this.connection);
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;

            this.connection.Open();

            adapter.Fill(ds);

            this.connection.Close();

            return Request.CreateResponse(HttpStatusCode.OK, ds);
        }


        public HttpResponseMessage GetCategoryById(int id)
        {
            DataSet ds = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter("spGetCategoryById", this.connection);
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.SelectCommand.Parameters.Add("@CategoryID", SqlDbType.Int);
            adapter.SelectCommand.Parameters[0].Value = id;

            this.connection.Open();

            adapter.Fill(ds);

            this.connection.Close();


            return Request.CreateResponse(HttpStatusCode.OK, ds);
        }
        [System.Web.Http.HttpPost]
        public IHttpActionResult CreateCategory(Category category)
        {
            using (var connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                using (var command = new SqlCommand("spCreateCategory", connection))
                {
                    connection.Open();

                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("@CategoryName", SqlDbType.NVarChar, 50).Value = category.CategoryName;
                    


                    command.ExecuteNonQuery();

                }
                connection.Close();
            }
            return Ok(category);
        }



    }
}
