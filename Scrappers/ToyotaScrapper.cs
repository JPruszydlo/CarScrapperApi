using CarScrapper.Models;
using HtmlAgilityPack;

namespace CarScrapper.Scrappers
{
    public class ToyotaScrapper : Scrapper
    {
        public ToyotaScrapper(string url) : base(url)
        { }

        public override List<CarScrappedDTO> GetParsedContent()
        {
            var doc = base.GetContent();
            var items = doc.DocumentNode.Descendants("a")
                .Where(node => node.GetAttributeValue("class", "").Split(new char[0])[0].Equals("cmp-mega-menu__card"))
                .ToList();

            var result = GetCars(items);
            return result;
        }

        private List<CarScrappedDTO> GetCars(List<HtmlNode> nodes)
        {
            var result = new List<CarScrappedDTO>();
            var tmp = new List<string>();
            foreach(var node in nodes)
            {
                var link = "https://www.toyota.pl" + node.GetAttributeValue("href", "");
                var model = node.Descendants("div")
                    .Where(node => node.GetAttributeValue("class", "").Contains("cmp-mega-menu__card-header"))
                    .First()
                    .ChildNodes[1].InnerText;
                var price = node.Descendants("div").First(node => node.GetAttributeValue("class", "").Contains("mp-mega-menu__card-details")).InnerText.ToUpper()
                    .Replace("OD", "").Replace("ZŁ", "").Replace("\n", "").Replace("NETTO", "").Replace("BRUTTO", "").Replace(" ", "").Trim();
                var img = node.Descendants("img").First().GetAttributeValue("data-src", "");
                result.Add(new CarScrappedDTO
                {
                    Make = "Toyota",
                    Model = model,
                    Link = link,
                    Price = Convert.ToDouble(string.IsNullOrEmpty(price) ? "0" : price),
                    Image = img
                });
            }

            return result;
        }
    }
}
