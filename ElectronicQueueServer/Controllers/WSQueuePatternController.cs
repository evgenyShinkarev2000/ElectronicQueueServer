using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectronicQueueServer.Controllers
{
    [Route("api/QueuePattern")]
    [Authorize(Roles = "ADMIN OPERATOR")]
    public class WSQueuePatternController : Controller
    {
      
    }
}
