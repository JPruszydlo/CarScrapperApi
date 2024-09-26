using CarScrapper.Models;
using HtmlAgilityPack;

namespace CarScrapper.Scrappers
{
    public class PorscheScrapper : Scrapper
    {
        public PorscheScrapper(string url) : base(url)
        { }

        public override List<CarScrappedDTO> GetParsedContent()
        {
            var doc = GetContent();
            var items = doc.DocumentNode.Descendants("a").Where(x => x.GetAttributeValue("data-testid", "").Contains("card-link-"))
                .ToList();

            return GetCars(items);
        }
        private List<CarScrappedDTO> GetCars(List<HtmlNode> nodes)
        {
            var result = new List<CarScrappedDTO>();
            foreach(var node in nodes)
            {
                var model = node.Descendants("p-model-signature").First().GetAttributeValue("id", "").Replace("-label", "").Trim();
                var image = "https://configurator.porsche.com" + node.Descendants("img").ElementAt(2).GetAttributeValue("src", "");
                var price = node.Descendants("p").Where(x => x.GetAttributeValue("id", "").Contains("price")).First().InnerText.ToUpper().Replace("OD", "").Replace("PLN", "").Replace(" ", "").Trim();
                var link = "https://configurator.porsche.com" + node.GetAttributeValue("href", "");
                result.Add(new CarScrappedDTO
                {
                    Make = "Porsche",
                    Model = model,
                    Image = image,
                    Price = Convert.ToDouble(price),
                    Link = link
                });
            }

            return result;
        }
    }
}
