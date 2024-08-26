using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaVenta.DTO;

namespace SistemaVenta.BLL.Servicios.Interfaces
{
    public interface IDashBoardService
    {
        Task<DashBoardDTO> ResumenIntervalo(int interv);
        Task<DashBoardDTO> ResumenMes(int mes, int año);
        Task<DashBoardDTO> ResumenAnual(int año);
        Task<DashBoardDTO> ResumenProductos();
        Task<DashBoardDTO> ResumenVendedores(int año);
    }
}
