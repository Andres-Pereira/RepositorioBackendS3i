using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaVenta.DTO;

namespace SistemaVenta.BLL.Servicios.Interfaces
{
    public interface IActivoService
    {
        Task<List<ActivoDTO>> Lista();
        Task<List<ActivoDTO>> ListaSintrato();
        Task<List<ActivoDTO>> ListaContrato();
        Task<ActivoDTO> Crear(ActivoDTO modelo);
        Task<List<ActivoDTO>> FiltrosContratos(string criterioBusqueda, string encargado, string cliente);
        Task<bool> Editar(ActivoDTO modelo);
        Task<bool> EditarEstado(ActivoDTO modelo);
        Task<bool> Eliminar(int id);

    }
}
