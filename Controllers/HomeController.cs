using CarScrapper.Models;
using CarScrapper.Services.Interfaces;
using CarScrapper.Utils;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using static CarScrapper.Services.ScrapperService;

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
        {
            try
            {
                return Ok(_scrapperService.GetCarsScrapped());
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPut("{id?}"), ApiProtected]
        public IActionResult UpdateScrapped([FromRoute] int? id)
        {
            try
            {
                var result = false;
                if (id == null) 
                    result = _scrapperService.Scrap();
                
                var car = (CarMake)id;
                result = _scrapperService.Scrap(car);

                if (result)
                    return Ok(_scrapperService.GetCarsScrapped());
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPut("sync"), ApiProtected]
        public IActionResult SyncData()
        {
            try
            {
                var result = _scrapperService.SyncScrapped();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost("scrap"), ApiProtected]
        public IActionResult ScrapPages()
        {
            if(_scrapperService.Scrap())
                return Ok();
            return BadRequest();
        }
        
    }
}
