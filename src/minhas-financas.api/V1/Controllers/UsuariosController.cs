using Microsoft.AspNetCore.Mvc;
using minhas_financas.api.Controllers;
using minhas_financas.api.V1.ViewModels;

namespace minhas_financas.api.V1.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/usuarios")]
    public class UsuariosController : MainController
    {

        [HttpPost]
        public ActionResult Registrar(CreateUserViewModel createUser)
        {
            return Ok(createUser);
        }
    }
}
