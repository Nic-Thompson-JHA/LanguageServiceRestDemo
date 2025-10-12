using System.Text;
using Newtonsoft.Json.Linq;

class Program
{
    // Entry point
    static async Task Main()
    {
        // Azure resource details
        string endpoint = "https://rg-ai-language-service.cognitiveservices.azure.com/";
        string apiKey = "";

        Console.WriteLine("Enter text to analyze:");
        string text = Console.ReadLine();

        await AnalyzeSentimentAsync(endpoint, apiKey, text);
    }

    static async Task AnalyzeSentimentAsync(string endpoint, string apiKey, string text)
    {
        using var client = new HttpClient();

        // Set the request headers
        client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);

        string route = "/text/analytics/v3.1/sentiment";
        string uri = endpoint + route;

        // Request body: array of documents (you can send multiple)
        string requestBody = $@"{{
            ""documents"": [
                {{ ""id"": ""1"", ""language"": ""en"", ""text"": ""{text}"" }}
            ]
        }}";

        using var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await client.PostAsync(uri, content);
        string result = await response.Content.ReadAsStringAsync();

        Console.WriteLine("\nResponse JSON:");

        var json = JObject.Parse(result);
        var doc = json["documents"]?[0];
        Console.WriteLine($"\nSentiment: {doc?["sentiment"]}");
        Console.WriteLine($"Positive: {doc?["confidenceScores"]?["positive"]}");
        Console.WriteLine($"Neutral: {doc?["confidenceScores"]?["neutral"]}");
        Console.WriteLine($"Negative: {doc?["confidenceScores"]?["negative"]}");
    }
}