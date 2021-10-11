using TanvirArjel.ArgumentChecker;

namespace Identity.Api.Configs
{
    public class JwtConfig
    {
        public JwtConfig(string issuer, string key, int tokenLifeTime)
        {
            Issuer = issuer.ThrowIfNullOrEmpty(nameof(issuer));
            Key = key.ThrowIfNullOrEmpty(nameof(key));
            TokenLifeTime = tokenLifeTime;
        }

        public string Issuer { get; private set; }

        public string Key { get; private set; }

        public int TokenLifeTime { get; private set; }
    }
}
