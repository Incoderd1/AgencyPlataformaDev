using AgencyPlatform.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencyPlatform.Application.Interfaces.Repositories
{
    public interface IVisitaRepository
    {
        Task<List<visitas_perfil>> GetAllAsync();
        Task<visitas_perfil?> GetByIdAsync(int id);
        Task<List<visitas_perfil>> GetByAcompananteIdAsync(int acompananteId);
        Task<visitas_perfil?> GetVisitaRecienteAsync(int acompananteId, string? ipVisitante);
        Task<long> ContarVisitasTotalesAsync(int acompananteId);
        Task<long> ContarVisitasRecientesAsync(int acompananteId, int dias);
        Task AddAsync(visitas_perfil entity);
        Task UpdateAsync(visitas_perfil entity);
        Task DeleteAsync(visitas_perfil entity);
        Task SaveChangesAsync();
    }
}
