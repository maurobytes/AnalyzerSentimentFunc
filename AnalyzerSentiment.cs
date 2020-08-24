using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure;
using Azure.AI.TextAnalytics;

namespace AnalyzerSentiment
{
    public static class AnalyzerSentiment
    {
        [FunctionName("AnalyzerSentiment")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            AzureKeyCredential credentials = new AzureKeyCredential("d00f6d9924c84109af5180e3068356bc");
            Uri endpoint = new Uri("https://analyzersentiment.cognitiveservices.azure.com/");

            string text = req.Query["text"];

            var client = new TextAnalyticsClient(endpoint, credentials);

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            text = text ?? data?.text;

            if (!string.IsNullOrEmpty(text))
            {
                DocumentSentiment documentSentiment = client.AnalyzeSentiment(text);
                return new OkObjectResult(documentSentiment.Sentiment.ToString());
            }
            else
            {
                return new OkObjectResult("This HTTP triggered function executed successfully. Pass a text in the query string or in the request body for a personalized response.");
            }
        }
    }
}
