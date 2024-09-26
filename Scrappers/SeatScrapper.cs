using CarScrapper.Models;
using HtmlAgilityPack;

namespace CarScrapper.Scrappers
{
    public class SeatScrapper : Scrapper
    {
        public SeatScrapper(string url) : base(url)
        { }

        public override List<CarScrappedDTO> GetParsedContent()
        {
            var doc = base.GetContent();
            var itemNodes = doc.DocumentNode
                .Descendants("div")
                .Where(node => node.GetAttributeValue("class", "").Split(new char[0]).Contains("model-item") && node.GetAttributeValue("data-model-family-tag", null) != null && !node.GetAttributeValue("data-model-family-tag", null).Contains("family-cars/sport"))
                .ToList();
            
            var cars = new List<CarScrappedDTO>();
            foreach (var link in GetLinks(itemNodes))
            {
                var parsed = GetDataFromLink(link);
                cars.AddRange(parsed);
            }

            return cars;
        }

        private List<CarScrappedDTO> GetDataFromLink(string link)
        {
            var doc = base.GetContent(link);

            var name = GetModelName(doc.DocumentNode);
            var versions = GetVersions(doc.DocumentNode, name);
            var prices = GetPrices(doc.DocumentNode);
            var images = GetImages(doc.DocumentNode);
            var parsed = new List<CarScrappedDTO>();
            for(var i=0; i<versions.Count(); i++)
            {
                var car = new CarScrappedDTO()
                {
                    Make = "SEAT",
                    Model = name + " " + versions[i],
                    Image = images[i],
                    Link = link,
                    Price = prices[i],
                };
                parsed.Add(car);
            }

            return parsed;
        }

        private List<string> GetImages(HtmlNode document)
            => document.Descendants("img").Where(node => node.GetAttributeValue("class","").Contains("trim-img"))
                .Select(node => "https://www.seat.pl" + node.GetAttributeValue("src", "")).ToList();

        private List<double> GetPrices(HtmlNode document)
            => document.Descendants("div")
                .Where(node => node.GetAttributeValue("class", "").Contains("price-tag"))
                .Select(node => Convert.ToDouble(node.InnerText.Replace(".", "").Replace("zł", "").Trim()))
                .ToList();

        private List<string> GetVersions(HtmlNode document, string name)
            => document.Descendants("h3")
            .Where(node => node.GetAttributeValue("class", "").Contains("trim-title"))
            .Select(node => node.InnerText.Replace(name, "").Trim())
            .ToList();


        private string GetModelName(HtmlNode document)
            => document.Descendants("h1")
                .Where(node => node.GetAttributeValue("class", "").Contains("title"))
                .First().InnerText;

           // => document.Descendants("h2").Where(node => node.GetAttributeValue("class", "").Contains("title")).ToList()[1].InnerText;

        private List<string> GetLinks(List<HtmlNode> itemNodes)
            => itemNodes.Select(node => node.Descendants("a").First().GetAttributeValue("href", "")).ToList();
    }
}
