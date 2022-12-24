using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BitshopWebApi.Models
{
    public class Item
    {
        public int ItemID { get; set; }
        public string Name { get; set; }
        public string Abstract { get; set; }
        public string Desc { get; set; }
        public int Price { get; set; }
        public string CategoryID { get; set; }
        public string ImageUrl { get; set; }
    }
}