using SistemaVenta.API.Controllers;
using SistemaVenta.BLL.Servicios.Interfaces;

namespace ControllersTesting
{
    public class ActivoTest
    {
        private readonly ActivoController controller;
        private readonly IActivoService service;

        public ActivoTest(IActivoService service, ActivoController controller)
        {
            this.service = service;
            this.controller = controller;
        }

        [Fact]
        public void GetActivos()
        {
            var res = controller.Lista();

        }
    }
}