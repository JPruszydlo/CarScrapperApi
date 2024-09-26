using CarScrapper.Models;
using HtmlAgilityPack;

namespace CarScrapper.Scrappers
{
    public class VolkswagenScrapper : Scrapper
    {
        public VolkswagenScrapper(string url) : base(url)
        { }

        public override List<CarScrappedDTO> GetParsedContent()
        {
            var doc = base.GetContent();

            var carItems = doc.DocumentNode.Descendants("li")
                .Where(node => 
                    node.GetAttributeValue("data-testid", "").Contains("cartile") &&
                    !node.GetAttributeValue("aria-label", "").Contains("Volkswagen Samochody Dostawcze") &&
                    !node.GetAttributeValue("aria-label", "").Contains("ID. Buzz") &&
                    !node.GetAttributeValue("aria-label", "").Contains("Wszystkie modele sportowe")
                )
                .ToList();

            var names = GetModelNames(carItems);
            var prices = GetPrices(carItems);
            var images = GetImages(carItems);
            var links = GetLinks(carItems);

            var result = new List<CarScrappedDTO>();
            for(var i=0; i<names.Count; i++)
            {
                result.Add(new CarScrappedDTO()
                {
                    Make = "Volkswagen",
                    Model = names[i],
                    Image = images[i],
                    Price = prices[i],
                    Link = links[i]
                });
            }
            return result;
        }

        private List<string> GetLinks(List<HtmlNode> nodes)
            => nodes.Select(node => node.Descendants("a").Where(n => n.GetAttributeValue("data-testid", "").Contains("primary-cartile-cta")))
                .Select(node => "https://www.volkswagen.pl" + node.First().GetAttributeValue("href", ""))
                .ToList();
        

        private List<string> GetImages(List<HtmlNode> nodes)
            => nodes.Select(node => node.Descendants("img").Where(n => n.GetAttributeValue("class", "").Contains("Image-sc-")))
                .Select(node => node.FirstOrDefault() == null ? "" : node.First().GetAttributeValue("src", ""))
                .ToList();
         

        private List<double> GetPrices(List<HtmlNode> nodes)
            => nodes.Select(node => node.Descendants("div").Where(n => n.GetAttributeValue("class", "").Contains("StyledTextComponent-sc-")))
                .Select(node => Convert.ToDouble(node.FirstOrDefault() == null ? "0" : node.First().InnerHtml.Replace("od", "").Replace("&nbsp;", "").Replace("zł", "").Replace("VAT", "").Replace("z", "").Replace(" ", "").Trim()))
                .ToList();
         

        private List<string> GetModelNames(List<HtmlNode> nodes)
            => nodes.Select(node => node.Descendants("h3").Where(n => n.GetAttributeValue("class", "").Contains("StyledTextComponent-sc-")))
                .Select(node => node.First().InnerText)
                .ToList();
    }
}
