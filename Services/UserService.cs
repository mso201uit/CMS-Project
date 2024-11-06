using CMS_Project.Models;
using CMS_Project.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CMS_Project.Data;

namespace CMS_Project.Services
{
    public class UserService : IUserService
    {
        private readonly CMSContext _context;
        private readonly IConfiguration _configuration;

        public UserService(CMSContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<User> RegisterUserAsync(RegisterDto registerDto)
        {
            // Sjekk om brukernavn eller e-post allerede finnes
            if (await _context.Users.AnyAsync(u => u.Username == registerDto.Username))
                throw new ArgumentException("Brukernavn eksisterer allerede.");

            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
                throw new ArgumentException("E-post eksisterer allerede.");

            // Opprett ny bruker
            var user = new User
            {
                Username = registerDto.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                Email = registerDto.Email,
                CreatedDate = DateTime.UtcNow,
                Documents = new List<Document>()
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<string> AuthenticateUserAsync(LoginDto loginDto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == loginDto.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
                throw new UnauthorizedAccessException("Ugyldig brukernavn eller passord.");

            // Generer JWT-token
            var token = GenerateJwtToken(user);
            return token;
        }

        private string GenerateJwtToken(User user)
        {
            var key = _configuration["Jwt:Key"];
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            if (string.IsNullOrEmpty(key))
                throw new InvalidOperationException("JWT Key er ikke konfigurert.");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
