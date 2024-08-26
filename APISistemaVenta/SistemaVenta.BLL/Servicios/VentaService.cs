using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SistemaVenta.BLL.Servicios.Interfaces;
using SistemaVenta.DAL.DBContext;
using SistemaVenta.DAL.Repositorios.Interfaces;
using SistemaVenta.DTO;
using SistemaVenta.Model;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SistemaVenta.BLL.Servicios
{
    public class VentaService : IVentaService
    {
        private readonly IVentaRepository _ventaRepositorio;
        private readonly IGenericRepository<DetalleVenta> _detalleVentaRepositorio;
        private readonly IMapper _mapper;
        private readonly DbventaContext _dbcontext;

        public VentaService(IVentaRepository ventaRepositorio,
            IGenericRepository<DetalleVenta> detalleVentaRepositorio,
            IMapper mapper, DbventaContext dbcontext)
        {
            _ventaRepositorio = ventaRepositorio;
            _detalleVentaRepositorio = detalleVentaRepositorio;
            _mapper = mapper;
            _dbcontext = dbcontext;
        }

        public async Task<List<VentaDTO>> Lista()
        {
            try
            {
                var listaVenta = await _ventaRepositorio.Consultar();
                return _mapper.Map<List<VentaDTO>>(listaVenta.ToList());
            }
            catch
            {
                throw;
            }


        }
        public async Task<VentaDTO> Registrar(VentaDTO modelo)
        {
            try
            {

                var ventaGenerada = await _ventaRepositorio.Registrar(_mapper.Map<Venta>(modelo));

                if (ventaGenerada.IdVenta == 0)
                    throw new TaskCanceledException("No se pudo crear");

                return _mapper.Map<VentaDTO>(ventaGenerada);

            }
            catch
            {
                throw;
            }
        }

        public async Task<List<VentaDTO>> Historial(string criterioBusqueda, string numeroVenta, string fechaInicio, string fechaFin, string vendedor, string cliente)
        {
            IQueryable<Venta> query = await _ventaRepositorio.Consultar();
            var ListaResultante = new List<Venta>();

            try
            {
                if (criterioBusqueda == "fecha")
                {
                    DateTime fech_Inicio = DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", new CultureInfo("es-BO"));
                    DateTime fech_Fin = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", new CultureInfo("es-BO"));

                    ListaResultante = await query.Where(v =>
                        v.FechaRegistro.Value.Date >= fech_Inicio.Date &&
                        v.FechaRegistro.Value.Date <= fech_Fin.Date
                    ).Include(dv => dv.DetalleVenta)
                    .ThenInclude(p => p.IdProductoNavigation)
                    .ToListAsync();
                }
                if (criterioBusqueda == "numero")
                {
                    ListaResultante = await query.Where(v => v.NumeroDocumento == numeroVenta
                      ).Include(dv => dv.DetalleVenta)
                      .ThenInclude(p => p.IdProductoNavigation)
                      .ToListAsync();
                }
                if (criterioBusqueda == "cuentas")
                {
                    ListaResultante = await query.Where(v => v.EsPagado == false && v.Vendedor == vendedor
                      ).Include(dv => dv.DetalleVenta)
                      .ThenInclude(p => p.IdProductoNavigation)
                      .ToListAsync();
                }
                if (criterioBusqueda == "busqueda")
                {
                    ListaResultante = await query.Include(dv => dv.DetalleVenta)
                      .ThenInclude(p => p.IdProductoNavigation)
                      .ToListAsync();
                }
                if (criterioBusqueda == "clientes")
                {
                    ListaResultante = await query.Where(v => v.Cliente == cliente
                      ).Include(dv => dv.DetalleVenta)
                      .ThenInclude(p => p.IdProductoNavigation)
                      .ToListAsync();
                }

            }
            catch
            {
                throw;
            }

            return _mapper.Map<List<VentaDTO>>(ListaResultante);
        }

        public async Task<bool> EditarEstadoPago(VentaDTO modelo)
        {
            try
            {
                var ventaModelo = _mapper.Map<Venta>(modelo);
                var ventaEncontrada = await _ventaRepositorio.Obtener(u =>
                    u.IdVenta == ventaModelo.IdVenta
                );

                if (ventaEncontrada == null)
                    throw new TaskCanceledException("La venta no existe");

                if(ventaEncontrada.EsPagado==true)
                {
                    ventaEncontrada.EsPagado = false;
                }
                else
                    ventaEncontrada.EsPagado = true;


                bool respuesta = await _ventaRepositorio.Editar(ventaEncontrada);

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
                IQueryable<DetalleVenta> query = await _detalleVentaRepositorio.Consultar();
                var ListaDetalles = new List<DetalleVenta>();

                ListaDetalles = await query
                    .Include(p => p.IdProductoNavigation)
                    .Include(v => v.IdVentaNavigation)
                    .Where(dv =>
                        dv.IdVentaNavigation.IdVenta == id
                    ).ToListAsync();

                var ventaEncontrada = await _ventaRepositorio.Obtener(p => p.IdVenta == id);

                 if (ventaEncontrada == null)
                    throw new TaskCanceledException("La venta no existe");

                int i = 0;
                bool resp = true;
                while (resp == true)
                {
                    var detalle_encontrado = await _detalleVentaRepositorio.Obtener(d => d.IdVenta == id);
                    if (detalle_encontrado == null)
                        break;
                    resp = await _ventaRepositorio.EliminarDetalle(detalle_encontrado);
                }

                bool respuesta = await _ventaRepositorio.EliminarVenta(ventaEncontrada, ListaDetalles);

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
