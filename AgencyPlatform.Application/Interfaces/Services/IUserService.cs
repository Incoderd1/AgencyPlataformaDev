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


        //Acompanantes

        // Nuevo método para registro combinado de usuario y acompañante
        Task<(usuario Usuario, int AcompananteId)> RegisterUserAcompananteAsync(
            string email,
            string password,
            string? phone,
            string nombrePerfil,
            string genero,
            int edad,
            string? descripcion = null,
            int? altura = null,
            int? peso = null,
            string? ciudad = null,
            string? pais = null,
            string? idiomas = null,
            string? disponibilidad = null,
            decimal? tarifaBase = null,
            string? moneda = "USD",
            List<int>? categoriaIds = null,
            string? telefono = null,
            string? whatsapp = null,
            string? emailContacto = null);
    }
}
