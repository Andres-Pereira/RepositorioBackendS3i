using System;
using System.Collections.Generic;

namespace SistemaVenta.Model;

public partial class Activo
{
    public int IdActivo { get; set; }

    public string? Nombre { get; set; }

    public string? Marca { get; set; }

    public string? Codigo { get; set; }

    public bool? EsActivo { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public bool? EsContrato { get; set; }

    public string? Cliente { get; set; }

    public string? Encargado { get; set; }

    public DateTime? FechaCaducidad { get; set; }
}
