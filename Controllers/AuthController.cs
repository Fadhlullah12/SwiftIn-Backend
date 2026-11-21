using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiftIn.Dtos.RequestModels;
using SwiftIn.Services;
using SwiftIn.Settings;

namespace SwiftIn.Controllers
{
    public class AuthController : ApiControllerBase
    {
        private readonly IAuthService _auth;

        public AuthController(IAuthService auth)
        {
            _auth = auth;
        }

        // POST api/auth/login   (public: this is how you get a token)
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestModel request)
        {
            var response = await _auth.LoginAsync(request);
            return response.Success ? Ok(response) : Unauthorized(response);
        }

        // POST api/auth/users   (Admin only: create another Admin or Reviewer)
        [HttpPost("users")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> CreateUser([FromBody] CreateAdminRequestModel request)
        {
            return FromResponse(await _auth.CreateAdminAsync(request));
        }
    }
}