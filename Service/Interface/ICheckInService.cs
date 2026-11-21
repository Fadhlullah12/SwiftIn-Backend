using SwiftIn.Dtos.RequestModels;
using SwiftIn.Dtos.ResponseModels;
using SwiftIn.Models.Entities;

namespace SwiftIn.Services.Interface
{
    public interface ICheckInService
    {
        // Called by the camera worker. Data is null when the face wasn't recognised or was a repeat.
        Task<BaseResponse<CheckInResponseModel?>> RecordRecognitionAsync(RecognitionEventRequestModel request);

        // Admin fallback when the camera missed someone
        Task<BaseResponse<CheckInResponseModel>> CreateManualAsync(ManualCheckInRequestModel request, ApplicationUser performedBy);

        // Approve or reject a NeedsReview check-in
        Task<BaseResponse<CheckInResponseModel>> ReviewAsync(string checkInId, ReviewCheckInRequestModel request, ApplicationUser reviewedBy);

        Task<BaseResponse<PagedResponse<CheckInResponseModel>>> GetCheckInsAsync(CheckInFilterRequestModel filter);

        Task<BaseResponse<DailyAttendanceResponseModel>> GetDailyAttendanceAsync(DateOnly date);
    }
}