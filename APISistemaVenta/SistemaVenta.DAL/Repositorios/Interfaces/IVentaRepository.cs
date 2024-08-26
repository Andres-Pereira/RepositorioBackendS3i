using SistemaVenta.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.DAL.Repositorios.Interfaces
{
    public interface IVentaRepository : IGenericRepository<Venta>
    {
        Task<Venta> Registrar(Venta modelo);
        Task<bool> EditarVenta(Venta modelo);
        Task<bool> EliminarVenta(Venta modelo, List<DetalleVenta> detalles);
        Task<bool> EliminarDetalle(DetalleVenta modelo);


    }
}
