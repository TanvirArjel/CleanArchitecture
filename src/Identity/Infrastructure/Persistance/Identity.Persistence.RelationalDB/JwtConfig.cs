using TanvirArjel.ArgumentChecker;

namespace Identity.Persistence.RelationalDB
{
    public class JwtConfig
    {
        public JwtConfig(string issuer, string key)
        {
            Issuer = issuer.ThrowIfNullOrEmpty(nameof(issuer));
            Key = key.ThrowIfNullOrEmpty(nameof(key));
        }

        public string Issuer { get; private set; }
        public string Key { get; private set; }
    }
}
