namespace AzureFunctions_TextAnalyzer.Service.Model
{
    public class ServiceBusMessageModel
    {
        public string Name { get; set; }
        public Dictionary<string, int> WordsCount { get; set; }
    }
}
