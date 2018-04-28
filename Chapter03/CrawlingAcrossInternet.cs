using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp.Parser.Html;

namespace WebScrapingWithDotNetCore.Chapter03
{
    public class CrawlingAcrossInternet
    {
        private static readonly Random Random = new Random();

        public static async Task FollowExternalOnlyAsync(string startingSite)
        {
            var externalLink = await GetRandomExternalLinkAsync(startingSite);
            if (externalLink != null)
            {
                Console.WriteLine($"External Links is: {externalLink}");
                await FollowExternalOnlyAsync(externalLink);
            }
            else
            {
                Console.WriteLine("Random External link is null, Crawling terminated.");
            }
        }

        private static async Task<string> GetRandomExternalLinkAsync(string startingPage)
        {
            var httpClient = new HttpClient();
            try
            {
                var htmlSource = await httpClient.GetStringAsync(startingPage);
                var externalLinks = (await GetExternalLinksAsync(htmlSource, SplitAddress(startingPage)[0])).ToList();
                if (externalLinks.Any())
                {
                    return externalLinks[Random.Next(0, externalLinks.Count)];
                }

                var internalLinks = (await GetInternalLinksAsync(htmlSource, startingPage)).ToList();
                if (internalLinks.Any())
                {
                    return await GetRandomExternalLinkAsync(internalLinks[Random.Next(0, internalLinks.Count)]);
                }

                return null;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Error requesting: {e.Message}");
                return null;
            }
        }

        private static string[] SplitAddress(string address)
        {
            var addressParts = address.Replace("http://", "").Replace("https://", "").Split("/");
            return addressParts;
        }

        private static async Task<IEnumerable<string>> GetInternalLinksAsync(string htmlSource, string includeUrl)
        {
            var internalLinks = new HashSet<string>();
            var parser = new HtmlParser();
            var document = await parser.ParseAsync(htmlSource);
            var links = document.QuerySelectorAll("a")
                .Where(x => x.HasAttribute("href") && Regex.Match(x.Attributes["href"].Value, $@"^(/|.*{includeUrl})").Success)
                .Select(x => x.Attributes["href"].Value);
            foreach (var link in links)
            {
                if (!string.IsNullOrEmpty(link) && !internalLinks.Contains(link))
                {
                    internalLinks.Add(link);
                }
            }
            return internalLinks;
        }

        private static async Task<IEnumerable<string>> GetExternalLinksAsync(string htmlSource, string excludeUrl)
        {
            var externalLinks = new HashSet<string>();
            var parser = new HtmlParser();
            var document = await parser.ParseAsync(htmlSource);

            var links = document.QuerySelectorAll("a")
                .Where(x => x.HasAttribute("href") && Regex.Match(x.Attributes["href"].Value, $@"^(http|www)((?!{excludeUrl}).)*$").Success)
                .Select(x => x.Attributes["href"].Value);
            foreach (var link in links)
            {
                if (!string.IsNullOrEmpty(link) && !externalLinks.Contains(link))
                {
                    externalLinks.Add(link);
                }
            }
            return externalLinks;
        }

        public static async Task GetAllExternalLinksAsync(string siteUrl)
        {
            var httpClient = new HttpClient();
            var allExternalLinks = new HashSet<string>();
            var allInternalLinks = new HashSet<string>();
            try
            {
                var htmlSource = await httpClient.GetStringAsync(siteUrl);
                var internalLinks = await GetInternalLinksAsync(htmlSource, SplitAddress(siteUrl)[0]);
                var externalLinks = await GetExternalLinksAsync(htmlSource, SplitAddress(siteUrl)[0]);
                foreach (var link in externalLinks)
                {
                    if (!allExternalLinks.Contains(link))
                    {
                        allExternalLinks.Add(link);
                        Console.WriteLine(link);
                    }
                }

                foreach (var link in internalLinks)
                {
                    if (!allInternalLinks.Contains(link))
                    {
                        Console.WriteLine($"The link is: {link}");
                        allInternalLinks.Add(link);
                        await GetAllExternalLinksAsync(link);
                    }
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e);
                Console.WriteLine($"Request error: {e.Message}");
            }
        }
    }
}
