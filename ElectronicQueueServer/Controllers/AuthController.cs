using ElectronicQueueServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ElectronicQueueServer.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private AppDB appDB;
        public AuthController(AppDB appDB)
        {
            this.appDB = appDB;
        }

        [HttpPost, Route("login")]
        public IActionResult login([FromBody] LoginModel loginModel)
        {
            var user = appDB.GetUserByLoginModel(loginModel);
            if ( user != null)
            {
                var tokenOptions = new JwtConfigurator().GetJwtSecurityToken(user.Role);
                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
                return Ok(new { Token = tokenString, id =  user.Id.ToString()});
            }

            return Unauthorized();
        }

        [HttpPost, Route("signup")]
        public IActionResult Post([FromBody] User user)
        {
            try
            {
                user.Role = UserRole.Client;
                appDB.AddUser(user);
                return Ok();
            }
            catch(Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
