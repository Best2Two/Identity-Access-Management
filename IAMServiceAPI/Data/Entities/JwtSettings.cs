namespace IAMService.Data.Entities
{
    public class JwtSettings
    {
        // This maps to the section name in appsettings.json
        public const string SectionName = "JwtSettings";

        public string Secret { get; init; } = string.Empty;
        public string Issuer { get; init; } = string.Empty;
        public string Audience { get; init; } = string.Empty;
        public int ExpiryMinutes { get; init; }
        public int RefreshTokenExpiryDays { get; init; }
    }
}
