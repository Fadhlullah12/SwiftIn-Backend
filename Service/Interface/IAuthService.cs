using SwiftIn.Dtos.RequestModels;
using SwiftIn.Dtos.ResponseModels;

namespace SwiftIn.Services
{
    public interface IAuthService
    {
        Task<BaseResponse<LoginResponseModel>> LoginAsync(LoginRequestModel request);
        Task<BaseResponse<CreateAdminResponseModel>> CreateAdminAsync(CreateAdminRequestModel request);
    }
}