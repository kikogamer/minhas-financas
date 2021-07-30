using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using minhas_financas.api.Controllers;
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

        public UsuariosController(INotificador notificador,
                                  SignInManager<IdentityUser> signInManager,
                                  UserManager<IdentityUser> userManager) : base(notificador)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet("{id}")]
        public ActionResult GetUser(Guid id)
        {
            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult> Registrar(CreateUserViewModel createUser)
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
                //return CustomResponse(await GerarJwt(user.Email));
                return CustomCreatedResponse(nameof(GetUser), user.Id, createUser);
            }

            foreach (var error in result.Errors)
            {
                NotificarErro(error.Description);
            }

            return CustomResponse(createUser);
        }
    }
}
