using AutoMapper;
using SistemaVenta.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SistemaVenta.DTO;
using SistemaVenta.DAL.Repositorios.Interfaces;
using SistemaVenta.BLL.Servicios.Interfaces;

namespace SistemaVenta.BLL.Servicios
{
    public class ClienteService: IClienteService
    {
        private readonly IGenericRepository<Cliente> _clienteRepositorio;
        private readonly IMapper _mapper;

        public ClienteService(IGenericRepository<Cliente> clienteRepositorio, IMapper mapper)
        {
            _clienteRepositorio = clienteRepositorio;
            _mapper = mapper;
        }

        public async Task<List<ClienteDTO>> Lista()
        {
            try
            {
                var listaClientes = await _clienteRepositorio.Consultar();
                return _mapper.Map<List<ClienteDTO>>(listaClientes.ToList());
            }
            catch
            {
                throw;
            }
        }
        public async Task<ClienteDTO> Crear(ClienteDTO modelo)
        {
            try
            {
                var clienteCreado = await _clienteRepositorio.Crear(_mapper.Map<Cliente>(modelo));

                if (clienteCreado.IdCliente == 0)
                    throw new TaskCanceledException("No se pudo crear");

                return _mapper.Map<ClienteDTO>(clienteCreado);

            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> Editar(ClienteDTO modelo)
        {
            try
            {

                var clienteModelo = _mapper.Map<Cliente>(modelo);
                var clienteEncontrado = await _clienteRepositorio.Obtener(u =>
                    u.IdCliente == clienteModelo.IdCliente
                );

                if (clienteEncontrado == null)
                    throw new TaskCanceledException("El cliente no existe");

                clienteEncontrado.NombreCompleto = clienteModelo.NombreCompleto;
                clienteEncontrado.Correo = clienteModelo.Correo;
                clienteEncontrado.Contacto = clienteModelo.Contacto;
                clienteEncontrado.EsActivo = clienteModelo.EsActivo;

                bool respuesta = await _clienteRepositorio.Editar(clienteEncontrado);

                if (!respuesta)
                    throw new TaskCanceledException("No se pudo editar"); ;


                return respuesta;

            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> Eliminar(int id)
        {
            try
            {
                var clienteEncontrado = await _clienteRepositorio.Obtener(p => p.IdCliente == id);

                if (clienteEncontrado == null)
                    throw new TaskCanceledException("El cliente no existe");

                bool respuesta = await _clienteRepositorio.Eliminar(clienteEncontrado);

                if (!respuesta)
                    throw new TaskCanceledException("No se pudo elminar"); ;

                return respuesta;

            }
            catch
            {
                throw;
            }
        }
    }
}
