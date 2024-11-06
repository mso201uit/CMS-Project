using CMS_Project.Models;
using CMS_Project.Models.DTOs;
using System.Threading.Tasks;

namespace CMS_Project.Services
{
    public interface IUserService
    {
        Task<User> RegisterUserAsync(RegisterDto registerDto);
        Task<string> AuthenticateUserAsync(LoginDto loginDto);
    }
}