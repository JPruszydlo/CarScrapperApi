using CarScrapper.Models;
using HtmlAgilityPack;

namespace CarScrapper.Scrappers
{
    public class RenaultScrapper : Scrapper
    {
        public RenaultScrapper(string url) : base(url)
        { }

        public override List<CarScrappedDTO> GetParsedContent()
        {
            var doc = base.GetContent();

            var items = doc.DocumentNode.Descendants("div").Where(node => 
                node.GetAttributeValue("class", "").Contains("RangeModelsPicker__model") &&
                !node.GetAttributeValue("class", "").Contains("RangeModelsPicker__modelsList")
            ).ToList();

            var names = GetNames(items);
            var prices = GetPrices(items);
            var links = GetLinks(items);
            var images = GetImages(items);

            var result = new List<CarScrappedDTO>();
            for(var i=0; i<names.Count; i++)
            {
                result.Add(new CarScrappedDTO
                {
                    Make = "Renault",
                    Model = names[i],
                    Price = prices[i],
                    Link = links[i],
                    Image = images[i]
                });
            }
            return result;
        }

        private List<string> GetImages(List<HtmlNode> nodes)
            => nodes.Select(node => "https://www.renault.pl" + node.Descendants("noscript").First().ChildNodes[0].GetAttributeValue("src", ""))
                  .ToList();

        private List<string> GetLinks(List<HtmlNode> nodes)
            => nodes.Select(node => node.Descendants("a").Where(n => n.GetAttributeValue("class", "").Contains("RangeModelCard__name")))
                .Select(node => "https://www.renault.pl" + node.First().GetAttributeValue("href", ""))
                .ToList();

        private List<double> GetPrices(List<HtmlNode> nodes)
            => nodes.Select(node => node.Descendants("span").Where(n => n.GetAttributeValue("class", "").Contains("ModelPrice__priceWrapper")))
                .Select(node => Convert.ToDouble(node.FirstOrDefault() == null ? "0" : node.First().LastChild.InnerText.Replace("zł", "").Replace(" ", "").Trim()))
                .ToList();

        private List<string> GetNames(List<HtmlNode> nodes)
            => nodes.Select(node => node.Descendants("a").Where(n => n.GetAttributeValue("class", "").Contains("RangeModelCard__name")))
                .Select(node => node.First().InnerText.Trim())
                .ToList();
    }
}
