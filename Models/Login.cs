﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BitshopWebApi.Models
{
    public class Login
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public string Token { get; set; }

    }
}