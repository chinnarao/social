using System;

namespace account
{
    public class TokenVM
    {
        public string BearerToken { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string TokenType { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
    }
}
