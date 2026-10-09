namespace SwiftIn.Repositories;

public static class RepositoryRegistration
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

        services.AddScoped<ITeacherRepository, TeacherRepository>();
        services.AddScoped<IFaceEmbeddingRepository, FaceEmbeddingRepository>();
        services.AddScoped<ICheckInRepository, CheckInRepository>();

        return services;
    }
}