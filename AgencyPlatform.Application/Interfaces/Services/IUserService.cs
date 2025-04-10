using AgencyPlatform.Core.Entities;
using AgencyPlatform.Core.Enums;

namespace AgencyPlatform.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<usuario> RegisterUserAsync(string email, string password, string tipoUsuario, string? phone = null);
        Task<(List<usuario> Usuarios, int Total)> GetAllUsersPagedAsync(int pagina, int elementosPorPagina); // Nuevo método

        Task<(string AccessToken, string RefreshToken)> LoginUserAsync(string email, string password, string ipAddress, string userAgent);

        Task<string> RefreshTokenAsync(string refreshToken, string ipAddress, string userAgent);

        Task RequestPasswordResetAsync(string email);
        Task ResetPasswordAsync(string token, string newPassword);

        Task<usuario> GetUserByIdAsync(int id);
        Task<List<usuario>> GetAllUsersAsync();
        Task UpdateUserAsync(int id, string email, string? phone, bool estaActivo);
        Task DeleteUserAsync(int id);
    }
}
