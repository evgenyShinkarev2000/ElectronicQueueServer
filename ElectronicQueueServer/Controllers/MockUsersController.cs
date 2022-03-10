﻿using ElectronicQueueServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ElectronicQueueServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MockUsersController : Controller
    {
        private readonly AppDB appDB;

        public MockUsersController(AppDB appDB)
        {
            this.appDB = appDB;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IEnumerable<User> Get()
        {
            return appDB.GetAllUsers();
        }
    }
}
