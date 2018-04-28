using System;
using System.Threading.Tasks;
using WebScrapingWithDotNetCore.Chapter03;

namespace WebScrapingWithDotNetCore
{
    class Program
    {
        static void Main(string[] args)
        {
            var task = Task.Run(async () =>
            {
                #region Chapter 01                
                // await MakeWebRequest.SendRequestWithHttpClientAsync();
                // await MakeWebRequest.ReadWithAngleSharpAsync();
                // await MakeWebRequest.ResponseWithErrorsAsync();
                // await MakeWebRequest.ReadNonExistTagAsync();
                // await MakeWebRequest.RunAllAsync();
                #endregion

                #region Chapter 02
                // await ParseComplexHtml.FindGreenClassAsync();
                // await ParseComplexHtml.FindByAttributeAsync();
                // await ParseComplexHtml.FindDescendantAsync();
                // await AdvancedHtmlParsing.FindByRegexAsync();
                #endregion

                #region Chapter 03

                // await StartingToCrawl.TraversingASingleDomainAsync();
                // await StartingToCrawl.FindSpecificLinksAsync();
                // await StartingToCrawl.GetRandomNestedLinksAsync();
                // await StartingToCrawl.GetUniqueLinksAsync();
                // await StartingToCrawl.GetLinksWithInfoAsync();
                // await CrawlingAcrossInternet.FollowExternalOnlyAsync("http://oreilly.com");
                await CrawlingAcrossInternet.GetAllExternalLinksAsync("https://v.qq.com/");

                #endregion

            });

            try
            {
                task.Wait();
            }
            catch (AggregateException aex)
            {
                if (aex.InnerException is NullReferenceException)
                    Console.WriteLine("Null!");
                else
                    throw;
            }
            finally
            {
                Console.ReadKey();
            }
        }
    }
}
