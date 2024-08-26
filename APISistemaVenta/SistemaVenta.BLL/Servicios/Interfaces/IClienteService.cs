using SistemaVenta.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.Servicios.Interfaces
{
    public interface IClienteService
    {
        Task<List<ClienteDTO>> Lista();
        Task<ClienteDTO> Crear(ClienteDTO modelo);
        Task<bool> Editar(ClienteDTO modelo);
        Task<bool> Eliminar(int id);
    }
}
