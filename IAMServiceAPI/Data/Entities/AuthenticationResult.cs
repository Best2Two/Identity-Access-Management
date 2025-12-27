namespace IAMService.Data.Entities
{
    public class AuthenticationResult
    {
        public bool Success { get; private set; }
        public string? AccessToken { get; private set; }
        public string? RefreshToken { get; private set; }
        public IEnumerable<string> Errors { get; private set; } = [];
        public string? Message { get; private set; }

        private AuthenticationResult() { }

        public static AuthenticationResult Succeeded(
            string message,
            string accessToken,
            string refreshToken
            )
        {
            return new AuthenticationResult
            {
                Success = true,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Errors = [],
                Message = message
            };
        }

        public static AuthenticationResult Succeeded(string message)
        {
            return new AuthenticationResult
            {
                Success = true,
                AccessToken = null,
                RefreshToken = null,
                Message = message
            };
        }

        public static AuthenticationResult Failed(string message, params string[] errors)
        {
            return new AuthenticationResult
            {
                Success = false,
                AccessToken = null,
                RefreshToken = null,
                Errors = errors,
                Message = message
            };
        }
    }
}