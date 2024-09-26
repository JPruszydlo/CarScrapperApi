using CarScrapper.Models;
using System.Text.RegularExpressions;

namespace CarScrapper.Scrappers
{
    public class SkodaScrapper : Scrapper
    {
        public SkodaScrapper(string url) : base(url)
        { }

        public override List<CarScrappedDTO> GetParsedContent()
        {
            var content = base.GetContent();
            var html = content.ParsedText;
            html = GetCarsSection(html);
            return GetModelList(html);
        }

        private List<CarScrappedDTO>? GetModelList(string html)
        {
            var names = GetModelNames(html);
            var images = GetImages(html);
            var links = GetLinks(html);
            var prices = GetPrices(html);
            if (names.Count() != links.Count() || images.Count() != prices.Count())
                return null;
            var cars = new List<CarScrappedDTO>();
            for (var i = 0; i < names.Count(); i++)
            {
                cars.Add(new CarScrappedDTO()
                {
                    Make = "Skoda",
                    Model = names[i],
                    Image = images[i] + "_bp1920_1.webp",
                    Link = links[i],
                    Price = Convert.ToDouble(prices[i].Replace(" ", "").Trim())
                });
            }
            return cars;
        }

        private List<string> GetImages(string html)
        {
            Regex urlReg = new Regex(@"https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&\/=]*)");
            var images = urlReg.Matches(html).Where((l, idx) => idx % 2 != 0).Select(m => m.Groups[0].Value).ToList();
            return images;
        }

        private List<string> GetPrices(string html)
        {
            Regex priceReg = new Regex(@"(value:([0-9\s]+))");
            var prices = priceReg.Matches(html).Select(p => p.Groups[0].Value.Split(":")[1].Trim()).ToList();
            return prices;
        }

        private List<string> GetLinks(string html)
        {
            Regex urlReg = new Regex(@"https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&\/=]*)");
            var links = urlReg.Matches(html).Where((l, idx) => idx % 2 == 0).Select(m => m.Groups[0].Value).ToList();
            return links;
        }

        private List<string> GetModelNames(string html)
        {
            Regex modelNameReg = new Regex(@"(modelName:([a-zA-Z0-9\s.:\/\/-é&amp;]+))");
            var modelNames = modelNameReg
                .Matches(html)
                .Select(m => m.Groups[0].Value
                    .Split(":")[1]
                    .Replace("Nowy", "")
                    .Replace("Nowa", "")
                    .Replace("Nowe", "")
                    .Trim())
                .ToList();
            return modelNames;
        }

        private string GetCarsSection(string html)
        {
            var startIdx = html.IndexOf("bodies");
            var endIdx = html.IndexOf(",&quot;title&quot;:&quot;Modele");
            return html.Substring(startIdx, endIdx - startIdx).Replace("&quot;", "");
        }

    }
}
