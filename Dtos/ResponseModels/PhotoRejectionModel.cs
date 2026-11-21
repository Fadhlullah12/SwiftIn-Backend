namespace SwiftIn.Dtos.ResponseModels
{
    // A photo that couldn't be used, and why (no face, too blurry, several faces...)
    public class PhotoRejectionModel
    {
        public string FileName { get; set; } = default!;
        public string Reason { get; set; } = default!;
    }
}
