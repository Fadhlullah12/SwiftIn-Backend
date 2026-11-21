using SwiftIn.Dtos.RequestModels;
using SwiftIn.Dtos.ResponseModels;
using SwiftIn.Models.Entities;
using SwiftIn.Repositories;
using SwiftIn.Services.Interface;

namespace SwiftIn.Services.Implementation
{
    public class TeacherService : ITeacherService
    {
        private readonly ITeacherRepository _teachers;
        private readonly IFaceEmbeddingRepository _embeddings;

        public TeacherService(ITeacherRepository teachers, IFaceEmbeddingRepository embeddings)
        {
            _teachers = teachers;
            _embeddings = embeddings;
        }

        public async Task<BaseResponse<TeacherResponseModel>> RegisterTeacherAsync(CreateTeacherRequestModel request)
        {
            if (string.IsNullOrWhiteSpace(request.StaffNumber))
                return BaseResponse<TeacherResponseModel>.Fail("Staff number is required.");
            if (string.IsNullOrWhiteSpace(request.FullName))
                return BaseResponse<TeacherResponseModel>.Fail("Full name is required.");

            if (!request.ConsentGiven)
                return BaseResponse<TeacherResponseModel>.Fail("Biometric consent is required to register a teacher.");

            var staffNumber = request.StaffNumber.Trim();
            if (await _teachers.StaffNumberExistsAsync(staffNumber))
                return BaseResponse<TeacherResponseModel>.Fail($"A teacher with staff number '{staffNumber}' already exists.");

            var teacher = new Teacher
            {
                StaffNumber = staffNumber,
                FullName = request.FullName.Trim(),
                IsActive = true,
                ConsentGivenAt = DateTime.UtcNow
            };

            await _teachers.AddAsync(teacher);
            return BaseResponse<TeacherResponseModel>.Ok(
                TeacherResponseModel.FromEntity(teacher), "Teacher registered successfully.");
        }

        public async Task<BaseResponse<TeacherResponseModel>> UpdateTeacherAsync(string id, UpdateTeacherRequestModel request)
        {
            if (string.IsNullOrWhiteSpace(request.FullName))
                return BaseResponse<TeacherResponseModel>.Fail("Full name is required.");

            var teacher = await _teachers.GetByIdAsync(id);
            if (teacher is null)
                return BaseResponse<TeacherResponseModel>.Fail("Teacher not found.");

            teacher.FullName = request.FullName.Trim();
            await _teachers.UpdateAsync(teacher);

            return BaseResponse<TeacherResponseModel>.Ok(
                TeacherResponseModel.FromEntity(teacher), "Teacher updated successfully.");
        }

        public async Task<BaseResponse<TeacherResponseModel?>> GetTeacherByIdAsync(string id)
        {
            var teacher = await _teachers.GetByIdAsync(id);
            if (teacher is null)
                return BaseResponse<TeacherResponseModel?>.Fail("Teacher not found.");

            return BaseResponse<TeacherResponseModel?>.Ok(
                TeacherResponseModel.FromEntity(teacher), "Teacher retrieved successfully.");
        }

        public async Task<BaseResponse<List<TeacherResponseModel>>> GetAllTeachersAsync()
        {
            var teachers = await _teachers.GetAllAsync();
            var data = teachers.Select(TeacherResponseModel.FromEntity).ToList();

            return BaseResponse<List<TeacherResponseModel>>.Ok(data, "Teachers retrieved successfully.");
        }

        public async Task DeactivateTeacherAsync(string id)
        {
            var teacher = await _teachers.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Teacher not found.");

            teacher.IsActive = false;
            await _teachers.UpdateAsync(teacher);
        }

        public async Task DeleteFaceDataAsync(string id)
        {
            var teacher = await _teachers.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Teacher not found.");

            await _embeddings.DeleteByTeacherAsync(teacher.Id);
        }
    }
}