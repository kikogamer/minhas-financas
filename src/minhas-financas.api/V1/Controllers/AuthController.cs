using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using minhas_financas.api.Controllers;
using minhas_financas.api.Extensions;
using minhas_financas.api.V1.ViewModels;
using minhas_financas.business.Interfaces;
using System.Threading.Tasks;

namespace minhas_financas.api.V1.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}")]
    public class AuthController : MainController
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly JwtGeradorToken _geradorToken;

        public AuthController(INotificador notificador,
                              SignInManager<IdentityUser> signInManager,
                              JwtGeradorToken geradorToken) : base(notificador)
        {
            _signInManager = signInManager;
            _geradorToken = geradorToken;
        }

        [HttpPost("entrar")]
        public async Task<ActionResult> Login(LoginUserViewModel loginUser)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var result = await _signInManager.PasswordSignInAsync(loginUser.Email, 
                                                                  loginUser.Password, 
                                                                  false, 
                                                                  true);

            if (result.Succeeded)
                return CustomResponse(await _geradorToken.GerarToken(loginUser.Email));
            
            if (result.IsLockedOut)
            {
                NotificarErro("Usuário temporariamente bloqueado por tentativas inválidas");
                return CustomResponse(loginUser);
            }

            NotificarErro("Usuário ou Senha incorretos");
            return CustomResponse(loginUser);
        }
    }
}
