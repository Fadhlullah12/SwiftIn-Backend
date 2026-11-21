namespace SwiftIn.Settings
{
    // Bound from the "Jwt" section of appsettings.json
    public class JwtSettings
    {
        public string Key { get; set; } = default!;          // secret, at least 32 characters
        public string Issuer { get; set; } = "SwiftIn";
        public string Audience { get; set; } = "SwiftInClients";
        public int ExpiryMinutes { get; set; } = 480;
    }
}