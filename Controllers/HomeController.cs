using CarScrapper.Services.Interfaces;
using CarScrapper.Utils;
using Microsoft.AspNetCore.Mvc;

namespace CarScrapper.Controllers
{
    [Route("[controller]")]
    public class HomeController : Controller
    {
        private readonly IScrapperService _scrapperService;
        public HomeController(IScrapperService scrapperService)
        {
            _scrapperService = scrapperService;
        }

        [HttpGet, ApiProtected]
        public IActionResult GetScrapped()
            => Ok(_scrapperService.GetCarsScrapped());

        [HttpPut("{id?}"), ApiProtected]
        public IActionResult UpdateScrapped([FromRoute] int? id)
        {
            if (_scrapperService.Scrap(id))
                return Ok(_scrapperService.GetCarsScrapped());
            return BadRequest();
        }

        [HttpPut("sync"), ApiProtected]
        public IActionResult SyncData()
            => Ok(_scrapperService.SyncScrapped());

        [HttpPost("scrap"), ApiProtected]
        public IActionResult ScrapPages()
        {
            if(_scrapperService.Scrap(null))
                return Ok();
            return BadRequest();
        }
        
    }
}
