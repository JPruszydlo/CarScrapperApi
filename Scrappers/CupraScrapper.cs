using CarScrapper.Models;
using HtmlAgilityPack;

namespace CarScrapper.Scrappers
{
    public class CupraScrapper : Scrapper
    {
        public CupraScrapper(string url) : base(url)
        { }

        public override List<CarScrappedDTO> GetParsedContent()
        {
            var doc = GetContent();
            var items = doc.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("class", "").Split(new char[0]).Last() == "cmp-car-range__swiper__item")
                .ToList();

            var result = GetCars(items);
            return result;
        }

        private List<CarScrappedDTO> GetCars(List<HtmlNode> nodes)
        {
            var result = new List<CarScrappedDTO>();

            foreach (var node in nodes)
            {
                var model = node.GetAttributeValue("data-idmodel", "");
                var image = "https://www.cupraofficial.pl" + node.Descendants("img").First().GetAttributeValue("src", "");
                var price = string.Join("", node.Descendants("div").Where(node => node.GetAttributeValue("class", "").Contains("item__info-wrapper__prices-container__total-price"))
                    .First().InnerText.Where(Char.IsDigit).ToArray());
                var link = node.Descendants("a").First().GetAttributeValue("href", "");

                result.Add(new CarScrappedDTO
                {
                    Make = "Cupra",
                    Model = model,
                    Price = Convert.ToDouble(price),
                    Link = link,
                    Image = image
                });
            }

            return result;
        }
    }
}
