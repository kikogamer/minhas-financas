using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using minhas_financas.api.Extensions;
using minhas_financas.api.V1.Controllers;
using minhas_financas.api.V1.ViewModels;
using minhas_financas.business.Interfaces;
using minhas_financas.business.Notificacoes;
using Moq;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace minhas_financas.api.ut.V1.Controllers
{
    public class AuthControllerTest
    {
        private readonly Fixture _fixture;
        private readonly Mock<INotificador> _mockNotificador;
        private readonly Mock<SignInManager<IdentityUser>> _mockSignInManager;
        private readonly Mock<JwtGeradorToken> _mockJwtGeradorToken;
        private readonly AuthController _controller;

        public AuthControllerTest()
        {
            _fixture = new Fixture();
            _mockNotificador = new Mock<INotificador>();
            var mockUserManager = new Mock<UserManager<IdentityUser>>(Mock.Of<IUserStore<IdentityUser>>(),
                                                                      null,
                                                                      null,
                                                                      null,
                                                                      null,
                                                                      null,
                                                                      null,
                                                                      null,
                                                                      null);
            _mockSignInManager = new Mock<SignInManager<IdentityUser>>(mockUserManager.Object,
                                                                       Mock.Of<IHttpContextAccessor>(),
                                                                       Mock.Of<IUserClaimsPrincipalFactory<IdentityUser>>(),
                                                                       null,
                                                                       null,
                                                                       null,
                                                                       null);
            _mockJwtGeradorToken = new Mock<JwtGeradorToken>();
            _controller = new AuthController(_mockNotificador.Object,
                                             _mockSignInManager.Object,
                                             _mockJwtGeradorToken.Object);
        }

        [Fact]
        public async Task Login_RetornaOk_QuandoModelEhValido()
        {
            _mockSignInManager.Setup(signInManager =>
                signInManager.PasswordSignInAsync(It.IsAny<string>(), 
                                                  It.IsAny<string>(), 
                                                  It.IsAny<bool>(),
                                                  It.IsAny<bool>()))
                    .Returns(Task.FromResult(Microsoft.AspNetCore.Identity.SignInResult.Success));

            _mockJwtGeradorToken.Setup(geradorToken => geradorToken.GerarToken(It.IsAny<string>()))
                .ReturnsAsync(_fixture.Create<LoginResponseViewModel>());

            var loginUser = _fixture.Create<LoginUserViewModel>();
            var result = await _controller.Login(loginUser);
            var returnValue = Assert.IsType<OkObjectResult>(result);
            var okResponse = Assert.IsType<ApiOkResponse>(returnValue.Value);
            Assert.True(okResponse.Success);
            Assert.IsType<LoginResponseViewModel>(okResponse.Data);
        }

        [Fact]
        public async Task Login_RetornaBadRequest_QuandoModelEhInvalido()
        {
            var notificacoes = _fixture.CreateMany<Notificacao>().ToList();
            _mockNotificador.Setup(notificador => notificador.TemNotificacao()).Returns(true);
            _mockNotificador.Setup(notificador => notificador.ObterNotificacoes()).Returns(notificacoes);

            foreach (var notificacao in notificacoes)
            {
                _controller.ModelState.AddModelError(_fixture.Create<string>(), notificacao.Mensagem);
            }

            var result = await _controller.Login(loginUser: null);
            var returnValue = Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResponse = Assert.IsType<ApiBadRequestResponse>(returnValue.Value);
            Assert.False(badRequestResponse.Success);
            Assert.Equal(badRequestResponse.Erros, notificacoes.Select(n => n.Mensagem).ToList());
        }

        [Fact]
        public async Task Login_RetornaBadRequest_QuandoUsuarioEstaBloqueado()
        {
            var notificacoes = _fixture.CreateMany<Notificacao>().ToList();
            _mockNotificador.Setup(notificador => notificador.TemNotificacao()).Returns(true);
            _mockNotificador.Setup(notificador => notificador.ObterNotificacoes()).Returns(notificacoes);

            _mockSignInManager.Setup(signInManager =>
                signInManager.PasswordSignInAsync(It.IsAny<string>(),
                                                  It.IsAny<string>(),
                                                  It.IsAny<bool>(),
                                                  It.IsAny<bool>()))
                    .Returns(Task.FromResult(Microsoft.AspNetCore.Identity.SignInResult.LockedOut));

            var loginUser = _fixture.Create<LoginUserViewModel>();
            var result = await _controller.Login(loginUser);
            var returnValue = Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResponse = Assert.IsType<ApiBadRequestResponse>(returnValue.Value);
            Assert.False(badRequestResponse.Success);
            Assert.Equal(badRequestResponse.Erros, notificacoes.Select(n => n.Mensagem).ToList());
        }

        [Fact]
        public async Task Login_RetornaBadRequest_QuandoAsCredenciaisSaoInvalidas()
        {
            var notificacoes = _fixture.CreateMany<Notificacao>().ToList();
            _mockNotificador.Setup(notificador => notificador.TemNotificacao()).Returns(true);
            _mockNotificador.Setup(notificador => notificador.ObterNotificacoes()).Returns(notificacoes);

            _mockSignInManager.Setup(signInManager =>
                signInManager.PasswordSignInAsync(It.IsAny<string>(),
                                                  It.IsAny<string>(),
                                                  It.IsAny<bool>(),
                                                  It.IsAny<bool>()))
                    .Returns(Task.FromResult(Microsoft.AspNetCore.Identity.SignInResult.Failed));

            var loginUser = _fixture.Create<LoginUserViewModel>();
            var result = await _controller.Login(loginUser);
            var returnValue = Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResponse = Assert.IsType<ApiBadRequestResponse>(returnValue.Value);
            Assert.False(badRequestResponse.Success);
            Assert.Equal(badRequestResponse.Erros, notificacoes.Select(n => n.Mensagem).ToList());
        }
    }
}
