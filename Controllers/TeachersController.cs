using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiftIn.Dtos.RequestModels;
using SwiftIn.Dtos.ResponseModels;
using SwiftIn.Services;
using SwiftIn.Services.Interface;
using SwiftIn.Settings;

namespace SwiftIn.Controllers
{
    [Authorize]   // any logged-in user (Admin or Reviewer) can read; writes are Admin only
    public class TeachersController : ApiControllerBase
    {
        private readonly ITeacherService _teachers;

        public TeachersController(ITeacherService teachers)
        {
            _teachers = teachers;
        }

        // GET api/teachers
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return FromResponse(await _teachers.GetAllTeachersAsync());
        }

        // GET api/teachers/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(string id)
        {
            var response = await _teachers.GetTeacherByIdAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        // POST api/teachers
        [HttpPost]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> Register([FromBody] CreateTeacherRequestModel request)
        {
            var response = await _teachers.RegisterTeacherAsync(request);
            if (!response.Success)
                return BadRequest(response);

            return CreatedAtAction(nameof(GetById), new { id = response.Data!.Id }, response);
        }

        // PUT api/teachers/{id}
        [HttpPut("{id:guid}")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateTeacherRequestModel request)
        {
            return FromResponse(await _teachers.UpdateTeacherAsync(id, request));
        }

        // POST api/teachers/{id}/deactivate
        [HttpPost("{id:guid}/deactivate")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> Deactivate(string id)
        {
            try
            {
                await _teachers.DeactivateTeacherAsync(id);
                return Ok(BaseResponse<bool>.Ok(true, "Teacher deactivated."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(BaseResponse<bool>.Fail(ex.Message));
            }
        }

        // DELETE api/teachers/{id}/face-data
        [HttpDelete("{id:guid}/face-data")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> DeleteFaceData(string id)
        {
            try
            {
                await _teachers.DeleteFaceDataAsync(id);
                return Ok(BaseResponse<bool>.Ok(true, "Face data deleted."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(BaseResponse<bool>.Fail(ex.Message));
            }
        }
    }
}