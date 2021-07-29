﻿using Microsoft.AspNetCore.Mvc;
using minhas_financas_api.Controllers;
using System.Threading.Tasks;

namespace minhas_financas_api.V1.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}")]
    public class UsuariosController : MainController
    {

        [HttpPost("nova-conta")]
        public async Task<ActionResult> Registrar()
        {
            return Ok();
        }
    }
}
