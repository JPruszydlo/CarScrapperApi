using CarScrapper.Models;
using HtmlAgilityPack;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using PuppeteerSharp;

namespace CarScrapper.Scrappers
{
    public class AudiScrapper : Scrapper
    {
        public AudiScrapper(string url) : base(url)
        { }
        
        public override List<CarScrappedDTO> GetParsedContent()
        {
            var doc = base.GetContent();

            var items = doc.DocumentNode.Descendants("div")
                .Where(node => node.GetAttributeValue("class", "").Contains("audi-modelfinder__car-model-copy"))
                .ToList();

            var names = GetModelNames(items);
            var prices = GetPrices(items);
            var links = GetLinks(doc.DocumentNode);
            var images = GetImages(doc.DocumentNode);


            var result = new List<CarScrappedDTO>();
            for(var i=0; i< names.Count; i++)
            {
                result.Add(new CarScrappedDTO()
                {
                    Make = "Audi",
                    Model = names[i],
                    Price = prices[i],
                    Link = links[i],
                    Image = images[i]
                });
            }

            return result;
        }

        private List<string> GetImages(HtmlNode document)
            => document.Descendants("img")
                .Where(node => node.GetAttributeValue("class", "").Contains("audi-modelfinder__car-model-image"))
                .Select(node => node.GetAttributeValue("data-src", node.GetAttributeValue("src", "")))
                .ToList();

        private List<string> GetLinks(HtmlNode document)
            => document.Descendants("a")
                .Where(node => node.GetAttributeValue("class", "").Contains("audi-modelfinder__car-model-link"))
                .Select(node => "https://www.audi.pl" + node.GetAttributeValue("href", ""))
                .ToList();
         

        private List<double> GetPrices(List<HtmlNode> nodes)
            => nodes.Select(node => node.Descendants("p").Where(n => n.GetAttributeValue("class", "").Contains("audi-copy-s audi-modelfinder__car-model-price")))
                .Select(node => Convert.ToDouble(node.FirstOrDefault() == null ? "0" : node.First().InnerText.Replace("Cena", "").Replace("od", "").Replace("PLN", "").Replace(" ", "").Trim()))
                .ToList();

        private List<string> GetModelNames(List<HtmlNode> nodes)
            => nodes.Select(node => node.Descendants("h2").Where(n => n.GetAttributeValue("class", "").Contains("audi-modelfinder__car-model-full-name")))
                .Select(node => node.First().InnerText.Replace("\n\t", " ").Replace("\n", "").Replace("\t", "").Trim())
                .ToList();

    }
}