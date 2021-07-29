using Microsoft.AspNetCore.Mvc;
using minhas_financas.api.Controllers;
using minhas_financas.api.V1.ViewModels;
using minhas_financas.business.Interfaces;
using System;

namespace minhas_financas.api.V1.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/usuarios")]
    public class UsuariosController : MainController
    {
        public UsuariosController(INotificador notificador) : base(notificador)
        {
        }

        [HttpGet("{id}")]
        public ActionResult GetUser(Guid id)
        {
            return Ok();
        }

        [HttpPost]
        public ActionResult Registrar(CreateUserViewModel createUser)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            return CustomCreatedResponse(nameof(GetUser), Guid.Empty, createUser);
        }
    }
}
