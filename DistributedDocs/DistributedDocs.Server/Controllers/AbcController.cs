using Microsoft.AspNetCore.Mvc;

namespace DistributedDocs.Server.Controllers
{
    [ApiController]
    [Route("abc")]
    public class AbcController : ControllerBase
    {
        [HttpGet("get")]
        public int Get()
        {
            return 1;
        }
    }
}