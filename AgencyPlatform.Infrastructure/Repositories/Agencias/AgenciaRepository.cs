using AgencyPlatform.Application.Interfaces.Repositories;
using AgencyPlatform.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgencyPlatform.Infrastructure.Repositories
{
    public class AgenciaRepository : IAgenciaRepository
    {
        private readonly AgencyPlatformDbContext _context;

        public AgenciaRepository(AgencyPlatformDbContext context)
        {
            _context = context;
        }

        public async Task<List<agencia>> GetAllAsync()
        {
            return await _context.agencias
                .Include(a => a.usuario)
                .Include(a => a.acompanantes)
                .ToListAsync();
        }

        public async Task<agencia?> GetByIdAsync(int id)
        {
            return await _context.agencias
                .Include(a => a.usuario)
                .Include(a => a.acompanantes)
                .FirstOrDefaultAsync(a => a.id == id);
        }
        public async Task<agencia?> GetAgenciaByUsuarioIdAsync(int usuarioId)
        {
            return await _context.agencias
                .FirstOrDefaultAsync(a => a.usuario_id == usuarioId);
        }

        public async Task<agencia?> GetByUsuarioIdAsync(int usuarioId)
        {
            return await _context.agencias
                .Include(a => a.usuario)
                .Include(a => a.acompanantes)
                .FirstOrDefaultAsync(a => a.usuario_id == usuarioId);
        }

        public async Task AddAsync(agencia entity)
        {
            await _context.agencias.AddAsync(entity);
        }

        public async Task UpdateAsync(agencia entity)
        {
            _context.agencias.Update(entity);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(agencia entity)
        {
            _context.agencias.Remove(entity);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        // Métodos para acceder a las vistas
        public async Task<List<vw_agencias_acompanante>> GetAgenciasAcompanantesViewAsync()
        {
            return await _context.vw_agencias_acompanantes.ToListAsync();
        }

        public async Task<vw_agencias_acompanante?> GetAgenciaAcompanantesViewByIdAsync(int agenciaId)
        {
            return await _context.vw_agencias_acompanantes
                .FirstOrDefaultAsync(a => a.agencia_id == agenciaId);
        }

        // Métodos para acompañantes de agencia
        public async Task<List<acompanante>> GetAcompanantesByAgenciaIdAsync(int agenciaId)
        {
            return await _context.acompanantes
                .Include(a => a.usuario)
                .Include(a => a.fotos)
                .Include(a => a.servicios)
                .Include(a => a.acompanante_categoria)
                    .ThenInclude(ac => ac.categoria)
                .Where(a => a.agencia_id == agenciaId)
                .ToListAsync();
        }

        public async Task<List<acompanante>> GetAcompanantesVerificadosByAgenciaIdAsync(int agenciaId)
        {
            return await _context.acompanantes
                .Include(a => a.usuario)
                .Include(a => a.fotos)
                .Include(a => a.servicios)
                .Include(a => a.acompanante_categoria)
                    .ThenInclude(ac => ac.categoria)
                .Where(a => a.agencia_id == agenciaId && a.esta_verificado == true)
                .ToListAsync();
        }

        // Métodos para comisiones
        public async Task<decimal> GetComisionPorcentajeByAgenciaIdAsync(int agenciaId)
        {
            var agencia = await _context.agencias
                .Where(a => a.id == agenciaId)
                .Select(a => a.comision_porcentaje)
                .FirstOrDefaultAsync();

            return agencia ?? 0;
        }

        public async Task UpdateComisionPorcentajeAsync(int agenciaId, decimal porcentaje)
        {
            var agencia = await _context.agencias
                .FirstOrDefaultAsync(a => a.id == agenciaId);

            if (agencia != null)
            {
                agencia.comision_porcentaje = porcentaje;
                _context.agencias.Update(agencia);
                await _context.SaveChangesAsync();
            }
        }

        // Método para agencias pendientes de verificación
        public async Task<List<agencia>> GetAgenciasPendientesVerificacionAsync()
        {
            return await _context.agencias
                .Include(a => a.usuario)
                .Include(a => a.acompanantes)
                .Where(a => a.esta_verificada == false)
                .ToListAsync();
        }

        // Implementación de métodos para verificaciones
        public async Task<verificacione?> GetVerificacionByAcompananteIdAsync(int acompananteId)
        {
            return await _context.verificaciones
                .Include(v => v.agencia)
                .FirstOrDefaultAsync(v => v.acompanante_id == acompananteId);
        }

        public async Task AddVerificacionAsync(verificacione entity)
        {
            await _context.verificaciones.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        // Implementación de métodos para anuncios destacados
        public async Task<List<anuncios_destacado>> GetAnunciosDestacadosByAgenciaIdAsync(int agenciaId)
        {
            return await _context.anuncios_destacados
                .Include(a => a.acompanante)
                .Include(a => a.cupon)
                .Where(a => a.acompanante.agencia_id == agenciaId)
                .ToListAsync();
        }

        public async Task AddAnuncioDestacadoAsync( anuncios_destacado entity)
        {
            await _context.anuncios_destacados.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        // Métodos adicionales que podrían ser necesarios
        public async Task<List<verificacione>> GetVerificacionesByAgenciaIdAsync(int agenciaId)
        {
            return await _context.verificaciones
                .Include(v => v.acompanante)
                .Where(v => v.agencia_id == agenciaId)
                .ToListAsync();
        }

        public async Task<List<verificacione>> GetVerificacionesByAgenciaIdAndPeriodoAsync(int agenciaId, DateTime fechaInicio, DateTime fechaFin)
        {
            return await _context.verificaciones
                .Include(v => v.acompanante)
                .Where(v => v.agencia_id == agenciaId &&
                       v.fecha_verificacion >= fechaInicio &&
                       v.fecha_verificacion <= fechaFin)
                .ToListAsync();
        }

        public async Task DeleteVerificacionAsync(verificacione entity)
        {
            _context.verificaciones.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteVerificacionesByAgenciaIdAsync(int agenciaId)
        {
            var verificaciones = await _context.verificaciones
                .Where(v => v.agencia_id == agenciaId)
                .ToListAsync();

            if (verificaciones.Any())
            {
                _context.verificaciones.RemoveRange(verificaciones);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<bool> ExisteSolicitudPendienteAsync(int acompananteId, int agenciaId)
        {
            return await _context.SolicitudAgencias.AnyAsync(s =>
                s.AcompananteId == acompananteId &&
                s.AgenciaId == agenciaId &&
                s.Estado == "pendiente");
        }

        public async Task CrearSolicitudAsync(SolicitudAgencia solicitud)
        {
            await _context.SolicitudAgencias.AddAsync(solicitud);
        }

        public async Task<List<SolicitudAgencia>> GetSolicitudesPendientesPorAgenciaAsync(int agenciaId)
        {
            return await _context.SolicitudAgencias
                .Where(s => s.AgenciaId == agenciaId && s.Estado == "pendiente")
                .Include(s => s.Acompanante)
                .ToListAsync();
        }

        public async Task<SolicitudAgencia?> GetSolicitudByIdAsync(int solicitudId)
        {
            return await _context.SolicitudAgencias
                .Include(s => s.Acompanante)
                .FirstOrDefaultAsync(s => s.Id == solicitudId);
        }
        public async Task UpdateSolicitudAsync(SolicitudAgencia solicitud)
        {
            _context.SolicitudAgencias.Update(solicitud);
        }

    }
}