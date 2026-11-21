using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SwiftIn.Context;
using SwiftIn.Models.Entities;
using SwiftIn.Repositories;
using SwiftIn.Service;
using SwiftIn.Services;
using SwiftIn.Settings;

var builder = WebApplication.CreateBuilder(args);

// ---------- Database (MySQL) ----------
var connectionString = builder.Configuration.GetConnectionString("ConnectionString")
    ?? throw new InvalidOperationException("ConnectionStrings:ConnectionString is missing from appsettings.json.");

builder.Services.AddDbContext<SwiftInContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// ---------- Identity (users, roles, password hashing) ----------
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequiredLength = 8;
})
.AddEntityFrameworkStores<SwiftInContext>()
.AddDefaultTokenProviders();

// ---------- Settings classes (read from appsettings.json) ----------
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<CheckInSettings>(builder.Configuration.GetSection("CheckIn"));

// ---------- JWT authentication ----------
// AddIdentity makes cookies the default login method, so we switch the defaults to JWT here.
var jwt = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()
    ?? throw new InvalidOperationException("The \"Jwt\" section is missing from appsettings.json.");

if (string.IsNullOrWhiteSpace(jwt.Key) || jwt.Key.Length < 32)
    throw new InvalidOperationException("Jwt:Key must be set and at least 32 characters long.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwt.Issuer,
        ValidateAudience = true,
        ValidAudience = jwt.Audience,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
        ClockSkew = TimeSpan.FromMinutes(1)
    };
});

builder.Services.AddAuthorization();

// ---------- Our own repositories and services ----------
builder.Services.AddRepositories();
builder.Services.AddServices();

// ---------- Controllers ----------
builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        // Send enums as text ("NeedsReview") instead of numbers (1)
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddOpenApi();   // keep this if your template already had it

var app = builder.Build();

// Creates the roles and the first admin (see SeedAdmin in appsettings.json)
await IdentitySeeder.SeedAsync(app.Services, app.Configuration);

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();   
app.UseAuthorization();    

app.MapControllers();

app.Run();