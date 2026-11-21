namespace SwiftIn.Dtos.ResponseModels
{
    public class LoginResponseModel
    {
        public string Token { get; set; } = default!;
        public DateTime ExpiresAt { get; set; }
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public List<string> Roles { get; set; } = new();
    }
}
