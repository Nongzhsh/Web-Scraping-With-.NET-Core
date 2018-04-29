using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Parser.Html;

namespace WebScrapingWithDotNetCore.Chapter02
{
    public class AdvancedHtmlParsing
    {
        public static async Task<string> GetHtmlSourceCodeAsync(string uri)
        {
            var httpClient = new HttpClient();
            try
            {
                var htmlSource = await httpClient.GetStringAsync(uri);
                return htmlSource;
            }
            catch (HttpRequestException e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{nameof(HttpRequestException)}: {e.Message}");
                return null;
            }
        }

        public static async Task FindGreenClassAsync()
        {
            const string url = "http://www.pythonscraping.com/pages/warandpeace.html";
            var html = await GetHtmlSourceCodeAsync(url);
            if (!string.IsNullOrWhiteSpace(html))
            {
                var parser = new HtmlParser();
                var document = await parser.ParseAsync(html);
                var nameList = document.QuerySelectorAll("span > .green");

                Console.WriteLine("Green names are:");
                Console.ForegroundColor = ConsoleColor.Green;
                foreach (var item in nameList)
                {
                    Console.WriteLine(item.TextContent);
                }
            }
            else
            {
                Console.WriteLine("No html source code returned.");
            }
        }

        public static async Task FindByAttributeAsync()
        {
            const string url = "http://www.pythonscraping.com/pages/warandpeace.html";
            var html = await GetHtmlSourceCodeAsync(url);
            if (!string.IsNullOrWhiteSpace(html))
            {
                var parser = new HtmlParser();
                var document = await parser.ParseAsync(html);

                var headers = document.QuerySelectorAll("*")
                    .Where(x => new[] { "h1", "h2", "h3", "h4", "h5", "h6" }.Contains(x.TagName.ToLower()));
                Console.WriteLine("Headers are:");
                PrintItemsText(headers);

                var greenAndRed = document.All
                    .Where(x => x.TagName == "span" && (x.ClassList.Contains("green") || x.ClassList.Contains("red")));
                Console.WriteLine("Green and Red spans are:");
                PrintItemsText(greenAndRed);

                var thePrinces = document.QuerySelectorAll("*").Where(x => x.TextContent == "the prince");
                Console.WriteLine(thePrinces.Count());
            }
            else
            {
                Console.WriteLine("No html source code returned.");
            }

            void PrintItemsText(IEnumerable<IElement> elements)
            {
                foreach (var item in elements)
                {
                    Console.WriteLine(item.TextContent);
                }
            }
        }

        public static async Task FindDescendantAsync()
        {
            const string url = "http://www.pythonscraping.com/pages/page3.html";
            var html = await GetHtmlSourceCodeAsync(url);
            if (!string.IsNullOrWhiteSpace(html))
            {
                var parser = new HtmlParser();
                var document = await parser.ParseAsync(html);

                var tableChildren = document.QuerySelector("table#giftList > tbody").Children;
                Console.WriteLine("Table's children are:");
                foreach (var child in tableChildren)
                {
                    System.Console.WriteLine(child.LocalName);
                }

                var descendants = document.QuerySelectorAll("table#giftList > tbody *");
                Console.WriteLine("Table's descendants are:");
                foreach (var item in descendants)
                {
                    Console.WriteLine(item.LocalName);
                }

                var siblings = document.QuerySelectorAll("table#giftList > tbody > tr").Select(x => x.NextElementSibling);
                Console.WriteLine("Table's next siblings are:");
                foreach (var item in siblings)
                {
                    Console.WriteLine(item?.LocalName);
                }

                var parentSibling = document.All.SingleOrDefault(x => x.HasAttribute("src") && x.GetAttribute("src") == "../img/gifts/img1.jpg")
                    ?.ParentElement.PreviousElementSibling;
                if (parentSibling != null)
                {
                    Console.WriteLine($"Parent's previous sibling is: {parentSibling.TextContent}");
                }
            }
            else
            {
                Console.WriteLine("No html source code returned.");
            }
        }

        public static async Task FindByRegexAsync()
        {
            const string url = "http://www.pythonscraping.com/pages/page3.html";
            var html = await GetHtmlSourceCodeAsync(url);
            if (!string.IsNullOrWhiteSpace(html))
            {
                var parser = new HtmlParser();
                var document = await parser.ParseAsync(html);

                var images = document.QuerySelectorAll("img")
                    .Where(x => x.HasAttribute("src") && Regex.Match(x.Attributes["src"].Value, @"\.\.\/img\/gifts/img.*\.jpg").Success);
                foreach (var item in images)
                {
                    Console.WriteLine(item.Attributes["src"].Value);
                }

                var elementsWith2Attributes = document.All.Where(x => x.Attributes.Length == 2);
                foreach (var item in elementsWith2Attributes)
                {
                    Console.WriteLine(item.LocalName);
                    foreach (var attr in item.Attributes)
                    {
                        Console.WriteLine($"\t{attr.Name} - {attr.Value}");
                    }
                }
            }
            else
            {
                Console.WriteLine("No html source code returned.");
            }
        }
    }
}