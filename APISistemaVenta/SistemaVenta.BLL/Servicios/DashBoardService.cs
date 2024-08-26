using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using SistemaVenta.BLL.Servicios.Interfaces;
using SistemaVenta.DAL.Repositorios.Interfaces;
using SistemaVenta.DTO;
using SistemaVenta.Model;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SistemaVenta.BLL.Servicios
{
    public class DashBoardService : IDashBoardService
    {
        private readonly IVentaRepository _ventaRepositorio;
        private readonly IGenericRepository<Producto> _productoRepositorio;
        private readonly IGenericRepository<DetalleVenta> _detalleRepositorio;
        private readonly IGenericRepository<Usuario> _usuarioRepositorio;


        private readonly IMapper _mapper;

        public DashBoardService(IVentaRepository ventaRepositorio,
            IGenericRepository<Producto> productoRepositorio,
            IGenericRepository<DetalleVenta> detalleRepositorio,
            IGenericRepository<Usuario> usuarioRepositorio,
            IMapper mapper)
        {
            _ventaRepositorio = ventaRepositorio;
            _productoRepositorio = productoRepositorio;
            _detalleRepositorio = detalleRepositorio;
            _usuarioRepositorio = usuarioRepositorio;
            _mapper = mapper;
        }

        #region ventasIntervalo
        private IQueryable<Venta> retornarVentas(IQueryable<Venta> tablaVenta, int restarCantidadDias)
        {
            DateTime? ultimaFecha = tablaVenta.OrderByDescending(v => v.FechaRegistro).Select(v => v.FechaRegistro).First();
            ultimaFecha = ultimaFecha.Value.AddDays(restarCantidadDias);

            return tablaVenta.Where(v => v.FechaRegistro.Value.Date >= ultimaFecha.Value.Date);
        }
        private async Task<int> TotalVentasIntervalo(int interv)
        {
            int total = 0;
            IQueryable<Venta> _ventaQuery = await _ventaRepositorio.Consultar();

            if (_ventaQuery.Count() > 0)
            {
                var tablaVenta = retornarVentas(_ventaQuery, -interv);
                total = tablaVenta.Count();
            }

            return total;
        }
        private async Task<string> TotalIngresosIntervalo(int interv)
        {
            decimal ingreso = 0;
            IQueryable<Venta> _ventaQuery = await _ventaRepositorio.Consultar();

            if (_ventaQuery.Count() > 0)
            {
                var tablaventa = retornarVentas(_ventaQuery, -interv);

                ingreso = tablaventa.Select(v => v.Total).Sum(v => v.Value);
            }

            return Convert.ToString(ingreso, new CultureInfo("es-BO"));

        }

        private async Task<int> TotalProductos()
        {

            IQueryable<Producto> _productoQuery = await _productoRepositorio.Consultar();
            int total = _productoQuery.Count();
            return total;
        }
        private async Task<Dictionary<string, int>> VentasIntervalo(int interv)
        {
            Dictionary<string, int> resultado = new Dictionary<string, int>();

            IQueryable<Venta> _ventaQuery = await _ventaRepositorio.Consultar();

            if (_ventaQuery.Count() > 0)
            {
                var tablaVenta = retornarVentas(_ventaQuery, -interv);

                resultado = tablaVenta
                    .GroupBy(v => v.FechaRegistro.Value.Date).OrderBy(g => g.Key) 
                    .Select(dv => new { fecha = dv.Key.ToString("dd/MM/yyyy"), total = dv.Count() }) 
                    .ToDictionary(keySelector: r => r.fecha, elementSelector: r => r.total); 
            }

            return resultado;
        }

        public async Task<DashBoardDTO> ResumenIntervalo(int interv)
        {
            DashBoardDTO vmDahsBoard = new DashBoardDTO();

            try
            {
                vmDahsBoard.TotalVentas = await TotalVentasIntervalo(interv);
                vmDahsBoard.TotalIngresos = await TotalIngresosIntervalo(interv);
                vmDahsBoard.TotalProductos = await TotalProductos();

                List<DatosDashDTO> listaVentaSemana = new List<DatosDashDTO>();

                foreach (KeyValuePair<string, int> item in await VentasIntervalo(interv))
                {
                    listaVentaSemana.Add(new DatosDashDTO()
                    {
                        Descripcion = item.Key,
                        Total = item.Value
                    });
                }

                vmDahsBoard.DatosDash = listaVentaSemana;
            }
            catch
            {
                throw;
            }

            return vmDahsBoard;
        }

        #endregion

        #region VentaMes

        private IQueryable<Venta> retornarVentasMes(IQueryable<Venta> tablaVenta, int restarCantidadDias, int mes, int año)
        {
            int dias = DateTime.DaysInMonth(año, mes);
            DateTime? fecha_fin = new DateTime(año, mes, dias);
            DateTime? fech_Inicio = fecha_fin.Value.AddDays(restarCantidadDias);

            return tablaVenta.Where(v =>
                        v.FechaRegistro.Value.Date >= fech_Inicio.Value.Date &&
                        v.FechaRegistro.Value.Date <= fecha_fin.Value.Date);
        }

        private async Task<int> TotalVentasMes(int mes, int año)
        {
            int total = 0;
            int dias = DateTime.DaysInMonth(año, mes);

            var fech_Fin = new DateTime(año, mes, dias);

            IQueryable<Venta> _ventaQuery = await _ventaRepositorio.Consultar();

            if (_ventaQuery.Count() > 0)
            {

                var tablaVenta = retornarVentasMes(_ventaQuery, -dias, mes,año);
                total = tablaVenta.Count();
            }

            return total;
        }
        private async Task<string> TotalIngresosMes(int mes, int año)
        {

            decimal resultado = 0;
            int dias = DateTime.DaysInMonth(año, mes);
            IQueryable<Venta> _ventaQuery = await _ventaRepositorio.Consultar();

            if (_ventaQuery.Count() > 0)
            {
                var tablaventa = retornarVentasMes(_ventaQuery, -dias, mes, año);

                resultado = tablaventa.Select(v => v.Total).Sum(v => v.Value);
            }

            return Convert.ToString(resultado, new CultureInfo("es-BO"));

        }

        private async Task<Dictionary<string, int>> VentasMes(int mes, int año)
        {

            Dictionary<string, int> resultado = new Dictionary<string, int>();
            int dias = DateTime.DaysInMonth(año, mes);

            IQueryable<Venta> _ventaQuery = await _ventaRepositorio.Consultar();

            if (_ventaQuery.Count() > 0)
            {

                var tablaVenta = retornarVentasMes(_ventaQuery, -dias, mes, año);

                resultado = tablaVenta
                    .GroupBy(v => v.FechaRegistro.Value.Date).OrderBy(g => g.Key) 
                    .Select(dv => new { fecha = dv.Key.ToString("dd/MM/yyyy"), total = dv.Count() })
                    .ToDictionary(keySelector: r => r.fecha, elementSelector: r => r.total); 
            }

            return resultado;
        }
        public async Task<DashBoardDTO> ResumenMes(int mes, int año)
        {
            DashBoardDTO vmDahsBoard = new DashBoardDTO();
            try
            {
                vmDahsBoard.TotalVentas = await TotalVentasMes(mes, año);
                vmDahsBoard.TotalIngresos = await TotalIngresosMes(mes, año);
                vmDahsBoard.TotalProductos = await TotalProductos();

                List<DatosDashDTO> listaVentaMes = new List<DatosDashDTO>();
                foreach (KeyValuePair<string, int> item in await VentasMes(mes, año))
                {
                    listaVentaMes.Add(new DatosDashDTO()
                    {
                        Descripcion = item.Key,
                        Total = item.Value
                    });
                }

                vmDahsBoard.DatosDash = listaVentaMes;
            }
            catch
            {
                throw;
            }

            return vmDahsBoard;
        }

        #endregion

        #region ventasAnuales
        private IQueryable<Venta> retornarVentasAño(IQueryable<Venta> tablaVenta, int año)
        {
            DateTime? fecha_fin = new DateTime(año, 12, 31);
            DateTime? fech_Inicio = new DateTime(año, 1, 1);

            return tablaVenta.Where(v =>
                        v.FechaRegistro.Value.Date >= fech_Inicio.Value.Date &&
                        v.FechaRegistro.Value.Date <= fecha_fin.Value.Date);
        }

        private async Task<int> TotalVentasAño(int año)
        {
            int total = 0;

            IQueryable<Venta> _ventaQuery = await _ventaRepositorio.Consultar();

            if (_ventaQuery.Count() > 0)
            {
                var tablaVenta = retornarVentasAño(_ventaQuery, año);
                total = tablaVenta.Count();
            }

            return total;
        }
        private async Task<string> TotalIngresosAño(int año)
        {
            decimal ingreso = 0;
            IQueryable<Venta> _ventaQuery = await _ventaRepositorio.Consultar();

            if (_ventaQuery.Count() > 0)
            {
                var tablaventa = retornarVentasAño(_ventaQuery,año);

                ingreso = tablaventa.Select(v => v.Total).Sum(v => v.Value);
            }

            return Convert.ToString(ingreso, new CultureInfo("es-BO"));

        }

        private async Task<Dictionary<string, int>> VentasAño(int año)
        {
            DateTimeFormatInfo dtinfo = new CultureInfo("es-BO", false).DateTimeFormat;
            System.Globalization.DateTimeFormatInfo mfi = new
            
            System.Globalization.DateTimeFormatInfo();
            Dictionary<string, int> resultado = new Dictionary<string, int>();

            IQueryable<Venta> _ventaQuery = await _ventaRepositorio.Consultar();

            if (_ventaQuery.Count() > 0)
            {
                var tablaVenta = retornarVentasAño(_ventaQuery,año);

                resultado = tablaVenta
                    .GroupBy(v => v.FechaRegistro.Value.Month).OrderBy(g => g.Key) 
                    .Select(dv => new { mes = dtinfo.GetMonthName(dv.Key).ToString(), total = dv.Count() })
                    .ToDictionary(r => r.mes, r => r.total); 
            }

            return resultado;
        }

        public async Task<DashBoardDTO> ResumenAnual(int año)
        {
            DashBoardDTO vmDahsBoard = new DashBoardDTO();
            try
            {
                vmDahsBoard.TotalVentas = await TotalVentasAño(año);
                vmDahsBoard.TotalIngresos = await TotalIngresosAño(año);
                vmDahsBoard.TotalProductos = await TotalProductos();

                List<DatosDashDTO> listaVentaMes = new List<DatosDashDTO>();
                foreach (KeyValuePair<string, int> item in await VentasAño(año))
                {
                    listaVentaMes.Add(new DatosDashDTO()
                    {
                        Descripcion = item.Key,
                        Total = item.Value
                    });
                }

                vmDahsBoard.DatosDash = listaVentaMes;
            }
            catch
            {
                throw;
            }

            return vmDahsBoard;
        }

        #endregion

        #region ProductosVendidods
        private async Task<Dictionary<string, int>> ProductosVendidos()
        {
            Dictionary<string, int> resultado = new Dictionary<string, int>();

            IQueryable<DetalleVenta> _ventaQuery = await _detalleRepositorio.Consultar();

            if (_ventaQuery.Count() > 0)
            {
                var tablaVenta = _ventaQuery;

                resultado = tablaVenta
                    .GroupBy(v => v.IdProducto.Value).OrderBy(g => g.Key)
                    .Select(dv => new { producto = dv.Key.ToString(), total = dv.Count() }).OrderByDescending(g => g.total)
                    .ToDictionary(r => r.producto, r => r.total);
            }

            return resultado;
        }

        public async Task<string> encontrarProducto(int id)
        {
            String resp = "";
            try
            {
                var productoEncontrado = await _productoRepositorio.Obtener(p => p.IdProducto == id);

                if (productoEncontrado == null)
                {
                     resp = "El producto no existe";
                }
                else
                {
                    resp = productoEncontrado.Nombre;
                }

                return resp;
                
            }
            catch
            {
                throw;
            }
        }

        public async Task<DashBoardDTO> ResumenProductos()
        {
            DashBoardDTO vmDahsBoard = new DashBoardDTO();
            try
            {
                vmDahsBoard.TotalProductos = await TotalProductos();
                int i = 0;
                List<DatosDashDTO> listaVentaMes = new List<DatosDashDTO>();

                foreach (KeyValuePair<string, int> item in await ProductosVendidos())
                {
                    if(i>4)
                    {
                        break;
                    }

                    Task<string> task1 = encontrarProducto(Int32.Parse(item.Key)); 
                    string text = task1.Result;

                    listaVentaMes.Add(new DatosDashDTO()
                    {
                        Descripcion = text,
                        Total = item.Value
                    });

                    i++;
                }

                vmDahsBoard.DatosDash = listaVentaMes;
            }
            catch
            {
                throw;
            }

            return vmDahsBoard;
        }

        #endregion

        #region Vendedores
        private async Task<int> TotalVendedores()
        {
            int total = 0;

            IQueryable<Usuario> _userQuery = await _usuarioRepositorio.Consultar();

            if (_userQuery.Count() > 0)
            {
                var tablaVendedores = _userQuery;
                total = tablaVendedores.Count();
            }

            return total;
        }

        private async Task<Dictionary<string, int>> VendedoresAño(int año)
        {
            Dictionary<string, int> resultado = new Dictionary<string, int>();

            IQueryable<Venta> _ventaQuery = await _ventaRepositorio.Consultar();

            if (_ventaQuery.Count() > 0)
            {
                var tablaVenta = retornarVentasAño(_ventaQuery, año);

                resultado = tablaVenta
                    .GroupBy(v => v.Vendedor).OrderBy(g => g.Key)
                    .Select(dv => new { vendedor = dv.Key.ToString(), total = dv.Count() }).OrderByDescending(g => g.total)
                    .ToDictionary(r => r.vendedor, r => r.total);
            }

            return resultado;
        }

        public async Task<DashBoardDTO> ResumenVendedores(int año)
        {
            DashBoardDTO vmDahsBoard = new DashBoardDTO();
            try
            {
                vmDahsBoard.TotalVentas = await TotalVentasAño(año);
                vmDahsBoard.TotalIngresos = await TotalIngresosAño(año);
                vmDahsBoard.TotalVendedores = await TotalVendedores();

                int i = 0;
                List<DatosDashDTO> listaVentaMes = new List<DatosDashDTO>();

                foreach (KeyValuePair<string, int> item in await VendedoresAño(año))
                {
                    if (i > 4)
                    {
                        break;
                    }

                    listaVentaMes.Add(new DatosDashDTO()
                    {
                        Descripcion = item.Key,
                        Total = item.Value
                    });
                    i++;
                }

                vmDahsBoard.DatosDash = listaVentaMes;
            }
            catch
            {
                throw;
            }

            return vmDahsBoard;
        }


        #endregion

    }
}
