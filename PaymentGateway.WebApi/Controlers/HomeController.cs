﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.WebApi.Controlers
{
    // http://localhost:5000/api/Home/GetHello
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController :ControllerBase
    {
        [HttpGet]
        [Route("GetHello")] 
        public string GetMessage()
        {
            return "Hello";
        }

        [HttpGet]
        [Route("Index")]
        public string Index()
        {
            return "Index";
        }
    }
}
