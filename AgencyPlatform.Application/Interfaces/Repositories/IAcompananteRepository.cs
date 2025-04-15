using AgencyPlatform.Application.DTOs.Estadisticas;
using AgencyPlatform.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencyPlatform.Application.Interfaces.Repositories
{
    public interface IAcompananteRepository
    {
        Task<List<acompanante>> GetAllAsync();
        Task<acompanante?> GetByIdAsync(int id);
        Task<acompanante?> GetByUsuarioIdAsync(int usuarioId);
        Task AddAsync(acompanante entity);
        Task UpdateAsync(acompanante entity);
        Task DeleteAsync(acompanante entity);
        Task SaveChangesAsync();
        Task<List<acompanante>> GetDestacadosAsync();
        Task<List<acompanante>> GetRecientesAsync(int cantidad);
        Task<List<acompanante>> GetPopularesAsync(int cantidad);

        // Métodos para búsqueda y filtrado
        Task<List<acompanante>> BuscarAsync(string? busqueda, string? ciudad, string? pais, string? genero,
            int? edadMinima, int? edadMaxima, decimal? tarifaMinima, decimal? tarifaMaxima,
            bool? soloVerificados, bool? soloDisponibles, List<int>? categoriaIds,
            string? ordenarPor, int pagina, int elementosPorPagina);

        // Métodos para categorías
        Task<List<acompanante_categoria>> GetCategoriasByAcompananteIdAsync(int acompananteId);
        Task<bool> TieneCategoriaAsync(int acompananteId, int categoriaId);
        Task AgregarCategoriaAsync(int acompananteId, int categoriaId);
        Task EliminarCategoriaAsync(int acompananteId, int categoriaId);
        Task ActualizarScoreActividadAsync(int acompananteId, long scoreActividad);
        Task<bool> TieneAcompanantesAsync(int categoriaId);
        Task<PerfilEstadisticasDto?> GetEstadisticasPerfilAsync(int acompananteId);



        Task<int> CountByAgenciaIdAsync(int agenciaId);
        Task<int> CountVerificadosByAgenciaIdAsync(int agenciaId);
        Task<List<acompanante>> GetDestacadosByAgenciaIdAsync(int agenciaId, int limit = 5);

        Task<PaginatedResult<acompanante>> GetIndependientesAsync(
                int pageNumber,
                int pageSize,
                string filterBy,
                string sortBy,
                bool sortDesc);


    }
}
