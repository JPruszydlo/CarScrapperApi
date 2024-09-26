using CarScrapper.Models;
using HtmlAgilityPack;

namespace CarScrapper.Scrappers
{
    public class OpelScrapper : Scrapper
    {
        public OpelScrapper(string url) : base(url)
        {
            brandName = "Opel";
        }

        public override List<CarScrappedDTO> GetParsedContent()
        {
            var doc = GetContent();
            var items = doc.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("class", "").Contains("row"))
                .ElementAt(1).Descendants("div").Where(n => n.GetAttributeValue("class", "").Contains("box")).ToList();

            var result = GetCars(items);
            return result;
        }

        private List<CarScrappedDTO> GetCars(List<HtmlNode> nodes)
        {
            var result = new List<CarScrappedDTO>();

            foreach(var node in nodes)
            {
                var linkElement = node.Descendants("a").Where(node => node.InnerText != "").First();
                var link = "https://mwauto.com.pl" + linkElement.GetAttributeValue("href", "");
                var model = GetModelName(linkElement);
                var image = "https://mwauto.com.pl" + node.Descendants("img").First().GetAttributeValue("data-src", "");
                var price = string.Join("", node.DescendantsAndSelf("p").ElementAt(2).Descendants("span").First().InnerText.Where(Char.IsDigit));

                result.Add(new CarScrappedDTO
                {
                    Make = brandName,
                    Model = model,
                    Link = link,
                    Image = image,
                    Price = Convert.ToDouble(price),
                });
            }

            return result;
        }

        protected virtual string GetModelName(HtmlNode linkElement)
            => linkElement.InnerText.ToUpper().Replace(brandName.ToUpper(), "").Trim();
    }
}
