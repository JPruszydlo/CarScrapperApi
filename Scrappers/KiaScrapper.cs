using CarScrapper.Models;
using HtmlAgilityPack;
using System.Net;

namespace CarScrapper.Scrappers
{
    public class KiaScrapper : Scrapper
    {
        public KiaScrapper(string url) : base(url)
        { }

        public override List<CarScrappedDTO> GetParsedContent()
        {
            var content = GetStaticPageContent();
            content.Wait();
            var doc = new HtmlDocument();
            doc.LoadHtml(content.Result);

            var items = doc.DocumentNode.Descendants("li")
                .Where(node => node.GetAttributeValue("class", "").Contains("category_item"))
                .ToList();
            items.RemoveRange(items.Count - 2, 2);

            var result = GetCar(items);
            return result;
        }

        private List<CarScrappedDTO> GetCar(List<HtmlNode> nodes)
        {
            var result = new List<CarScrappedDTO>();
            foreach(var node in nodes)
            {
                var price = Convert.ToDouble(node.Descendants("span").First(x => x.GetAttributeValue("class", "").Contains("price")).InnerText.Replace("\n", "").Replace("Od", "").Replace("zł", "").Replace(" ", "").Trim());
                var img = node.Descendants("img").First().GetAttributeValue("data-srcset", "");
                var link = node.Descendants("a").First().GetAttributeValue("href", "");
                var model = node.Descendants("strong").First().InnerText;
                result.Add(new CarScrappedDTO
                {
                    Price = price,
                    Make = "KIA",
                    Model = model,
                    Link = link,
                    Image = img.Contains("prod2admin") ? "" : "https://www.kia.com" + img
                });
            }

            return result;
        }

        public override async Task<string> GetStaticPageContent(string? url = null)
        {

            using (var handler = new HttpClientHandler())
            {
                handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                using (var client = new HttpClient(handler))
                {
                    var response = await client.GetAsync(url ?? base._url);
                    string content = await response.Content.ReadAsStringAsync();
                    return content;
                }
            }
        }
    }
}
