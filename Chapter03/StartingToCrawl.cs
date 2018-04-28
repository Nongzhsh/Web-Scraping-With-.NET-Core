using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Parser.Html;

namespace WebScrapingWithDotNetCore.Chapter03
{
    public class StartingToCrawl
    {
        public static async Task TraversingASingleDomainAsync()
        {
            var httpClient = new HttpClient();
            var htmlSource = await httpClient.GetStringAsync("http://en.wikipedia.org/wiki/Kevin_Bacon");

            var parser = new HtmlParser();
            var document = await parser.ParseAsync(htmlSource);
            var links = document.QuerySelectorAll("a");
            foreach (var link in links)
            {
                Console.WriteLine(link.Attributes["href"]?.Value);
            }
        }

        public static async Task FindSpecificLinksAsync()
        {
            var httpClient = new HttpClient();
            var htmlSource = await httpClient.GetStringAsync("http://en.wikipedia.org/wiki/Kevin_Bacon");

            var parser = new HtmlParser();
            var document = await parser.ParseAsync(htmlSource);
            var links = document.QuerySelector("div#bodyContent").QuerySelectorAll("a")
                .Where(x => x.HasAttribute("href") && Regex.Match(x.Attributes["href"].Value, @"^(/wiki/)((?!:).)*$").Success);
            foreach (var link in links)
            {
                Console.WriteLine(link.Attributes["href"]?.Value);
            }
        }

        private static async Task<IEnumerable<IElement>> GetLinksAsync(string uri)
        {
            var httpClient = new HttpClient();
            var htmlSource = await httpClient.GetStringAsync($"http://en.wikipedia.org{uri}");
            var parser = new HtmlParser();
            var document = await parser.ParseAsync(htmlSource);

            var links = document.QuerySelector("div#bodyContent").QuerySelectorAll("a")
                .Where(x => x.HasAttribute("href") && Regex.Match(x.Attributes["href"].Value, @"^(/wiki/)((?!:).)*$").Success);
            return links;
        }

        public static async Task GetRandomNestedLinksAsync()
        {
            var random = new Random();
            var links = (await GetLinksAsync("/wiki/Kevin_Bacon")).ToList();
            while (links.Any())
            {
                var newArticle = links[random.Next(0, links.Count)].Attributes["href"].Value;
                Console.WriteLine(newArticle);
                links = (await GetLinksAsync(newArticle)).ToList();
            }
        }

        public static async Task GetUniqueLinksAsync(string uri = "")
        {
            var linkSet = new HashSet<string>();

            var httpClient = new HttpClient();
            var htmlSource = await httpClient.GetStringAsync($"http://en.wikipedia.org{uri}");

            var parser = new HtmlParser();
            var document = await parser.ParseAsync(htmlSource);

            var links = document.QuerySelectorAll("a")
                .Where(x => x.HasAttribute("href") && Regex.Match(x.Attributes["href"].Value, @"^(/wiki/)").Success);

            foreach (var link in links)
            {
                if (!linkSet.Contains(link.Attributes["href"].Value))
                {
                    var newPage = link.Attributes["href"].Value;
                    Console.WriteLine(newPage);
                    linkSet.Add(newPage);
                    await GetUniqueLinksAsync(newPage);
                }
            }
        }

        public static async Task GetLinksWithInfoAsync(string uri = "")
        {
            var linkSet = new HashSet<string>();

            var httpClient = new HttpClient();
            var htmlSource = await httpClient.GetStringAsync($"http://en.wikipedia.org{uri}");

            var parser = new HtmlParser();
            var document = await parser.ParseAsync(htmlSource);

            try
            {
                var title = document.QuerySelector("h1").TextContent;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(title);

                var contentElement = document.QuerySelector("#mw-content-text").QuerySelectorAll("p").FirstOrDefault();
                if (contentElement != null)
                {
                    Console.WriteLine(contentElement.TextContent);
                }

                var alink = document.QuerySelector("#ca-edit").QuerySelectorAll("span a").SingleOrDefault(x => x.HasAttribute("href"))?.Attributes["href"].Value;
                Console.WriteLine(alink);
            }
            catch (NullReferenceException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Cannot find the tag!");
            }

            var links = document.QuerySelectorAll("a")
                .Where(x => x.HasAttribute("href") && Regex.Match(x.Attributes["href"].Value, @"^(/wiki/)").Success).ToList();
            foreach (var link in links)
            {
                if (!linkSet.Contains(link.Attributes["href"].Value))
                {
                    var newPage = link.Attributes["href"].Value;
                    Console.WriteLine(newPage);
                    linkSet.Add(newPage);
                    await GetLinksWithInfoAsync(newPage);
                }
            }
        }
    }
}
