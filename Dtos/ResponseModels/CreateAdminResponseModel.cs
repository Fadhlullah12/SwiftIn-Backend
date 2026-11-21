namespace SwiftIn.Dtos.ResponseModels
{
    public class CreateAdminResponseModel
    {
        public string Id { get; set; } = default!;   // Identity user ids are strings
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Role { get; set; } = default!;
    }
}
