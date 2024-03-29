﻿using System;
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
        public int CategoryID { get; set; }
        public string ImageUrl { get; set; }

        public List<Spec> Specs { get; set; }
    }
    public class Spec
    {
        public int SpecID { get; set; }
        public string Value { get; set; }
    }
    public class SpecForCategory
    {
        public int SpecCatID { get; set;}

        public string CategoryName { get; set; }

        public string SpecialCategoryName { get; set; }

    }



}