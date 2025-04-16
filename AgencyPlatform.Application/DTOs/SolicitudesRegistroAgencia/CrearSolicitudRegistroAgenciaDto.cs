using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencyPlatform.Application.DTOs.SolicitudesRegistroAgencia
{
    public class CrearSolicitudRegistroAgenciaDto
    {
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Password { get; set; } // Contraseña sin hashear
        public string Descripcion { get; set; }
        public string LogoUrl { get; set; }
        public string SitioWeb { get; set; }
        public string Direccion { get; set; }
        public string Ciudad { get; set; }
        public string Pais { get; set; }
    }
}
