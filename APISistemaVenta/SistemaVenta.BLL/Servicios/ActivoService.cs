using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SistemaVenta.BLL.Servicios.Interfaces;
using SistemaVenta.DAL.Repositorios.Interfaces;
using SistemaVenta.DTO;
using SistemaVenta.Model;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SistemaVenta.BLL.Servicios
{
    public class ActivoService: IActivoService
    {
        private readonly IGenericRepository<Activo> _activoRepositorio;
        private readonly IMapper _mapper;

        public ActivoService(IGenericRepository<Activo> activoRepositorio, IMapper mapper)
        {
            _activoRepositorio = activoRepositorio;
            _mapper = mapper;
        }

        public async Task<List<ActivoDTO>> Lista()
        {
            try
            {
                var listaActivos = await _activoRepositorio.Consultar();
                return _mapper.Map<List<ActivoDTO>>(listaActivos.ToList());
            }
            catch
            {
                throw;
            }
        }
        public async Task<ActivoDTO> Crear(ActivoDTO modelo)
        {
            try
            {
                var activoCreado = await _activoRepositorio.Crear(_mapper.Map<Activo>(modelo));

                if (activoCreado.IdActivo == 0)
                    throw new TaskCanceledException("No se pudo crear");

                return _mapper.Map<ActivoDTO>(activoCreado);

            }
            catch
            {
                throw;
            }
        }
        public async Task<bool> Editar(ActivoDTO modelo)
        {
            try
            {
                var activoModelo = _mapper.Map<Activo>(modelo);
                var activoEncontrado = await _activoRepositorio.Obtener(u =>
                    u.IdActivo == activoModelo.IdActivo
                );

                if (activoEncontrado == null)
                    throw new TaskCanceledException("El activo no existe");

                activoEncontrado.Nombre = activoModelo.Nombre;
                activoEncontrado.Marca = activoModelo.Marca;
                activoEncontrado.Codigo = activoModelo.Codigo;
                activoEncontrado.EsActivo = activoModelo.EsActivo;
                activoEncontrado.Cliente = activoModelo.Cliente;
                activoEncontrado.Encargado = activoModelo.Encargado;
                activoEncontrado.EsContrato = activoModelo.EsContrato;
                activoEncontrado.FechaCaducidad = activoModelo.FechaCaducidad;

                bool respuesta = await _activoRepositorio.Editar(activoEncontrado);

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
                var activoEncontrado = await _activoRepositorio.Obtener(p => p.IdActivo == id);

                if (activoEncontrado == null)
                    throw new TaskCanceledException("El activo no existe");

                bool respuesta = await _activoRepositorio.Eliminar(activoEncontrado);


                if (!respuesta)
                    throw new TaskCanceledException("No se pudo elminar"); ;

                return respuesta;

            }
            catch
            {
                throw;
            }
        }
        public async Task<List<ActivoDTO>> ListaSintrato()
        {
            try
            {
                IQueryable<Activo> query = await _activoRepositorio.Consultar();
                var ListaResultado = new List<Activo>();

                ListaResultado = await query.Where(v => v.EsContrato == false || v.EsContrato == null
                      ).ToListAsync();

                return _mapper.Map<List<ActivoDTO>>(ListaResultado);
            }
            catch
            {
                throw;
            }
        }
        public async Task<List<ActivoDTO>> ListaContrato()
        {
            try
            {
                IQueryable<Activo> query = await _activoRepositorio.Consultar();
                var ListaResultado = new List<Activo>();

                ListaResultado = await query.Where(v => v.EsContrato == true
                      ).ToListAsync();

                return _mapper.Map<List<ActivoDTO>>(ListaResultado);
            }
            catch
            {
                throw;
            }

        }
        public async Task<List<ActivoDTO>> FiltrosContratos(string criterioBusqueda, string encargado, string cliente)
        {
            IQueryable<Activo> query = await _activoRepositorio.Consultar();
            var ListaResultante = new List<Activo>();

            try
            {
                if (criterioBusqueda == "todo")
                {
                    ListaResultante = await query.Where(v => v.EsContrato == true 
                      ).ToListAsync();
                }
                if (criterioBusqueda == "caducados")
                {
                    string fecahActual = DateTime.Now.ToString("yyyy-MM-dd");
                    DateTime fech_Actual = DateTime.ParseExact(fecahActual, "yyyy-MM-dd", new CultureInfo("es-BO"));

                    ListaResultante = await query.Where(v =>
                        v.FechaCaducidad.Value.Date < fech_Actual.Date && v.EsContrato == true
                    ).ToListAsync();
                }
                if (criterioBusqueda == "vigentes")
                {
                    string fecahActual = DateTime.Now.ToString("yyyy-MM-dd");
                    DateTime fech_Actual = DateTime.ParseExact(fecahActual, "yyyy-MM-dd", new CultureInfo("es-BO"));

                    ListaResultante = await query.Where(v =>
                        v.FechaCaducidad.Value.Date >= fech_Actual.Date && v.EsContrato == true
                    ).ToListAsync();
                }
                if (criterioBusqueda == "encargados")
                {
                    ListaResultante = await query.Where(v => v.Encargado == encargado && v.EsContrato == true
                      ).ToListAsync();
                }
                if (criterioBusqueda == "clientes")
                {
                    ListaResultante = await query.Where(v => v.Cliente == cliente && v.EsContrato == true) 
                      .ToListAsync();
                }
            }
            catch
            {
                throw;
            }

            return _mapper.Map<List<ActivoDTO>>(ListaResultante);
        }
        public async Task<bool> EditarEstado(ActivoDTO modelo)
        {
            try
            {
                var activoModelo = _mapper.Map<Activo>(modelo);
                var activoEncontrado = await _activoRepositorio.Obtener(u =>
                    u.IdActivo == activoModelo.IdActivo
                );

                if (activoEncontrado == null)
                    throw new TaskCanceledException("El activo no existe");

                if (activoEncontrado.EsContrato == true)
                {
                    activoEncontrado.EsContrato = false;
                }
                else
                    activoEncontrado.EsContrato = true;

                activoEncontrado.FechaCaducidad = activoModelo.FechaCaducidad;


                bool respuesta = await _activoRepositorio.Editar(activoEncontrado);

                if (!respuesta)
                    throw new TaskCanceledException("No se pudo editar"); ;

                return respuesta;

            }
            catch
            {
                throw;
            }
        }
    }
}
