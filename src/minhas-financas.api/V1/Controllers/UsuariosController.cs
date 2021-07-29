using Microsoft.AspNetCore.Mvc;
using minhas_financas.api.Controllers;
using System.Threading.Tasks;

namespace minhas_financas.api.V1.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/usuarios")]
    public class UsuariosController : MainController
    {

        [HttpPost]
        public async Task<ActionResult> Registrar()
        {
            return Ok();
        }
    }
}
