using System;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp.Parser.Html;

namespace WebScrapingWithDotNetCore.Chapter01
{
    public class FirstWebScraper
    {
        public static async Task<string> SendRequestWithHttpClientAsync()
        {
            var client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync("http://pythonscraping.com/pages/page1.html");
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseBody);
            return responseBody;
        }

        public static async Task ReadWithAngleSharpAsync()
        {
            var htmlSourceCode = await SendRequestWithHttpClientAsync();
            var parser = new HtmlParser();
            var document = await parser.ParseAsync(htmlSourceCode);

            Console.WriteLine($"Serializing the (original) document: {document.QuerySelector("h1").OuterHtml}");
            Console.WriteLine($"Serializing the (original) document: {document.QuerySelector("html > body > h1").OuterHtml}");
        }

        public static async Task ResponseWithErrorsAsync()
        {
            try
            {
                var client = new HttpClient();
                var responseBody = await client.GetStringAsync("http://notexistwebsite");
                Console.WriteLine(responseBody);
            }
            catch (HttpRequestException e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
        }

        public static async Task ReadNonExistTagAsync()
        {
            var htmlSourceCode = await SendRequestWithHttpClientAsync();
            var parser = new HtmlParser();
            var document = await parser.ParseAsync(htmlSourceCode);

            var nonExistTag = document.QuerySelector("h8");
            Console.WriteLine(nonExistTag);
            Console.WriteLine($"nonExistTag is null: {nonExistTag is null}");

            try
            {
                Console.WriteLine(nonExistTag.QuerySelector("p").OuterHtml);
            }
            catch (NullReferenceException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Tag was not found");
            }
        }

        public static async Task RunAllAsync()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            async Task<string> GetTileAsync(string uri)
            {
                var httpClient = new HttpClient();
                try
                {
                    var responseHtml = await httpClient.GetStringAsync(uri);
                    var parser = new HtmlParser();
                    var document = await parser.ParseAsync(responseHtml);
                    var tagContent = document.QuerySelector("body > h8").TextContent;
                    return tagContent;
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"{nameof(HttpRequestException)}:");
                    Console.WriteLine("Message :{0} ", e.Message);
                    return null;
                }
                catch (NullReferenceException)
                {
                    Console.WriteLine($"{nameof(NullReferenceException)}:");
                    Console.WriteLine("Tag was not found");
                    return null;
                }
            }

            var title = await GetTileAsync("http://www.pythonscraping.com/pages/page1.html");
            if (string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("Title was not found");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(title);
            }
        }
    }
}