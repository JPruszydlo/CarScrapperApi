using CarScrapper.Models;
using HtmlAgilityPack;
using System.Security.Policy;

namespace CarScrapper.Scrappers
{
    public class BmwScrapper : Scrapper
    {
        public BmwScrapper(string url) : base(url)
        { }

        public override List<CarScrappedDTO> GetParsedContent()
        {
            var doc = GetContent();
            var groups = doc.DocumentNode.Descendants("div")
                .Where(node =>
                    node.GetAttributeValue("class", "").Split(new char[0])[0] == "cmp-modelselection__group" &&
                    !node.GetAttributeValue("data-group-name", "").Contains("OFERTY") &&
                    !node.GetAttributeValue("data-group-name", "").Contains("Concept Cars") &&
                    !node.GetAttributeValue("data-group-name", "").Contains("Protection Vehicle")
                )
                .ToList();

            var result = new List<CarScrappedDTO>();
            for (var i = 0; i < groups.Count; i++)
            {
                var names = GetNames(groups[i]);
                var prices = GetPrices(groups[i]);
                var links = GetLinks(groups[i]);
                var images = GetImages(links);

                for (var j = 0; j < names.Count; j++)
                {
                    result.Add(new CarScrappedDTO
                    {
                        Make = "BMW",
                        Model = names[j],
                        Link = links[j],
                        Price = prices[j],
                        Image = images[j],
                    });
                }
            }

            return result;

        }

        private List<string> GetImages(List<string> links)
        {
            var result = new List<string>();
            foreach(var link in links)
            {
                HtmlDocument doc = null;
                try
                {
                    HtmlWeb web = new HtmlWeb();
                    doc = web.Load(link);
                }
                catch(Exception ex)
                {
                    result.Add("");
                    continue;
                }
                var tmp = doc.DocumentNode.Descendants("source")
                    .Select(i => i.GetAttributeValue("srcset", ""))
                    .ToList();
                //cmp - image__image
                var img = tmp.FirstOrDefault(x => x.Contains("all-models"));
                if (img != null)
                    img = "https://www.bmw.pl" + img.Split(new char[0])[0];
                result.Add(img ?? "");
            }
            return result;
        }

        private List<string> GetLinks(HtmlNode node)
            => node.Descendants("a").Where(node => node.GetAttributeValue("class", "").Contains("cmp-modelselection__cta-show-more"))
                .Select(node => "https://www.bmw.pl" + node.GetAttributeValue("href", ""))
                .ToList();


        private List<string> GetNames(HtmlNode node)
            => node.Descendants("h5").Where(n => n.GetAttributeValue("class", "").Contains("cmp-modelcard__name"))
                .Select(node => node.InnerText.Replace("BMW", "").Trim())
                .ToList();

        private List<double> GetPrices(HtmlNode node)
            => node.Descendants("span").Where(node => node.GetAttributeValue("class", "").Contains("cmp-modelcard__price"))
                  .Select(node => Convert.ToDouble(node.InnerText.Trim() == "" ? "0" : node.InnerText.ToUpper().Replace(" ", "").Replace("OD", "").Replace("PLN", "").Replace(" ", "").Trim()))
                  .ToList();
    }
}
