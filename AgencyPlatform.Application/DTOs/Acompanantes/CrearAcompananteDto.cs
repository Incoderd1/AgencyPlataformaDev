using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencyPlatform.Application.DTOs.Acompanantes
{
    public class CrearAcompananteDto
    {
        public string NombrePerfil { get; set; } = string.Empty;
        public string Genero { get; set; } = string.Empty;
        public int Edad { get; set; }
        public string? Descripcion { get; set; }
        public int? Altura { get; set; }
        public int? Peso { get; set; }
        public string? Ciudad { get; set; }
        public string? Pais { get; set; }
        public string? Idiomas { get; set; }
        public string? Disponibilidad { get; set; }
        public decimal? TarifaBase { get; set; }
        public string Moneda { get; set; } = "USD";
        public List<int> CategoriaIds { get; set; } = new List<int>();
    }
}
