using AgencyPlatform.Application.Interfaces.Repositories;
using AgencyPlatform.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AgencyPlatform.Infrastructure.Repositories
{
    public class AcompananteRepository : IAcompananteRepository
    {
        private readonly AgencyPlatformDbContext _context;

        public AcompananteRepository(AgencyPlatformDbContext context)
        {
            _context = context;
        }

        public async Task<List<acompanante>> GetAllAsync()
        {
            return await _context.acompanantes
                .Include(a => a.usuario)
                .Include(a => a.fotos)
                .Include(a => a.servicios)
                .Include(a => a.acompanante_categoria)
                    .ThenInclude(ac => ac.categoria)
                .ToListAsync();
        }

        public async Task<acompanante?> GetByIdAsync(int id)
        {
            return await _context.acompanantes
                .Include(a => a.usuario)
                .Include(a => a.fotos)
                .Include(a => a.servicios)
                .Include(a => a.acompanante_categoria)
                    .ThenInclude(ac => ac.categoria)
                .FirstOrDefaultAsync(a => a.id == id);
        }

        public async Task<acompanante?> GetByUsuarioIdAsync(int usuarioId)
        {
            return await _context.acompanantes
                .Include(a => a.usuario)
                .Include(a => a.fotos)
                .Include(a => a.servicios)
                .Include(a => a.acompanante_categoria)
                    .ThenInclude(ac => ac.categoria)
                .FirstOrDefaultAsync(a => a.usuario_id == usuarioId);
        }

        public async Task AddAsync(acompanante entity)
        {
            await _context.acompanantes.AddAsync(entity);
        }

        public async Task UpdateAsync(acompanante entity)
        {
            _context.acompanantes.Update(entity);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(acompanante entity)
        {
            _context.acompanantes.Remove(entity);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
