using AgencyPlatform.Application.DTOs.Agencias;
using AgencyPlatform.Application.DTOs.Acompanantes;
using AgencyPlatform.Application.DTOs.Verificaciones;
using AgencyPlatform.Application.DTOs.Anuncios;
using AgencyPlatform.Application.DTOs.Estadisticas;
using AgencyPlatform.Application.Interfaces.Repositories;
using AgencyPlatform.Application.Interfaces.Services.Agencias;
using AgencyPlatform.Core.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AgencyPlatform.Infrastructure.Services.Agencias
{
    public class AgenciaService : IAgenciaService
    {
        private readonly IAgenciaRepository _agenciaRepository;
        private readonly IAcompananteRepository _acompananteRepository;
        private readonly IVerificacionRepository _verificacionRepository;
        private readonly IAnuncioDestacadoRepository _anuncioRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public AgenciaService(
            IAgenciaRepository agenciaRepository,
            IAcompananteRepository acompananteRepository,
            IVerificacionRepository verificacionRepository,
            IAnuncioDestacadoRepository anuncioRepository,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _agenciaRepository = agenciaRepository;
            _acompananteRepository = acompananteRepository;
            _verificacionRepository = verificacionRepository;
            _anuncioRepository = anuncioRepository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        // Métodos CRUD básicos
        public async Task<List<AgenciaDto>> GetAllAsync()
        {
            var entidades = await _agenciaRepository.GetAllAsync();
            return _mapper.Map<List<AgenciaDto>>(entidades);
        }

        public async Task<AgenciaDto?> GetByIdAsync(int id)
        {
            var entidad = await _agenciaRepository.GetByIdAsync(id);
            return entidad == null ? null : _mapper.Map<AgenciaDto>(entidad);
        }

        public async Task<AgenciaDto?> GetByUsuarioIdAsync(int usuarioId)
        {
            var entidad = await _agenciaRepository.GetByUsuarioIdAsync(usuarioId);
            return entidad == null ? null : _mapper.Map<AgenciaDto>(entidad);
        }

        public async Task CrearAsync(CrearAgenciaDto nuevaAgenciaDto)
        {
            var nueva = _mapper.Map<agencia>(nuevaAgenciaDto);
            nueva.usuario_id = ObtenerUsuarioId();
            nueva.esta_verificada = false; // Por defecto, las agencias no están verificadas

            await _agenciaRepository.AddAsync(nueva);
            await _agenciaRepository.SaveChangesAsync();
        }

        public async Task ActualizarAsync(UpdateAgenciaDto agenciaDto)
        {
            var usuarioId = ObtenerUsuarioId();
            var actual = await _agenciaRepository.GetByUsuarioIdAsync(usuarioId);

            if (actual == null || actual.id != agenciaDto.Id)
                throw new UnauthorizedAccessException("No tienes permisos para editar esta agencia.");

            // Actualizar propiedades permitidas
            actual.nombre = agenciaDto.Nombre;
            actual.descripcion = agenciaDto.Descripcion;
            actual.logo_url = agenciaDto.LogoUrl;
            actual.sitio_web = agenciaDto.SitioWeb;
            actual.direccion = agenciaDto.Direccion;
            actual.ciudad = agenciaDto.Ciudad;
            actual.pais = agenciaDto.Pais;

            await _agenciaRepository.UpdateAsync(actual);
            await _agenciaRepository.SaveChangesAsync();
        }

        public async Task EliminarAsync(int id)
        {
            var entidad = await _agenciaRepository.GetByIdAsync(id);

            if (entidad == null)
                throw new Exception("Agencia no encontrada.");

            if (entidad.usuario_id != ObtenerUsuarioId() && !EsAdmin())
                throw new UnauthorizedAccessException("No tienes permisos para eliminar esta agencia.");

            await _agenciaRepository.DeleteAsync(entidad);
            await _agenciaRepository.SaveChangesAsync();
        }

        // Gestión de acompañantes
        public async Task<List<AcompananteDto>> GetAcompanantesByAgenciaIdAsync(int agenciaId)
        {
            await VerificarPermisosAgencia(agenciaId);

            var acompanantes = await _agenciaRepository.GetAcompanantesByAgenciaIdAsync(agenciaId);
            return _mapper.Map<List<AcompananteDto>>(acompanantes);
        }

        public async Task AgregarAcompananteAsync(int agenciaId, int acompananteId)
        {
            await VerificarPermisosAgencia(agenciaId);

            var acompanante = await _acompananteRepository.GetByIdAsync(acompananteId);

            if (acompanante == null)
                throw new Exception("Acompañante no encontrado.");

            if (acompanante.agencia_id != null && acompanante.agencia_id != agenciaId)
                throw new Exception("El acompañante ya pertenece a otra agencia.");

            acompanante.agencia_id = agenciaId;

            // Al cambiar de agencia, se pierde la verificación anterior
            if (acompanante.esta_verificado == true)
            {
                acompanante.esta_verificado = false;
                acompanante.fecha_verificacion = null;

                // Eliminar registro de verificación si existe
                var verificacion = await _verificacionRepository.GetByAcompananteIdAsync(acompananteId);
                if (verificacion != null)
                {
                    await _verificacionRepository.DeleteAsync(verificacion);
                }
            }

            await _acompananteRepository.UpdateAsync(acompanante);
            await _acompananteRepository.SaveChangesAsync();
        }

        public async Task RemoverAcompananteAsync(int agenciaId, int acompananteId)
        {
            await VerificarPermisosAgencia(agenciaId);

            var acompanante = await _acompananteRepository.GetByIdAsync(acompananteId);

            if (acompanante == null)
                throw new Exception("Acompañante no encontrado.");

            if (acompanante.agencia_id != agenciaId)
                throw new Exception("El acompañante no pertenece a esta agencia.");

            // Al salir de la agencia, se pierde la verificación
            acompanante.agencia_id = null;
            acompanante.esta_verificado = false;
            acompanante.fecha_verificacion = null;

            // Eliminar registro de verificación si existe
            var verificacion = await _verificacionRepository.GetByAcompananteIdAsync(acompananteId);
            if (verificacion != null)
            {
                await _verificacionRepository.DeleteAsync(verificacion);
            }

            await _acompananteRepository.UpdateAsync(acompanante);
            await _acompananteRepository.SaveChangesAsync();
        }

        // Verificación de acompañantes
        public async Task<VerificacionDto> VerificarAcompananteAsync(int agenciaId, int acompananteId, VerificarAcompananteDto datosVerificacion)
        {
            await VerificarPermisosAgencia(agenciaId);

            var agencia = await _agenciaRepository.GetByIdAsync(agenciaId);
            var acompanante = await _acompananteRepository.GetByIdAsync(acompananteId);

            if (agencia == null)
                throw new Exception("Agencia no encontrada.");

            if (acompanante == null)
                throw new Exception("Acompañante no encontrado.");

            if (acompanante.agencia_id != agenciaId)
                throw new Exception("El acompañante no pertenece a esta agencia.");

            if (acompanante.esta_verificado == true)
                throw new Exception("El acompañante ya está verificado.");

            if (agencia.esta_verificada != true)
                throw new Exception("La agencia debe estar verificada para poder verificar acompañantes.");

            // Crear verificación
            var verificacion = new verificacione
            {
                agencia_id = agenciaId,
                acompanante_id = acompananteId,
                fecha_verificacion = DateTime.UtcNow,
                monto_cobrado = datosVerificacion.MontoCobrado,
                estado = "aprobada",
                observaciones = datosVerificacion.Observaciones
            };

            await _verificacionRepository.AddAsync(verificacion);

            // Actualizar acompañante
            acompanante.esta_verificado = true;
            acompanante.fecha_verificacion = DateTime.UtcNow;
            await _acompananteRepository.UpdateAsync(acompanante);

            // Aplicar comisión/descuento a la agencia
            await AplicarComisionPorVerificacionAsync(agenciaId);

            await _verificacionRepository.SaveChangesAsync();
            await _acompananteRepository.SaveChangesAsync();

            return _mapper.Map<VerificacionDto>(verificacion);
        }

        public async Task<List<AcompananteDto>> GetAcompanantesVerificadosAsync(int agenciaId)
        {
            await VerificarPermisosAgencia(agenciaId);

            var acompanantes = await _agenciaRepository.GetAcompanantesVerificadosByAgenciaIdAsync(agenciaId);
            return _mapper.Map<List<AcompananteDto>>(acompanantes);
        }

        public async Task<List<AcompananteDto>> GetAcompanantesPendientesVerificacionAsync(int agenciaId)
        {
            await VerificarPermisosAgencia(agenciaId);

            var todosAcompanantes = await _agenciaRepository.GetAcompanantesByAgenciaIdAsync(agenciaId);
            var pendientes = todosAcompanantes.Where(a => a.esta_verificado != true).ToList();

            return _mapper.Map<List<AcompananteDto>>(pendientes);
        }

        // Anuncios destacados
        public async Task<AnuncioDestacadoDto> CrearAnuncioDestacadoAsync(CrearAnuncioDestacadoDto anuncioDto)
        {
            await VerificarPermisosAgencia(anuncioDto.AgenciaId);

            var acompanante = await _acompananteRepository.GetByIdAsync(anuncioDto.AcompananteId);

            if (acompanante == null)
                throw new Exception("Acompañante no encontrado.");

            if (acompanante.agencia_id != anuncioDto.AgenciaId)
                throw new Exception("El acompañante no pertenece a esta agencia.");

            var nuevoAnuncio = new anuncios_destacado
            {
                acompanante_id = anuncioDto.AcompananteId,
                fecha_inicio = anuncioDto.FechaInicio,
                fecha_fin = anuncioDto.FechaFin,
                tipo = anuncioDto.Tipo,
                monto_pagado = anuncioDto.MontoPagado,
                cupon_id = anuncioDto.CuponId,
                esta_activo = true
            };

            await _anuncioRepository.AddAsync(nuevoAnuncio);
            await _anuncioRepository.SaveChangesAsync();

            return _mapper.Map<AnuncioDestacadoDto>(nuevoAnuncio);
        }

        public async Task<List<AnuncioDestacadoDto>> GetAnunciosByAgenciaAsync(int agenciaId)
        {
            await VerificarPermisosAgencia(agenciaId);

            var anuncios = await _agenciaRepository.GetAnunciosDestacadosByAgenciaIdAsync(agenciaId);
            return _mapper.Map<List<AnuncioDestacadoDto>>(anuncios);
        }

        // Estadísticas y métricas
        public async Task<AgenciaEstadisticasDto> GetEstadisticasAgenciaAsync(int agenciaId)
        {
            await VerificarPermisosAgencia(agenciaId);

            var estadisticas = await _agenciaRepository.GetAgenciaAcompanantesViewByIdAsync(agenciaId);

            if (estadisticas == null)
                throw new Exception("No se encontraron estadísticas para esta agencia.");

            return new AgenciaEstadisticasDto
            {
                AgenciaId = estadisticas.agencia_id ?? 0,
                NombreAgencia = estadisticas.agencia_nombre ?? string.Empty,
                EstaVerificada = estadisticas.agencia_verificada ?? false,
                TotalAcompanantes = estadisticas.total_acompanantes ?? 0,
                AcompanantesVerificados = estadisticas.acompanantes_verificados ?? 0,
                AcompanantesDisponibles = estadisticas.acompanantes_disponibles ?? 0
            };
        }
        public async Task<ComisionesDto> GetComisionesByAgenciaAsync(int agenciaId, DateTime fechaInicio, DateTime fechaFin)
        {
            await VerificarPermisosAgencia(agenciaId);

            var agencia = await _agenciaRepository.GetByIdAsync(agenciaId);

            if (agencia == null)
                throw new Exception("Agencia no encontrada.");

            // Obtener verificaciones en el período
            var verificaciones = await _verificacionRepository.GetByAgenciaIdAndPeriodoAsync(agenciaId, fechaInicio, fechaFin);

            // Calcular comisión
            decimal comisionPorcentaje = await _agenciaRepository.GetComisionPorcentajeByAgenciaIdAsync(agenciaId);
            decimal totalVerificaciones = verificaciones.Sum(v => v.monto_cobrado ?? 0);
            decimal comisionTotal = totalVerificaciones * (comisionPorcentaje / 100);

            return new ComisionesDto
            {
                AgenciaId = agenciaId,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                PorcentajeComision = comisionPorcentaje,
                TotalVerificaciones = verificaciones.Count,  // Corregir esto
                MontoTotalVerificaciones = totalVerificaciones,
                ComisionTotal = comisionTotal
            };
        }

        // Comisiones y beneficios

        // Métodos para administradores
        public async Task<bool> VerificarAgenciaAsync(int agenciaId, bool verificada)
        {
            if (!EsAdmin())
                throw new UnauthorizedAccessException("Solo los administradores pueden verificar agencias.");

            var agencia = await _agenciaRepository.GetByIdAsync(agenciaId);

            if (agencia == null)
                throw new Exception("Agencia no encontrada.");

            agencia.esta_verificada = verificada;

            if (verificada)
            {
                agencia.fecha_verificacion = DateTime.UtcNow;

                // Establecer comisión inicial para la agencia verificada
                agencia.comision_porcentaje = 5.00m; // Comisión base del 5%
            }
            else
            {
                agencia.fecha_verificacion = null;

                // Una agencia no verificada no puede tener acompañantes verificados
                var acompanantes = await _agenciaRepository.GetAcompanantesVerificadosByAgenciaIdAsync(agenciaId);
                foreach (var acompanante in acompanantes)
                {
                    acompanante.esta_verificado = false;
                    acompanante.fecha_verificacion = null;
                    await _acompananteRepository.UpdateAsync(acompanante);
                }

                // Eliminar todas las verificaciones de acompañantes de esta agencia
                await _verificacionRepository.DeleteByAgenciaIdAsync(agenciaId);
            }

            await _agenciaRepository.UpdateAsync(agencia);
            await _agenciaRepository.SaveChangesAsync();

            return verificada;
        }

        public async Task<List<AgenciaPendienteVerificacionDto>> GetAgenciasPendientesVerificacionAsync()
        {
            if (!EsAdmin())
                throw new UnauthorizedAccessException("Solo los administradores pueden ver agencias pendientes de verificación.");

            var agencias = await _agenciaRepository.GetAgenciasPendientesVerificacionAsync();
            return _mapper.Map<List<AgenciaPendienteVerificacionDto>>(agencias);
        }

        // Métodos privados de utilidad
        private int ObtenerUsuarioId()
        {
            var userIdStr = _httpContextAccessor.HttpContext?.User?.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdStr, out int userId))
                throw new UnauthorizedAccessException("No se pudo identificar al usuario.");

            return userId;
        }

        private bool EsAdmin()
        {
            return _httpContextAccessor.HttpContext?.User?.IsInRole("admin") ?? false;
        }

        private async Task VerificarPermisosAgencia(int agenciaId)
        {
            if (EsAdmin())
                return; // Los administradores tienen acceso a todas las agencias

            var agencia = await _agenciaRepository.GetByIdAsync(agenciaId);

            if (agencia == null)
                throw new Exception("Agencia no encontrada.");

            if (agencia.usuario_id != ObtenerUsuarioId())
                throw new UnauthorizedAccessException("No tienes permisos para acceder a esta agencia.");
        }

        private async Task AplicarComisionPorVerificacionAsync(int agenciaId)
        {
            var agencia = await _agenciaRepository.GetByIdAsync(agenciaId);

            if (agencia == null)
                return;

            // Obtener el número de acompañantes verificados para determinar el aumento de comisión
            var verificados = await _agenciaRepository.GetAcompanantesVerificadosByAgenciaIdAsync(agenciaId);
            int cantidadVerificados = verificados.Count;

            // Lógica para aumentar la comisión según la cantidad de verificaciones
            // Esto implementa el concepto "Reciben comisión o descuento al realizar verificaciones"
            decimal nuevaComision = agencia.comision_porcentaje ?? 0;

            if (cantidadVerificados >= 50)
                nuevaComision = 12.00m;
            else if (cantidadVerificados >= 25)
                nuevaComision = 10.00m;
            else if (cantidadVerificados >= 10)
                nuevaComision = 8.00m;
            else if (cantidadVerificados >= 5)
                nuevaComision = 7.00m;
            else if (cantidadVerificados >= 1)
                nuevaComision = 6.00m;

            // Solo actualizar si hay un aumento real (las comisiones no bajan)
            if (nuevaComision > (agencia.comision_porcentaje ?? 0))
            {
                await _agenciaRepository.UpdateComisionPorcentajeAsync(agenciaId, nuevaComision);
            }
        }
    }
}