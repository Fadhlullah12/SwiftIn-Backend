using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SwiftIn.Dtos.RequestModels;
using SwiftIn.Dtos.ResponseModels;
using SwiftIn.Models.Entities;
using SwiftIn.Settings;

namespace SwiftIn.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _users;
        private readonly SignInManager<ApplicationUser> _signIn;
        private readonly RoleManager<IdentityRole> _roles;
        private readonly JwtSettings _jwt;

        public AuthService(
            UserManager<ApplicationUser> users,
            SignInManager<ApplicationUser> signIn,
            RoleManager<IdentityRole> roles,
            IOptions<JwtSettings> jwt)
        {
            _users = users;
            _signIn = signIn;
            _roles = roles;
            _jwt = jwt.Value;
        }

        public async Task<BaseResponse<LoginResponseModel>> LoginAsync(LoginRequestModel request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return BaseResponse<LoginResponseModel>.Fail("Email and password are required.");

            const string invalid = "Invalid email or password.";   // same message for both, so attackers learn nothing

            var user = await _users.FindByEmailAsync(request.Email.Trim());
            if (user is null)
                return BaseResponse<LoginResponseModel>.Fail(invalid);

            // lockoutOnFailure: too many wrong passwords locks the account for a few minutes
            var result = await _signIn.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
            if (result.IsLockedOut)
                return BaseResponse<LoginResponseModel>.Fail("Account temporarily locked after too many failed attempts. Try again later.");
            if (!result.Succeeded)
                return BaseResponse<LoginResponseModel>.Fail(invalid);

            var roles = (await _users.GetRolesAsync(user)).ToList();
            var (token, expiresAt) = CreateToken(user, roles);

            return BaseResponse<LoginResponseModel>.Ok(new LoginResponseModel
            {
                Token = token,
                ExpiresAt = expiresAt,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                Roles = roles
            }, "Login successful.");
        }

        public async Task<BaseResponse<CreateAdminResponseModel>> CreateAdminAsync(CreateAdminRequestModel request)
        {
            if (string.IsNullOrWhiteSpace(request.FullName))
                return BaseResponse<CreateAdminResponseModel>.Fail("Full name is required.");
            if (string.IsNullOrWhiteSpace(request.Email))
                return BaseResponse<CreateAdminResponseModel>.Fail("Email is required.");
            if (string.IsNullOrWhiteSpace(request.Password))
                return BaseResponse<CreateAdminResponseModel>.Fail("Password is required.");

            // Normalise the role to the exact spelling we use ("admin" -> "Admin")
            var role = AppRoles.All.FirstOrDefault(r => r.Equals(request.Role?.Trim(), StringComparison.OrdinalIgnoreCase));
            if (role is null)
                return BaseResponse<CreateAdminResponseModel>.Fail($"Role must be one of: {string.Join(", ", AppRoles.All)}.");

            var email = request.Email.Trim();
            if (await _users.FindByEmailAsync(email) is not null)
                return BaseResponse<CreateAdminResponseModel>.Fail("A user with this email already exists.");

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = request.FullName.Trim()
            };

            // Identity hashes the password itself and enforces its password rules
            var created = await _users.CreateAsync(user, request.Password);
            if (!created.Succeeded)
                return BaseResponse<CreateAdminResponseModel>.Fail(
                    "Could not create user.", created.Errors.Select(e => e.Description).ToList());

            if (!await _roles.RoleExistsAsync(role))
                await _roles.CreateAsync(new IdentityRole(role));

            await _users.AddToRoleAsync(user, role);

            return BaseResponse<CreateAdminResponseModel>.Ok(new CreateAdminResponseModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = email,
                Role = role
            }, "User created successfully.");
        }

        // ---------- helpers ----------
        private (string Token, DateTime ExpiresAt) CreateToken(ApplicationUser user, IEnumerable<string> roles)
        {
            if (string.IsNullOrWhiteSpace(_jwt.Key) || _jwt.Key.Length < 32)
                throw new InvalidOperationException("Jwt:Key must be set and at least 32 characters long.");

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Name, user.FullName),
                new(ClaimTypes.Email, user.Email ?? string.Empty),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiresAt = DateTime.UtcNow.AddMinutes(_jwt.ExpiryMinutes);

            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials);

            return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
        }
    }
}