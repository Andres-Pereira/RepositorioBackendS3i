using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaVenta.DTO;
using SistemaVenta.API.Utilidad;
using SistemaVenta.BLL.Servicios.Interfaces;

namespace SistemaVenta.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivoController : ControllerBase
    {
        private readonly IActivoService _activoServicio;

        public ActivoController(IActivoService activoServicio)
        {
            _activoServicio = activoServicio;
        }

        [HttpGet]
        [Route("Lista")]
        public async Task<IActionResult> Lista()
        {
            var rsp = new Response<List<ActivoDTO>>();

            try
            {
                rsp.status = true;
                rsp.value = await _activoServicio.Lista();

            }
            catch (Exception ex)
            {
                rsp.status = false;
                rsp.msg = ex.Message;

            }
            return Ok(rsp);
        }

        [HttpGet]
        [Route("ListaSintrato")]
        public async Task<IActionResult> ListaSintrato()
        {
            var rsp = new Response<List<ActivoDTO>>();

            try
            {
                rsp.status = true;
                rsp.value = await _activoServicio.ListaSintrato();

            }
            catch (Exception ex)
            {
                rsp.status = false;
                rsp.msg = ex.Message;

            }
            return Ok(rsp);
        }

        [HttpGet]
        [Route("ListaContrato")]
        public async Task<IActionResult> ListaContrato()
        {
            var rsp = new Response<List<ActivoDTO>>();

            try
            {
                rsp.status = true;
                rsp.value = await _activoServicio.ListaContrato();

            }
            catch (Exception ex)
            {
                rsp.status = false;
                rsp.msg = ex.Message;

            }
            return Ok(rsp);
        }


        [HttpPost]
        [Route("Guardar")]
        public async Task<IActionResult> Guardar([FromBody] ActivoDTO activo)
        {
            var rsp = new Response<ActivoDTO>();

            try
            {
                rsp.status = true;
                rsp.value = await _activoServicio.Crear(activo);

            }
            catch (Exception ex)
            {
                rsp.status = false;
                rsp.msg = ex.Message;

            }
            return Ok(rsp);

        }


        [HttpPut]
        [Route("Editar")]
        public async Task<IActionResult> Editar([FromBody] ActivoDTO activo)
        {
            var rsp = new Response<bool>();

            try
            {
                rsp.status = true;
                rsp.value = await _activoServicio.Editar(activo);

            }
            catch (Exception ex)
            {
                rsp.status = false;
                rsp.msg = ex.Message;

            }
            return Ok(rsp);

        }


        [HttpGet]
        [Route("Filtros")]
        public async Task<IActionResult> Filtros(string buscarPor, string? encargado, string? cliente)
        {
            var rsp = new Response<List<ActivoDTO>>();
            encargado = encargado is null ? "" : encargado;
            cliente = cliente is null ? "" : cliente;

            try
            {
                rsp.status = true;
                rsp.value = await _activoServicio.FiltrosContratos(buscarPor, encargado, cliente);

            }
            catch (Exception ex)
            {
                rsp.status = false;
                rsp.msg = ex.Message;

            }
            return Ok(rsp);
        }


        [HttpPut]
        [Route("EditarEstado")]
        public async Task<IActionResult> EditarEstado([FromBody] ActivoDTO activo)
        {
            var rsp = new Response<bool>();

            try
            {
                rsp.status = true;
                rsp.value = await _activoServicio.EditarEstado(activo);

            }
            catch (Exception ex)
            {
                rsp.status = false;
                rsp.msg = ex.Message;

            }
            return Ok(rsp);

        }


        [HttpDelete]
        [Route("Eliminar/{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var rsp = new Response<bool>();

            try
            {
                rsp.status = true;
                rsp.value = await _activoServicio.Eliminar(id);

            }
            catch (Exception ex)
            {
                rsp.status = false;
                rsp.msg = ex.Message;

            }
            return Ok(rsp);

        }

    }
}
