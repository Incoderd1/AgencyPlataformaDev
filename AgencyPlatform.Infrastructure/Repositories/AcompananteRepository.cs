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

        public async Task ActualizarScoreActividadAsync(int acompananteId, long scoreActividad)
        {
            var acompanante = await _context.acompanantes.FindAsync(acompananteId);
            if (acompanante != null)
            {
                acompanante.score_actividad = scoreActividad;
                await _context.SaveChangesAsync();
            }
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

        public async Task<List<acompanante>> GetDestacadosAsync()
        {
            var acompanantes = await _context.acompanantes
                .Include(a => a.usuario)
                .Include(a => a.fotos)
                .Include(a => a.anuncios_destacados)
                .Where(a => a.anuncios_destacados.Any(ad => ad.esta_activo == true && ad.fecha_fin > DateTime.Now))
                .Include(a => a.acompanante_categoria)
                    .ThenInclude(ac => ac.categoria)
                .ToListAsync();

            // Filtrar manualmente los anuncios destacados
            foreach (var acompanante in acompanantes)
            {
                acompanante.anuncios_destacados = acompanante.anuncios_destacados
                    .Where(ad => ad.esta_activo == true)
                    .ToList();
            }

            return acompanantes;
        }

        public async Task<List<acompanante>> GetRecientesAsync(int cantidad)
        {
            return await _context.acompanantes
                .Include(a => a.usuario)
                .Include(a => a.fotos)
                .Include(a => a.acompanante_categoria)
                    .ThenInclude(ac => ac.categoria)
                .Where(a => a.esta_disponible == true)
                .OrderByDescending(a => a.created_at)
                .Take(cantidad)
                .ToListAsync();
        }

        public async Task<List<acompanante>> GetPopularesAsync(int cantidad)
        {
            return await _context.acompanantes
                .Include(a => a.usuario)
                .Include(a => a.fotos)
                .Include(a => a.acompanante_categoria)
                    .ThenInclude(ac => ac.categoria)
                .Where(a => a.esta_disponible == true)
                .OrderByDescending(a => a.score_actividad)
                .Take(cantidad)
                .ToListAsync();
        }

        public async Task<List<acompanante>> BuscarAsync(string? busqueda, string? ciudad, string? pais, string? genero,
            int? edadMinima, int? edadMaxima, decimal? tarifaMinima, decimal? tarifaMaxima,
            bool? soloVerificados, bool? soloDisponibles, List<int>? categoriaIds,
            string? ordenarPor, int pagina, int elementosPorPagina)
        {
            // Iniciar con todos los acompañantes
            var query = _context.acompanantes
                .Include(a => a.usuario)
                .Include(a => a.fotos)
                .Include(a => a.servicios)
                .Include(a => a.acompanante_categoria)
                    .ThenInclude(ac => ac.categoria)
                .AsQueryable();

            // Aplicar filtros
            if (!string.IsNullOrEmpty(busqueda))
            {
                query = query.Where(a => a.nombre_perfil.Contains(busqueda) ||
                                        a.descripcion!.Contains(busqueda));
            }

            if (!string.IsNullOrEmpty(ciudad))
            {
                query = query.Where(a => a.ciudad!.ToLower() == ciudad.ToLower());
            }

            if (!string.IsNullOrEmpty(pais))
            {
                query = query.Where(a => a.pais!.ToLower() == pais.ToLower());
            }

            if (!string.IsNullOrEmpty(genero))
            {
                query = query.Where(a => a.genero!.ToLower() == genero.ToLower());
            }

            if (edadMinima.HasValue)
            {
                query = query.Where(a => a.edad >= edadMinima.Value);
            }

            if (edadMaxima.HasValue)
            {
                query = query.Where(a => a.edad <= edadMaxima.Value);
            }

            if (tarifaMinima.HasValue)
            {
                query = query.Where(a => a.tarifa_base >= tarifaMinima.Value);
            }

            if (tarifaMaxima.HasValue)
            {
                query = query.Where(a => a.tarifa_base <= tarifaMaxima.Value);
            }

            if (soloVerificados.HasValue && soloVerificados.Value)
            {
                query = query.Where(a => a.esta_verificado == true);
            }

            if (soloDisponibles.HasValue && soloDisponibles.Value)
            {
                query = query.Where(a => a.esta_disponible == true);
            }

            if (categoriaIds != null && categoriaIds.Any())
            {
                query = query.Where(a => a.acompanante_categoria.Any(ac => categoriaIds.Contains(ac.categoria_id)));
            }

            // Aplicar ordenamiento
            query = ordenarPor?.ToLower() switch
            {
                "precio_asc" => query.OrderBy(a => a.tarifa_base),
                "precio_desc" => query.OrderByDescending(a => a.tarifa_base),
                "edad_asc" => query.OrderBy(a => a.edad),
                "edad_desc" => query.OrderByDescending(a => a.edad),
                "popularidad" => query.OrderByDescending(a => a.score_actividad),
                _ => query.OrderByDescending(a => a.created_at) // Por defecto, los más recientes
            };

            // Aplicar paginación
            query = query.Skip((pagina - 1) * elementosPorPagina).Take(elementosPorPagina);

            return await query.ToListAsync();
        }

        public async Task<List<acompanante_categoria>> GetCategoriasByAcompananteIdAsync(int acompananteId)
        {
            return await _context.acompanante_categorias
                .Include(ac => ac.categoria)
                .Where(ac => ac.acompanante_id == acompananteId)
                .ToListAsync();
        }

        public async Task<bool> TieneCategoriaAsync(int acompananteId, int categoriaId)
        {
            return await _context.acompanante_categorias
                .AnyAsync(ac => ac.acompanante_id == acompananteId && ac.categoria_id == categoriaId);
        }

        public async Task AgregarCategoriaAsync(int acompananteId, int categoriaId)
        {
            // Verificar si la relación ya existe
            if (!await TieneCategoriaAsync(acompananteId, categoriaId))
            {
                // Crear nueva relación
                var nuevaCategoria = new acompanante_categoria
                {
                    acompanante_id = acompananteId,
                    categoria_id = categoriaId,
                    created_at = DateTime.Now
                };

                await _context.acompanante_categorias.AddAsync(nuevaCategoria);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<bool> TieneAcompanantesAsync(int categoriaId)
        {
            return await _context.acompanante_categorias
                .AnyAsync(ac => ac.categoria_id == categoriaId);
        }

        public async Task EliminarCategoriaAsync(int acompananteId, int categoriaId)
        {
            var relacion = await _context.acompanante_categorias
                .FirstOrDefaultAsync(ac => ac.acompanante_id == acompananteId && ac.categoria_id == categoriaId);

            if (relacion != null)
            {
                _context.acompanante_categorias.Remove(relacion);
                await _context.SaveChangesAsync();
            }
        }
    }
}