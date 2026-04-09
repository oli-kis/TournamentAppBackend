using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TournamentAppBackend.DTO.User;
using TournamentAppBackend.Model;

namespace TournamentAppBackend.Services.User
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _configuration;

        public UserService(AppDbContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        public async Task<UserResponseDTO> RegisterRefereeAsync(RegisterRefereeDTO dto)
        {
            var exists = await _db.Users.AnyAsync(u => u.Email == dto.Email);
            if (exists)
                throw new Exception("Email already exists");

            var user = new Model.UserModel
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                Name = dto.Name,
                Role = "REFEREE",
                Status = "ACTIVE",
                PasswordHash = HashPassword(dto.Password)
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return Map(user);
        }

        public async Task<LoginResponseDTO> LoginAsync(LoginDTO dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null || !VerifyPassword(dto.Password, user.PasswordHash))
                throw new Exception("Invalid credentials");

            var token = GenerateJwtToken(user);

            return new LoginResponseDTO
            {
                AccessToken = token,
                User = Map(user)
            };
        }

        public async Task<UserResponseDTO> GetByIdAsync(Guid id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null)
                throw new Exception("User not found");

            return Map(user);
        }

        public async Task<List<UserResponseDTO>> GetAllRefereesAsync()
        {
            var users = await _db.Users
                .Where(u => u.Role == "REFEREE")
                .ToListAsync();

            return users.Select(Map).ToList();
        }

        public async Task<List<UserResponseDTO>> GetAllActiveRefereesAsync()
        {
            var users = await _db.Users
                .Where(u => u.Role == "REFEREE" && u.Status == "ACTIVE")
                .ToListAsync();

            return users.Select(Map).ToList();
        }

        public async Task<UserResponseDTO> UpdateAsync(Guid id, UpdateUserDTO dto)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null)
                throw new Exception("User not found");

            if (!string.IsNullOrWhiteSpace(dto.Name))
                user.Name = dto.Name;

            if (!string.IsNullOrWhiteSpace(dto.Status))
                user.Status = dto.Status;

            await _db.SaveChangesAsync();

            return Map(user);
        }

        public async Task DeleteAsync(Guid id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null)
                throw new Exception("User not found");

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
        }

        public async Task LogoutAsync(Guid id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null)
                throw new Exception("User not found");

            user.Status = "INACTIVE";
            await _db.SaveChangesAsync();
        }

        private string GenerateJwtToken(Model.UserModel user)
        {
            var key = Environment.GetEnvironmentVariable("JWT_KEY")!;
            var issuer = _configuration["Jwt:Issuer"]!;
            var audience = _configuration["Jwt:Audience"]!;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("name", user.Name)
            };

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(12),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        private bool VerifyPassword(string password, string hash)
        {
            return HashPassword(password) == hash;
        }

        private static UserResponseDTO Map(Model.UserModel u)
        {
            return new UserResponseDTO
            {
                Id = u.Id,
                Email = u.Email,
                Name = u.Name,
                Role = u.Role,
                Status = u.Status
            };
        }
    }
}