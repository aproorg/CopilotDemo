namespace CopilotDemo.Web.Configuration;

public class ApiKeyOptions
{
    public const string SectionName = "ApiKey";
    
    public string Key { get; set; } = string.Empty;
    public string HeaderName { get; set; } = "X-Api-Key";
}
