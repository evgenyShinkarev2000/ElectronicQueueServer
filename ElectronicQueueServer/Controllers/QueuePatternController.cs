using ElectronicQueueServer.Models;
using ElectronicQueueServer.Models.DB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElectronicQueueServer.Controllers
{
    [Route("api/QueuePattern")]
    [Authorize(Roles = "ADMIN, OPERATOR")]
    public class QueuePatternController : Controller
    {
        private readonly AppDB _appDB;
        public QueuePatternController(AppDB appDB)
        {
            this._appDB = appDB;
        }
        [HttpGet, Route("dayPatterns")]
        public async Task<IActionResult> getAllDayPatterns()
        {
            try
            {
                return Ok(await this._appDB.GetAllDayPatterns());
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost, Route("dayPattern")]
        public async Task<IActionResult> addDayPattern([FromBody] EQDayPattern dayPattern)
        {
            try
            {
                await this._appDB.AddDayPattern(dayPattern);
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpDelete, Route("dayPattern")]
        public async Task<IActionResult> DeleteDayPattern([FromQuery] string name)
        {
            try
            {
                await this._appDB.DeleteDayPattern(name);
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPut, Route("dayPattern")]
        public async Task<IActionResult> UpdateDayPattern([FromBody] EQDayPattern dayPattern)
        {
            try
            {
                await this._appDB.UpdateDayPattern(dayPattern);
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
