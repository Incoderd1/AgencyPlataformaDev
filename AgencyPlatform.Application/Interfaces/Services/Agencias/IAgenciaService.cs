using AgencyPlatform.Application.DTOs.Agencias;
using AgencyPlatform.Application.DTOs.Verificaciones;
using AgencyPlatform.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgencyPlatform.Application.DTOs.Estadisticas;
using AgencyPlatform.Application.DTOs.Anuncios;
using AgencyPlatform.Application.DTOs.Acompanantes;

namespace AgencyPlatform.Application.Interfaces.Services.Agencias
{
    public interface IAgenciaService
    {
        Task<List<AgenciaDto>> GetAllAsync();
        Task<AgenciaDto?> GetByIdAsync(int id);
        Task<AgenciaDto?> GetByUsuarioIdAsync(int usuarioId);
        Task CrearAsync(CrearAgenciaDto nuevaAgencia);
        Task ActualizarAsync(UpdateAgenciaDto agenciaActualizada);
        Task EliminarAsync(int id);

        // Gestión de acompañantes
        Task<List<AcompananteDto>> GetAcompanantesByAgenciaIdAsync(int agenciaId);
        Task AgregarAcompananteAsync(int agenciaId, int acompananteId);
        Task RemoverAcompananteAsync(int agenciaId, int acompananteId);

        // Verificación de acompañantes
        Task<VerificacionDto> VerificarAcompananteAsync(int agenciaId, int acompananteId, VerificarAcompananteDto datosVerificacion);
        Task<List<AcompananteDto>> GetAcompanantesVerificadosAsync(int agenciaId);
        Task<List<AcompananteDto>> GetAcompanantesPendientesVerificacionAsync(int agenciaId);

        // Gestión de anuncios
        Task<AnuncioDestacadoDto> CrearAnuncioDestacadoAsync(CrearAnuncioDestacadoDto anuncioDto);
        Task<List<AnuncioDestacadoDto>> GetAnunciosByAgenciaAsync(int agenciaId);

        // Estadísticas y métricas
        Task<AgenciaEstadisticasDto> GetEstadisticasAgenciaAsync(int agenciaId);

        // Comisiones y beneficios
        Task<ComisionesDto> GetComisionesByAgenciaAsync(int agenciaId, DateTime fechaInicio, DateTime fechaFin);

        // Solo para administradores
        Task<bool> VerificarAgenciaAsync(int agenciaId, bool verificada);
        Task<List<AgenciaPendienteVerificacionDto>> GetAgenciasPendientesVerificacionAsync();
    }
}
