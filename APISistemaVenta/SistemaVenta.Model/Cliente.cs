using System;
using System.Collections.Generic;

namespace SistemaVenta.Model;

public partial class Cliente
{
    public int IdCliente { get; set; }

    public string? NombreCompleto { get; set; }

    public string? Correo { get; set; }

    public string? Contacto { get; set; }

    public bool? EsActivo { get; set; }

    public DateTime? FechaRegistro { get; set; }
}
