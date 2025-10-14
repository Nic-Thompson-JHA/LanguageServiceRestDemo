using System.Text;
using Newtonsoft.Json.Linq;

class Program
{
    // Azure resource details
    private static string endpoint = "https://rg-ai-language-service.cognitiveservices.azure.com/";
    private static string apiKey = "";

    // Entry point
    static async Task Main()
    {
        Console.WriteLine("Enter text to analyze:");
        string text = Console.ReadLine();

        await CallTextApi("LanguageDetection", text);
        await CallTextApi("SentimentAnalysis", text);
        await CallTextApi("KeyPhraseExtraction", text);
    }

    static async Task CallTextApi(string kind, string text)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);

        string uri = $"{endpoint}/language/:analyze-text?api-version=2023-04-01";
        string requestBody = $@"{{
            ""kind"": ""{kind}"",
            ""analysisInput"": {{
                ""documents"": [{{ ""id"": ""1"", ""text"": ""{text}"" }}]
            }}
        }}";

        using var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await client.PostAsync(uri, content);
        string result = await response.Content.ReadAsStringAsync();
        var json = JObject.Parse(result);

        Console.WriteLine($"\n=== {kind} ===");

        switch (kind)
        {
            case "LanguageDetection":
                var lang = json["results"]?["documents"]?[0]?["detectedLanguage"]?["name"];
                Console.WriteLine($"Language: {lang}");
                break;

            case "SentimentAnalysis":
                var sent = json["results"]?["documents"]?[0]?["sentiment"];
                var scores = json["results"]?["documents"]?[0]?["confidenceScores"];
                Console.WriteLine($"Sentiment: {sent}");
                Console.WriteLine($"Positive: {scores?["positive"]}\nNeutral: {scores?["neutral"]}\nNegative: {scores?["negative"]}");
                break;

            case "KeyPhraseExtraction":
                var phrases = json["results"]?["documents"]?[0]?["keyPhrases"];
                Console.WriteLine("Key Phrases: " + string.Join(", ", phrases ?? new JArray()));
                break;
        }
    }
}