using System;
using System.Threading.Tasks;
using WebScrapingWithDotNetCore.Chapter01;
using WebScrapingWithDotNetCore.Chapter02;

namespace WebScrapingWithDotNetCore
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(async () =>
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
                await ParseComplexHtml.FindByRegexAsync();
                #endregion

            }).GetAwaiter().GetResult();
        }
    }
}
