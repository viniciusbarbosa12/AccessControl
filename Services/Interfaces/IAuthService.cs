using Domain.Models;

namespace Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> RegisterAdminAsync(LoginModel model);
        Task<string?> LoginAsync(LoginModel model);
        Task<string> RegisterUserAsync(LoginModel model);

    }
}
