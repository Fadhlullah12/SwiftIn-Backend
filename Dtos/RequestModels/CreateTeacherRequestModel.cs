namespace SwiftIn.Dtos.RequestModels
{
    public class CreateTeacherRequestModel
    {
        public string StaffNumber { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public bool ConsentGiven { get; set; }
    }
}
