using System;
using System.Threading.Tasks;
using WebScrapingWithDotNetCore.Chapter01;

namespace WebScrapingWithDotNetCore
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                // C01
                // await MakeWebRequest.SendRequestWithHttpClientAsync();
                // await MakeWebRequest.ReadWithAngleSharpAsync();
                // await MakeWebRequest.ResponseWithErrorsAsync();
                // await MakeWebRequest.ReadNonExistTagAsync();
                // await MakeWebRequest.RunAllAsync();

            }).GetAwaiter().GetResult();
        }
    }
}
