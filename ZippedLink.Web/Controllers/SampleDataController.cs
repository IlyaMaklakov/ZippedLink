using Microsoft.AspNetCore.Mvc;

namespace ZippedLink.WebSpa.Controllers
{
    [Route("api/[controller]")]
    public class SampleDataController : Controller
    {
        [HttpGet("[action]")]
        public bool ShortUrl(string url)
        {
            return true;
        }
    }
}
