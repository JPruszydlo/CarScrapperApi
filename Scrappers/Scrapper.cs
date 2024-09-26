using CarScrapper.Models;
using HtmlAgilityPack;
using System.Net;

namespace CarScrapper.Scrappers
{
    public abstract class Scrapper
    {
        protected string brandName = "";
        protected readonly string _url;
        public Scrapper(string url)
        {
            _url = url;
        }

        public virtual HtmlDocument GetContent(string? url = null)
            => new HtmlWeb().Load(url ?? _url);

        public virtual async Task<string> GetStaticPageContent(string? url = null)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetStringAsync(url ?? _url);
                return response;
            }
        }
        public abstract List<CarScrappedDTO> GetParsedContent();
    }
}
