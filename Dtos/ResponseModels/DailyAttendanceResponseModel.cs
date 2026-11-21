namespace SwiftIn.Dtos.ResponseModels
{
    public class DailyAttendanceResponseModel
    {
        public DateOnly Date { get; set; }
        public int TotalActiveTeachers { get; set; }
        public int Present { get; set; }
        public int Absent { get; set; }
        public int NeedingReview { get; set; }
        public List<AttendanceRowModel> Rows { get; set; } = new();
    }
}
