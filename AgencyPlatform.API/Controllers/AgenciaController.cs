using AgencyPlatform.Application.DTOs.Acompanantes;
using AgencyPlatform.Application.DTOs.Agencias;
using AgencyPlatform.Application.DTOs.Anuncios;
using AgencyPlatform.Application.DTOs.Estadisticas;
using AgencyPlatform.Application.DTOs.Verificaciones;
using AgencyPlatform.Application.Interfaces.Services.Agencias;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AgencyPlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class AgenciaController : ControllerBase
    {
        private readonly IAgenciaService _agenciaService;

        public AgenciaController(IAgenciaService agenciaService)
        {
            _agenciaService = agenciaService;
        }

        // 🔹 CRUD
        [HttpGet]
        public async Task<ActionResult<List<AgenciaDto>>> GetAll()
            => Ok(await _agenciaService.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<AgenciaDto>> GetById(int id)
        {
            var result = await _agenciaService.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpGet("usuario")]
        public async Task<ActionResult<AgenciaDto>> GetByUsuarioActual()
            => Ok(await _agenciaService.GetByUsuarioIdAsync(GetUsuarioId()));

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CrearAgenciaDto dto)
        {
            await _agenciaService.CrearAsync(dto);
            return Ok(new { mensaje = "Agencia creada correctamente" });
        }

        [HttpPut]
        public async Task<IActionResult> Actualizar([FromBody] UpdateAgenciaDto dto)
        {
            await _agenciaService.ActualizarAsync(dto);
            return Ok(new { mensaje = "Agencia actualizada correctamente" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            await _agenciaService.EliminarAsync(id);
            return Ok(new { mensaje = "Agencia eliminada correctamente" });
        }

        // 🔹 Acompañantes
        [HttpGet("{agenciaId}/acompanantes")]
        public async Task<ActionResult<List<AcompananteDto>>> GetAcompanantes(int agenciaId)
            => Ok(await _agenciaService.GetAcompanantesByAgenciaIdAsync(agenciaId));

        [HttpPost("{agenciaId}/acompanantes/{acompananteId}")]
        public async Task<IActionResult> AgregarAcompanante(int agenciaId, int acompananteId)
        {
            await _agenciaService.AgregarAcompananteAsync(agenciaId, acompananteId);
            return Ok("Acompañante asignado correctamente");
        }

        [HttpDelete("{agenciaId}/acompanantes/{acompananteId}")]
        public async Task<IActionResult> RemoverAcompanante(int agenciaId, int acompananteId)
        {
            await _agenciaService.RemoverAcompananteAsync(agenciaId, acompananteId);
            return Ok("Acompañante removido correctamente");
        }

        // 🔹 Verificaciones
        [HttpPost("{agenciaId}/verificar/{acompananteId}")]
        public async Task<ActionResult<VerificacionDto>> VerificarAcompanante(int agenciaId, int acompananteId, [FromBody] VerificarAcompananteDto dto)
            => Ok(await _agenciaService.VerificarAcompananteAsync(agenciaId, acompananteId, dto));

        [HttpGet("{agenciaId}/verificados")]
        public async Task<ActionResult<List<AcompananteDto>>> GetVerificados(int agenciaId)
            => Ok(await _agenciaService.GetAcompanantesVerificadosAsync(agenciaId));

        [HttpGet("{agenciaId}/pendientes-verificacion")]
        public async Task<ActionResult<List<AcompananteDto>>> GetPendientesVerificacion(int agenciaId)
            => Ok(await _agenciaService.GetAcompanantesPendientesVerificacionAsync(agenciaId));

        // 🔹 Anuncios destacados
        [HttpPost("anuncios")]
        public async Task<ActionResult<AnuncioDestacadoDto>> CrearAnuncio([FromBody] CrearAnuncioDestacadoDto dto)
            => Ok(await _agenciaService.CrearAnuncioDestacadoAsync(dto));

        [HttpGet("{agenciaId}/anuncios")]
        public async Task<ActionResult<List<AnuncioDestacadoDto>>> GetAnuncios(int agenciaId)
            => Ok(await _agenciaService.GetAnunciosByAgenciaAsync(agenciaId));

        // 🔹 Estadísticas
        [HttpGet("{agenciaId}/estadisticas")]
        public async Task<ActionResult<AgenciaEstadisticasDto>> GetEstadisticas(int agenciaId)
            => Ok(await _agenciaService.GetEstadisticasAgenciaAsync(agenciaId));

        [HttpGet("{agenciaId}/comisiones")]
        public async Task<ActionResult<ComisionesDto>> GetComisiones(
            int agenciaId,
            [FromQuery] DateTime fechaInicio,
            [FromQuery] DateTime fechaFin)
            => Ok(await _agenciaService.GetComisionesByAgenciaAsync(agenciaId, fechaInicio, fechaFin));

        // 🔹 Admin
        [HttpPut("{agenciaId}/verificar")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> VerificarAgencia(int agenciaId, [FromQuery] bool verificada)
        {
            var result = await _agenciaService.VerificarAgenciaAsync(agenciaId, verificada);
            return Ok(new { mensaje = result ? "Agencia verificada" : "Agencia desverificada" });
        }

        [HttpGet("pendientes-verificacion")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<List<AgenciaPendienteVerificacionDto>>> GetPendientes()
            => Ok(await _agenciaService.GetAgenciasPendientesVerificacionAsync());


        [HttpGet("solicitudes")]
        //[Authorize(Roles = "agencia")]
        public async Task<IActionResult> GetSolicitudesPendientes()
        {
            var solicitudes = await _agenciaService.GetSolicitudesPendientesAsync();
            return Ok(solicitudes);
        }

        [HttpPut("solicitudes/{solicitudId}/aprobar")]
        public async Task<IActionResult> AprobarSolicitud(int solicitudId)
        {
            await _agenciaService.AprobarSolicitudAsync(solicitudId);
            return Ok(new { mensaje = "Solicitud aprobada y acompañante asignado a la agencia." });
        }

        [HttpPost("solicitudes/{id}/rechazar")]
        //[Authorize(Roles = "agencia")]
        public async Task<IActionResult> RechazarSolicitud(int id)
        {
            await _agenciaService.RechazarSolicitudAsync(id);
            return Ok(new { mensaje = "Solicitud rechazada correctamente" });
        }
       
        [HttpGet("estadisticas/perfil/{acompananteId}")]
        public async Task<IActionResult> GetEstadisticasPorPerfil(int acompananteId)
        {
            var estadisticas = await _agenciaService.GetEstadisticasPerfilAsync(acompananteId);
            return Ok(estadisticas);
        }



        // 🔐 Utilidad
        private int GetUsuarioId()
        {
            var idClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(idClaim, out int id) ? id : throw new UnauthorizedAccessException("Usuario no autenticado");
        }
    }
}
