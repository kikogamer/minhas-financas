using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class UsuariosController : MainController
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtGeradorToken _geradorToken;
        
        public UsuariosController(INotificador notificador,
                                  SignInManager<IdentityUser> signInManager,
                                  UserManager<IdentityUser> userManager,
                                  JwtGeradorToken geradorToken,
                                  IUser appUser) : base(notificador, appUser)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _geradorToken = geradorToken;
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult> Obter(Guid id)
        {
            if (!UsuarioAutenticado() || GetUsuarioId() != id) return NotFound();

            var usuario = await _userManager.FindByIdAsync(id.ToString());

            if (usuario == null) return NotFound();

            var userView = new UserViewModel()
            {
                Id = id,
                Nome = usuario.UserName,
                Email = usuario.Email
            };

            return CustomResponse(userView);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> Adicionar(CreateUserViewModel createUser)
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

        [HttpPut("{id:guid}")]
        public async Task<ActionResult> Alterar(Guid id, EditUserViewModel editUser)
        {
            if (!UsuarioAutenticado() || GetUsuarioId() != id) return NotFound();

            if (!ModelState.IsValid) return CustomResponse(ModelState);
            
            if (editUser == null || id != editUser.Id)
            {
                NotificarErro("O id informado não é o mesmo que foi passado na query");
                return CustomResponse(editUser);
            }
            
            var usuarioAtualizacao = await _userManager.FindByIdAsync(id.ToString());

            if (usuarioAtualizacao == null) return NotFound();

            usuarioAtualizacao.UserName = editUser.Nome;
            usuarioAtualizacao.Email = editUser.Email;

            await _userManager.UpdateAsync(usuarioAtualizacao);

            return CustomResponse(editUser);
        }
    }
}
