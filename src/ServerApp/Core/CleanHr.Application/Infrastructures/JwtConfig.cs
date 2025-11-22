using TanvirArjel.ArgumentChecker;

namespace CleanHr.Application.Infrastructures;

public class JwtConfig(string issuer, string key, int tokenLifeTime)
{
    public string Issuer { get; private set; } = issuer.ThrowIfNullOrEmpty(nameof(issuer));

    public string Key { get; private set; } = key.ThrowIfNullOrEmpty(nameof(key));

    public int TokenLifeTime { get; private set; } = tokenLifeTime;
}
