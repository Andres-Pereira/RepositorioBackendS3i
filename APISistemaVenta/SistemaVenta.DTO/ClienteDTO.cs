using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.DTO
{
    public class ClienteDTO
    {
        public int IdCliente { get; set; }

        public string? NombreCompleto { get; set; }

        public string? Correo { get; set; }

        public string? Contacto { get; set; }

        public int? EsActivo { get; set; }

    }
}
