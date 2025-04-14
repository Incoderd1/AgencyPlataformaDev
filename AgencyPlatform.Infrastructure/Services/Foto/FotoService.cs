using AgencyPlatform.Application.DTOs.Foto;
using AgencyPlatform.Application.Interfaces.Repositories;
using AgencyPlatform.Application.Interfaces.Services.Foto;
using AgencyPlatform.Core.Entities;
using AgencyPlatform.Infrastructure.Services.Storage;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AgencyPlatform.Infrastructure.Services.Foto
{
    public class FotoService : IFotoService
    {
        private readonly IFotoRepository _fotoRepository;
        private readonly IAcompananteRepository _acompananteRepository;
        private readonly IAgenciaRepository _agenciaRepository;
        private readonly IFileStorageService _fileStorage;
        private readonly IMapper _mapper;
        private readonly ILogger<FotoService> _logger;

        public FotoService(
            IFotoRepository fotoRepository,
            IAcompananteRepository acompananteRepository,
            IAgenciaRepository agenciaRepository,
            IFileStorageService fileStorage,
            IMapper mapper,
            ILogger<FotoService> logger)
        {
            _fotoRepository = fotoRepository;
            _acompananteRepository = acompananteRepository;
            _agenciaRepository = agenciaRepository;
            _fileStorage = fileStorage;
            _mapper = mapper;
            _logger = logger;
        }

        public Task<FotoDto> ActualizarFotoAsync(ActualizarFotoDto dto, int usuarioId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> EliminarFotoAsync(int id, int usuarioId)
        {
            throw new NotImplementedException();
        }

        public Task<FotoDto> EstablecerFotoPrincipalAsync(int fotoId, int acompananteId, int usuarioId)
        {
            throw new NotImplementedException();
        }

        public Task<List<FotoDto>> GetByAcompananteIdAsync(int acompananteId)
        {
            throw new NotImplementedException();
        }

        public Task<FotoDto> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<string> GuardarArchivoAsync(IFormFile archivo, string carpeta)
        {
            throw new NotImplementedException();
        }

        public Task<FotoDto> SubirFotoAsync(SubirFotoDto dto, int usuarioId)
        {
            throw new NotImplementedException();
        }

        public Task<FotoDto> VerificarFotoAsync(VerificarFotoDto dto, int agenciaId)
        {
            throw new NotImplementedException();
        }

        //public async Task<List<FotoDto>> GetByAcompananteIdAsync(int acompananteId)
        //{
        //    try
        //    {
        //        var fotos = await _fotoRepository.GetByAcompananteIdAsync(acompananteId);
        //        return _mapper.Map<List<FotoDto>>(fotos);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error al obtener fotos del acompañante {AcompananteId}", acompananteId);
        //        throw;
        //    }
        //}

        //public async Task<FotoDto> GetByIdAsync(int id)
        //{
        //    try
        //    {
        //        var foto = await _fotoRepository.GetByIdAsync(id);
        //        if (foto == null)
        //            return null;

        //        return _mapper.Map<FotoDto>(foto);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error al obtener foto con ID {FotoId}", id);
        //        throw;
        //    }
        //}

        //public async Task<FotoDto> SubirFotoAsync(SubirFotoDto dto, int usuarioId)
        //{
        //    try
        //    {
        //        // Verificar si el acompañante existe
        //        var acompanante = await _acompananteRepository.GetByIdAsync(dto.AcompananteId);
        //        if (acompanante == null)
        //            throw new InvalidOperationException($"El acompañante con ID {dto.AcompananteId} no existe");

        //        // Verificar permisos (si el usuario es dueño del perfil o es la agencia del acompañante)
        //        if (acompanante.usuario_id != usuarioId && acompanante.agencia?.usuario_id != usuarioId)
        //            throw new UnauthorizedAccessException("No tienes permisos para subir fotos a este perfil");

        //        // Guardar el archivo físicamente
        //        var urlArchivo = await GuardarArchivoAsync(dto.Foto, $"acompanantes/{dto.AcompananteId}");

        //        // Si es foto principal, actualizar las demás fotos
        //        if (dto.EsPrincipal)
        //        {
        //            var fotoPrincipalActual = await _fotoRepository.GetFotoPrincipalAsync(dto.AcompananteId);
        //            if (fotoPrincipalActual != null)
        //            {
        //                fotoPrincipalActual.es_principal = false;
        //                await _fotoRepository.UpdateAsync(fotoPrincipalActual);
        //            }
        //        }

        //        // Determinar el orden
        //        int orden = dto.Orden;
        //        if (orden == 0)
        //        {
        //            orden = await _fotoRepository.GetMaxOrdenAsync(dto.AcompananteId) + 1;
        //        }

        //        // Crear la entidad foto
        //        var nuevaFoto = new foto
        //        {
        //            acompanante_id = dto.AcompananteId,
        //            url = urlArchivo,
        //            es_principal = dto.EsPrincipal,
        //            orden = orden,
        //            estado = "activo",
        //            verificada = false,
        //            descripcion = dto.Descripcion,
        //            tipo = dto.Tipo,
        //            created_at = DateTime.UtcNow,
        //            updated_at = DateTime.UtcNow
        //        };

        //        // Guardar en la base de datos
        //        await _fotoRepository.AddAsync(nuevaFoto);
        //        await _fotoRepository.SaveChangesAsync();

        //        // Mapear y retornar DTO
        //        var fotoCompleta = await _fotoRepository.GetByIdAsync(nuevaFoto.id);
        //        return _mapper.Map<FotoDto>(fotoCompleta);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error al subir foto para acompañante {AcompananteId}", dto.AcompananteId);
        //        throw;
        //    }
        //}

        ////public async Task<FotoDto> ActualizarFotoAsync(ActualizarFotoDto dto, int usuarioId)
        ////{
        ////    //try
        ////    //{
        ////    //    // Obtener la foto
        ////    //    var foto = await _fotoRepository.GetByIdAsync(dto.Id);
        ////    //    if (foto == null)
        ////    //        throw new InvalidOperationException($"La foto con ID {dto.Id} no existe");

        ////    //    // Verificar permisos
        ////    //    var acompanante = await _acompananteRepository.GetByIdAsync(foto.acompanante_id);
        ////    //    if (acompanante == null)
        ////    //        throw new InvalidOperationException("El acompañante asociado a esta foto no existe");

        ////    //    if (acompanante.usuario_id != usuarioId && acompanante.agencia?.usuario_id != usuarioId)
        ////    //        throw new UnauthorizedAccessException("No tienes permisos para modificar esta foto");

        ////    //    // Si se está estableciendo como principal, actualizar las demás fotos
        ////    //    if (dto.EsPrincipal.HasValue && dto.EsPrincipal.Value && !foto.es_principal.GetValueOrDefault())
        ////    //    {
        ////    //        var fotoPrincipalActual = await _fotoRepository.GetFotoPrincipalAsync(foto.acompanante_id);
        ////    //        if (fotoPrincipalActual != null && fotoPrincipalActual.id != foto.id)
        ////    //        {
        ////    //            fotoPrincipalActual.es_principal = false;
        ////    //            await _fotoRepository.UpdateAsync(fotoPrincipalActual);
        ////    //        }
        ////    //    }

        ////    //    // Actualizar propiedades
        ////    //    if (dto.EsPrincipal.HasValue)
        ////    //        foto.es_principal = dto.EsPrincipal.Value;

        ////    //    if (dto.Orden.HasValue)
        ////    //        foto.orden = dto.Orden.Value;

        ////    //    //if (!string.IsNullOrEmpty(dto.Descripcion))
        ////    //    //    foto.descripcion = dto.Descripcion;

        ////    //    //if (!string.IsNullOrEmpty(dto.Estado))
        ////    //    //    foto.estado = dto.Estado;

        ////    //    foto.updated_at = DateTime.UtcNow;

        ////    //    // Guardar cambios
        ////    //    await _fotoRepository.UpdateAsync(foto);
        ////    //    await _fotoRepository.SaveChangesAsync();

        ////    //    // Retornar DTO actualizado
        ////    //    var fotoActualizada = await _fotoRepository.GetByIdAsync(foto.id);
        ////    //    return _mapper.Map<FotoDto>(fotoActualizada);
        ////    //}
        ////    //catch (Exception ex)
        ////    //{
        ////    //    _logger.LogError(ex, "Error al actualizar foto con ID {FotoId}", dto.Id);
        ////    //    throw;
        ////    //}
        ////}

        //public async Task<bool> EliminarFotoAsync(int id, int usuarioId)
        //{
        //    try
        //    {
        //        // Obtener la foto
        //        var foto = await _fotoRepository.GetByIdAsync(id);
        //        if (foto == null)
        //            return false;

        //        // Verificar permisos
        //        var acompanante = await _acompananteRepository.GetByIdAsync(foto.acompanante_id);
        //        if (acompanante == null)
        //            throw new InvalidOperationException("El acompañante asociado a esta foto no existe");

        //        if (acompanante.usuario_id != usuarioId && acompanante.agencia?.usuario_id != usuarioId)
        //            throw new UnauthorizedAccessException("No tienes permisos para eliminar esta foto");

        //        // Eliminar el archivo físico
        //        if (!string.IsNullOrEmpty(foto.url))
        //        {
        //            _fileStorage.DeleteFile(foto.url);
        //        }

        //        // Eliminar de la base de datos
        //        await _fotoRepository.DeleteAsync(foto);
        //        await _fotoRepository.SaveChangesAsync();

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error al eliminar foto con ID {FotoId}", id);
        //        throw;
        //    }
        //}


        //public async Task<FotoDto> VerificarFotoAsync(VerificarFotoDto dto, int agenciaId)
        //{
        //    try
        //    {
        //        // Obtener la foto
        //        var foto = await _fotoRepository.GetByIdAsync(dto.Id);
        //        if (foto == null)
        //            throw new InvalidOperationException($"La foto con ID {dto.Id} no existe");

        //        // Verificar permisos (solo agencias pueden verificar fotos)
        //        var agencia = await _agenciaRepository.GetByIdAsync(agenciaId);
        //        if (agencia == null)
        //            throw new UnauthorizedAccessException("Solo las agencias pueden verificar fotos");

        //        // Verificar que la agencia está verificada
        //        if (agencia.esta_verificada != true)
        //            throw new UnauthorizedAccessException("Solo agencias verificadas pueden realizar verificaciones");

        //        // Verificar que la agencia representa al acompañante
        //        var acompanante = await _acompananteRepository.GetByIdAsync(foto.acompanante_id);
        //        if (acompanante == null || acompanante.agencia_id != agenciaId)
        //            throw new UnauthorizedAccessException("Solo puedes verificar fotos de acompañantes que representas");

        //        //// Actualizar estado de verificación
        //        //foto.verificada = dto.Verificada;
        //        //foto.verificada_por = agenciaId;
        //        //foto.fecha_verificacion = DateTime.UtcNow;
        //        foto.updated_at = DateTime.UtcNow;

        //        // Guardar cambios
        //        await _fotoRepository.UpdateAsync(foto);
        //        await _fotoRepository.SaveChangesAsync();

        //        // Retornar DTO actualizado
        //        var fotoActualizada = await _fotoRepository.GetByIdAsync(foto.id);
        //        return _mapper.Map<FotoDto>(fotoActualizada);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error al verificar foto con ID {FotoId}", dto.Id);
        //        throw;
        //    }
        //}

        //public async Task<FotoDto> EstablecerFotoPrincipalAsync(int fotoId, int acompananteId, int usuarioId)
        //{
        //    try
        //    {
        //        // Verificar si la foto existe
        //        var foto = await _fotoRepository.GetByIdAsync(fotoId);
        //        if (foto == null)
        //            throw new InvalidOperationException($"La foto con ID {fotoId} no existe");

        //        // Verificar que la foto pertenece al acompañante
        //        if (foto.acompanante_id != acompananteId)
        //            throw new InvalidOperationException("La foto no pertenece al acompañante especificado");

        //        // Verificar permisos
        //        var acompanante = await _acompananteRepository.GetByIdAsync(acompananteId);
        //        if (acompanante == null)
        //            throw new InvalidOperationException($"El acompañante con ID {acompananteId} no existe");

        //        if (acompanante.usuario_id != usuarioId && acompanante.agencia?.usuario_id != usuarioId)
        //            throw new UnauthorizedAccessException("No tienes permisos para modificar las fotos de este perfil");

        //        // Quitar marca de principal a cualquier otra foto
        //        var fotoPrincipalActual = await _fotoRepository.GetFotoPrincipalAsync(acompananteId);
        //        if (fotoPrincipalActual != null && fotoPrincipalActual.id != fotoId)
        //        {
        //            fotoPrincipalActual.es_principal = false;
        //            await _fotoRepository.UpdateAsync(fotoPrincipalActual);
        //        }

        //        // Establecer como principal
        //        foto.es_principal = true;
        //        foto.updated_at = DateTime.UtcNow;

        //        await _fotoRepository.UpdateAsync(foto);
        //        await _fotoRepository.SaveChangesAsync();

        //        return _mapper.Map<FotoDto>(foto);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error al establecer foto principal {FotoId} para acompañante {AcompananteId}", fotoId, acompananteId);
        //        throw;
        //    }
        //}

        //public async Task<string> GuardarArchivoAsync(IFormFile archivo, string carpeta)
        //{
        //    return await _fileStorage.SaveFileAsync(archivo, carpeta);
        //}
    }
}