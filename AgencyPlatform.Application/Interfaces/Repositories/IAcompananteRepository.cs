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
    }
}
