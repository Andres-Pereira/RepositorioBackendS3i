using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaVenta.DTO;

namespace SistemaVenta.BLL.Servicios.Interfaces
{
    public interface IVentaService
    {
        Task<List<VentaDTO>> Lista();
        Task<VentaDTO> Registrar(VentaDTO modelo);
        Task<List<VentaDTO>> Historial(string criterioBusqueda, string numeroVenta, string fechaInicio, string fechaFin, string vendedor, string cliente);
        Task<bool> EditarEstadoPago(VentaDTO modelo);
        Task<bool> Eliminar(int id);
    }
}
