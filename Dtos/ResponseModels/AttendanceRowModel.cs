namespace SwiftIn.Dtos.ResponseModels
{
    public class AttendanceRowModel
    {
        public string TeacherId { get; set; } = default!;
        public string TeacherName { get; set; } = default!;
        public bool IsPresent { get; set; }
        public DateTime? FirstIn { get; set; }
        public DateTime? LastOut { get; set; }
        public bool HasUnreviewedCheckIns { get; set; }
    }
}
