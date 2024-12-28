using KuaförRandevuSistemi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KuaförRandevuSistemi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceApiController : ControllerBase
    {
        private SalonDbContext db = new SalonDbContext();

        // GET: api/ServiceApi
        [HttpGet]
        public IActionResult GetServices()
        {
            var services = db.Services.ToList();
            return Ok(services);
        }

    }
}
