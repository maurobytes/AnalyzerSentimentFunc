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

            AzureKeyCredential credentials = new AzureKeyCredential("<reemplazar-con-tu-text-analytics-key>");
            Uri endpoint = new Uri("<reemplazar-con-tu-endpoint-analytics>");

            string text = req.Query["body"];

            var client = new TextAnalyticsClient(endpoint, credentials);

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            text = text ?? data?.body;

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
