using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.DTO
{
    public class ActivoDTO
    {
        public int IdActivo { get; set; }
        public string? Nombre { get; set; }
        public string? Marca { get; set; }
        public string? Codigo { get; set; }
        public string? Cliente { get; set; }
        public string? Encargado { get; set; }
        public int? EsContrato { get; set; }
        public int? EsActivo { get; set; }
        public string? FechaCaducidad { get; set; }
    }
}
