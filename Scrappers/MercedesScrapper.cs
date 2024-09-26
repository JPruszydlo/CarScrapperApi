using CarScrapper.Models;

namespace CarScrapper.Scrappers
{
    public class MercedesScrapper : Scrapper
    {
        public MercedesScrapper(string url) : base(url)
        { }

        public override List<CarScrappedDTO> GetParsedContent()
        {
            var doc = base.GetContent();
            var models = doc.DocumentNode.Descendants("ul").Where(x => x.GetAttributeValue("class", "").Contains("multiselect__content"))
                .ElementAt(1).Descendants("li").Where(x => x.GetAttributeValue("class", "").Contains("multiselect__element"))
                .Select(x => x.InnerText.Split('(')[0].Trim()).ToList();

            var result = Scrap(models);

            return result;
        }

        private List<CarScrappedDTO> Scrap(List<string> models)
        {
            var urlPattern = "https://zasadaauto.pl/samochody-osobowe/Mercedes-Benz/[MODEL]?status=new&sort=leastExpensive";
            var result = new List<CarScrappedDTO>();

            foreach(var model in models)
            {
                var url = urlPattern.Replace("[MODEL]", model.Replace(" ", "-"));
                var doc = base.GetContent(url);
                var item = doc.DocumentNode.Descendants("section").First()
                    .Descendants("a").Where(x => x.GetAttributeValue("class", "").Contains("single-car")).First();

                var link = item.GetAttributeValue("href", "");
                var image = item.Descendants("div").Where(x => x.GetAttributeValue("class", "").Contains("image")).First().GetAttributeValue("lazy-background", "");
                var price = item.Descendants("p").Where(x => x.GetAttributeValue("class", "").Contains("price")).First().InnerText.ToUpper().Replace("OD", "").Replace("BRUTTO", "").Replace("NETTO", "").Replace(" ", "").Replace("\n", "").Replace("PLN", "").Trim();
                result.Add(new CarScrappedDTO()
                {
                    Image = image,
                    Make = "Mercedes",
                    Model = model,
                    Price = Convert.ToDouble(price),
                    Link = link
                }) ;
            }

            return result;
        }
    }
}
