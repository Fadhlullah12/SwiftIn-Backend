using SwiftIn.Services.Implementation;
using SwiftIn.Services.Interface;

namespace SwiftIn.Service
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {

            services.AddScoped<ITeacherService, TeacherService>();
            // services.AddScoped<IFaceEmbeddingService, FaceEmbeddingService>();
            services.AddScoped<ICheckInService, CheckInService>();

            return services;
        }
    }
}