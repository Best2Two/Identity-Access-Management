namespace IAMService.Data.DTOs
{
    public class AuthenticationResult
    {
        public bool Success { get; private set; }
        public string? AccessToken { get; private set; }
        public string? RefreshToken { get; private set; }
        public IEnumerable<string> Errors { get; private set; } = [];

        private AuthenticationResult() { }

        public static AuthenticationResult Succeeded(
            string accessToken,
            string refreshToken
            )
        {
            return new AuthenticationResult
            {
                Success = true,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Errors = []
            };
        }

        public static AuthenticationResult Failed(params string[] errors)
        {
            return new AuthenticationResult
            {
                Success = false,
                AccessToken = null,
                RefreshToken = null,
                Errors = errors
            };
        }
    }
}