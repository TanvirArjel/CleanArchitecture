using TanvirArjel.ArgumentChecker;

namespace CleanHr.Infrastructure.Services.Configs;

public class SendGridConfig
{
    public SendGridConfig(string apiKey)
    {
        ApiKey = apiKey.ThrowIfNullOrEmpty(nameof(apiKey));
    }

    public string ApiKey { get; set; }
}
