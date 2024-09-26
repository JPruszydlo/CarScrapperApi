using HtmlAgilityPack;

namespace CarScrapper.Scrappers
{
    public class CitroenScrapper : OpelScrapper
    {
        public CitroenScrapper(string url) : base(url)
        {
            brandName = "Citroën";
        }

        protected override string GetModelName(HtmlNode linkElement)
        {
            var name= linkElement.InnerText.ToUpper();
            name = name.Substring(name.IndexOf(" ")).Trim();
            return name;
        }
    }
}
