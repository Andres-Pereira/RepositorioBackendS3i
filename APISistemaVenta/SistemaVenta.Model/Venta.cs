using System;
using System.Collections.Generic;

namespace SistemaVenta.Model;

public partial class Venta
{
    public int IdVenta { get; set; }

    public string? NumeroDocumento { get; set; }

    public string? TipoPago { get; set; }

    public decimal? Total { get; set; }

    public bool? EsPagado { get; set; }

    public string? Cliente { get; set; }

    public string? Vendedor { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public virtual ICollection<DetalleVenta> DetalleVenta { get; } = new List<DetalleVenta>();
}
