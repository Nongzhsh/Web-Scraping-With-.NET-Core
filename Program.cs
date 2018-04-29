using System;
using System.Net.Http;
using System.Threading.Tasks;
using WebScrapingWithDotNetCore.Chapter01;
using WebScrapingWithDotNetCore.Chapter02;
using WebScrapingWithDotNetCore.Chapter03;

namespace WebScrapingWithDotNetCore
{
    class Program
    {
        static int Main(string[] args)
        {
            var task = Task.Run(async () =>
            {
                #region Chapter 01                
                // await FirstWebScraper.SendRequestWithHttpClientAsync();
                // await FirstWebScraper.ReadWithAngleSharpAsync();
                // await FirstWebScraper.ResponseWithErrorsAsync();
                // await FirstWebScraper.ReadNonExistTagAsync();
                // await FirstWebScraper.RunAllAsync();
                #endregion

                #region Chapter 02
                // await AdvancedHtmlParsing.FindGreenClassAsync();
                // await AdvancedHtmlParsing.FindByAttributeAsync();
                // await AdvancedHtmlParsing.FindDescendantAsync();
                // await AdvancedHtmlParsing.FindByRegexAsync();
                #endregion

                #region Chapter 03

                // await StartingToCrawl.TraversingASingleDomainAsync();
                // await StartingToCrawl.FindSpecificLinksAsync();
                // await StartingToCrawl.GetRandomNestedLinksAsync();
                // await StartingToCrawl.GetUniqueLinksAsync();
                // await StartingToCrawl.GetLinksWithInfoAsync();
                // await CrawlingAcrossInternet.FollowExternalOnlyAsync("http://oreilly.com");
                await CrawlingAcrossInternet.GetAllExternalLinksAsync("http://oreilly.com");

                #endregion

            });

            try
            {
                task.Wait();
            }
            catch (AggregateException aex)
            {
                if (aex.InnerException is NullReferenceException)
                {
                    Console.WriteLine("Null!");
                }
                else if (aex.InnerException is HttpRequestException)
                {
                    Console.WriteLine("Request Errors!");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(aex.Message);
                    Console.WriteLine(aex.InnerException.Message);
                    return -1;
                }
            }
            finally
            {
                Console.ReadKey();
            }
            return 0;
        }
    }
}
