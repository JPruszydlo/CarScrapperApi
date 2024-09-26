using CarScrapper.Models;
using HtmlAgilityPack;

namespace CarScrapper.Scrappers
{
    public class HyundaiScrapper : Scrapper
    {
        public HyundaiScrapper(string url) : base(url)
        { }

        public override List<CarScrappedDTO> GetParsedContent()
        {
            var doc = GetContent();
            var items = doc.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("class", "").Equals("nav_cars-list-el")).Distinct().ToList();

            var cars = GetCars(items);
            return cars;
        }
        private List<CarScrappedDTO> GetCars(List<HtmlNode> nodes)
        {
            var result = new List<CarScrappedDTO>();
            var models = new List<string>();
            foreach(var node in nodes)
            {
                var model = node.Descendants("span").First(x => x.GetAttributeValue("class", "").Contains("nav_cars-list-el-heading")).InnerText;
                if (models.Contains(model))
                    continue;
                models.Add(model);
                var link = "https://www.hyundai.koreamotors.pl" + node.Descendants("a").First().GetAttributeValue("href", "");
                var image = "https://www.hyundai.koreamotors.pl" + node.Descendants("img").First().GetAttributeValue("src", "");
                var price = string.Join("", node.Descendants("span").First(x => x.GetAttributeValue("class", "").Contains("nav_cars-list-el-price")).InnerText.Where(Char.IsDigit).ToArray());

                result.Add(new CarScrappedDTO
                {
                    Make = "Hyundai",
                    Model = model,
                    Link = link,
                    Image = image,
                    Price = Convert.ToDouble(String.IsNullOrEmpty(price) ? "0" : price),
                });
            }

            return result;
        }
    }
}
