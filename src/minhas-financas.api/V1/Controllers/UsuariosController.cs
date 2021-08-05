using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using minhas_financas.api.Controllers;
using minhas_financas.api.Extensions;
using minhas_financas.api.V1.ViewModels;
using minhas_financas.business.Interfaces;
using System;
using System.Threading.Tasks;

namespace minhas_financas.api.V1.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/usuarios")]
    public class UsuariosController : MainController
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtGeradorToken _geradorToken;
        
        public UsuariosController(INotificador notificador,
                                  SignInManager<IdentityUser> signInManager,
                                  UserManager<IdentityUser> userManager,
                                  JwtGeradorToken geradorToken) : base(notificador)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _geradorToken = geradorToken;
        }

        [HttpGet("{id}")]
        public ActionResult Obter(Guid id)
        {
            return CustomResponse();
        }

        [HttpPost]
        public async Task<ActionResult> Adicionar(UserViewModel createUser)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var user = new IdentityUser()
            {
                UserName = createUser.Email,
                Email = createUser.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, createUser.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);
                return CustomCreatedResponse(nameof(Obter), user.Id, await _geradorToken.GerarToken(user.Email));
            }

            foreach (var error in result.Errors)
            {
                NotificarErro(error.Description);
            }

            return CustomResponse(createUser);
        }
    }
}
