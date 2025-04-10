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
    public class AnuncioDestacadoRepository : IAnuncioDestacadoRepository
    {
        private readonly AgencyPlatformDbContext _context;

        public AnuncioDestacadoRepository(AgencyPlatformDbContext context)
        {
            _context = context;
        }

        public async Task<List<anuncios_destacado>> GetAllAsync()
        {
            return await _context.anuncios_destacados
                .Include(a => a.acompanante)
            .Include(a => a.cupon)
                .ToListAsync();
        }

        public async Task<anuncios_destacado?> GetByIdAsync(int id)
        {
            return await _context.anuncios_destacados
                .Include(a => a.acompanante)
                .Include(a => a.cupon)
                .FirstOrDefaultAsync(a => a.id == id);
        }

        public async Task<List<anuncios_destacado>> GetByAgenciaIdAsync(int agenciaId)
        {
            return await _context.anuncios_destacados
                .Include(a => a.acompanante)
                .Include(a => a.cupon)
                .Where(a => a.acompanante.agencia_id == agenciaId)
            .ToListAsync();
        }

        public async Task<List<anuncios_destacado>> GetByAcompananteIdAsync(int acompananteId)
        {
            return await _context.anuncios_destacados
                .Include(a => a.cupon)
                .Where(a => a.acompanante_id == acompananteId)
                .ToListAsync();
        }

        public async Task<List<anuncios_destacado>> GetActivosAsync()
        {
            return await _context.anuncios_destacados
                .Include(a => a.acompanante)
                .Include(a => a.cupon)
                    .Where(a => a.esta_activo == true && a.fecha_fin >= System.DateTime.UtcNow)
                    .ToListAsync();
        }

        public async Task AddAsync(anuncios_destacado entity)
        {
            await _context.anuncios_destacados.AddAsync(entity);
        }

        public async Task UpdateAsync(anuncios_destacado entity)
        {
            _context.anuncios_destacados.Update(entity);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(anuncios_destacado entity)
        {
            _context.anuncios_destacados.Remove(entity);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
