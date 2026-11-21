using SwiftIn.Dtos.RequestModels;
using SwiftIn.Dtos.ResponseModels;

namespace SwiftIn.Services.Interface
{
    public interface ITeacherService
    {
        Task<BaseResponse<TeacherResponseModel>> RegisterTeacherAsync(CreateTeacherRequestModel request);
        Task<BaseResponse<TeacherResponseModel>> UpdateTeacherAsync(string id, UpdateTeacherRequestModel request);
        Task<BaseResponse<TeacherResponseModel?>> GetTeacherByIdAsync(string id);
        Task<BaseResponse<List<TeacherResponseModel>>> GetAllTeachersAsync();
        Task DeactivateTeacherAsync(string id);
        Task DeleteFaceDataAsync(string id);
    }
}