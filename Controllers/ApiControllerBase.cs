using Microsoft.AspNetCore.Mvc;
using SwiftIn.Dtos.ResponseModels;

namespace SwiftIn.Controllers
{
    // Shared by every controller: sets up the route and turns a BaseResponse into an HTTP result.
    [ApiController]
    [Route("api/[controller]")]
    public abstract class ApiControllerBase : ControllerBase
    {
        // Success -> 200, failure -> 400, with the same BaseResponse body either way
        protected IActionResult FromResponse<T>(BaseResponse<T> response) =>
            response.Success ? Ok(response) : BadRequest(response);

        // The logged-in user's full name (comes from the token's name claim)
        protected string CurrentUserName => User.Identity?.Name ?? "unknown";
    }
}