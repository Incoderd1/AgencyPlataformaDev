using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AgencyPlatform.Infrastructure.Services.Storage
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(IFormFile file, string folder);
        bool DeleteFile(string filePath);
        bool FileExists(string filePath);
    }

    public class LocalFileStorageService : IFileStorageService
    {
        private readonly string _basePath;
        private readonly ILogger<LocalFileStorageService> _logger;

        public LocalFileStorageService(IConfiguration configuration, ILogger<LocalFileStorageService> logger)
        {
            // 📁 Ahora guarda en wwwroot/uploads para que sea accesible públicamente
            _basePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            _logger = logger;

            // Crear carpeta base si no existe
            Directory.CreateDirectory(_basePath);
        }

        public async Task<string> SaveFileAsync(IFormFile file, string folder)
        {
            try
            {
                if (file == null || file.Length == 0)
                    throw new ArgumentException("No se proporcionó un archivo válido");

                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                string[] permittedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

                if (string.IsNullOrEmpty(extension) || !Array.Exists(permittedExtensions, e => e == extension))
                    throw new InvalidOperationException("El archivo debe ser una imagen válida");

                var nombreArchivo = $"{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid()}{extension}";

                var rutaCarpeta = Path.Combine(_basePath, folder);
                Directory.CreateDirectory(rutaCarpeta); // se asegura de que exista

                var rutaCompleta = Path.Combine(rutaCarpeta, nombreArchivo);
                using var stream = new FileStream(rutaCompleta, FileMode.Create);
                await file.CopyToAsync(stream);

                return $"/uploads/{folder}/{nombreArchivo}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar archivo {Filename} en carpeta {Carpeta}", file?.FileName, folder);
                throw;
            }
        }

        public bool DeleteFile(string filePath)
        {
            try
            {
                var relativePath = filePath.TrimStart('/');
                var fullPath = Path.Combine(_basePath, relativePath.Replace("uploads/", ""));

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar archivo {FilePath}", filePath);
                return false;
            }
        }

        public bool FileExists(string filePath)
        {
            try
            {
                var relativePath = filePath.TrimStart('/');
                var fullPath = Path.Combine(_basePath, relativePath.Replace("uploads/", ""));

                return File.Exists(fullPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar si existe el archivo {FilePath}", filePath);
                return false;
            }
        }
    }
}
